using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Paratext.PluginInterfaces;
using TamilSandhi;
using TamilSandhi.Plugin;

// Developer-only assembly. Never included in the product package.
public sealed class InHostAcceptance : IParatextStartupAutomaticPlugin
{
    public string Name { get { return "Tamil Sandhi QA developer acceptance"; } }
    public Version Version { get { return new Version(0,1); } }
    public string VersionString { get { return "developer-only"; } }
    public string Publisher { get { return "selvabenz"; } }
    public string GetDescription(string locale) { return "Opt-in disposable-project acceptance tests; not distributed."; }
    public IDataFileMerger GetMerger(IPluginHost host,string identifier) { return null; }
    public void Run(IPluginHost host)
    {
        string root=AcceptancePaths.Root;
        string request=Path.Combine(root,"request.txt");
        if(!File.Exists(request))return;
        File.Move(request,Path.Combine(root,"consumed-"+Guid.NewGuid().ToString("N")+".txt"));
        var results=new List<string>();
        HostStore store=null;string original=null,lastKnown=null;bool touched=false;
        try
        {
            if(host.ApplicationVersion!=new Version(9,5,110,1))throw new InvalidOperationException("Unexpected host version; refused.");
            var p=host.GetAllProjects(false).Single(x=>x.ShortName=="NSTest");
            if(p.LongName!="Note Suggestions Disposable Test"||p.IsResource)throw new InvalidOperationException("Disposable project identity mismatch; refused.");
            store=new HostStore(p,this);
            string recoveryRequest=Path.Combine(root,"recover-from.txt");
            if(File.Exists(recoveryRequest))
            {
                string previous=Path.GetFullPath(File.ReadAllText(recoveryRequest).Trim());
                if(Path.GetDirectoryName(previous)!=Path.GetDirectoryName(root)||!Path.GetFileName(previous).StartsWith("host-acceptance-",StringComparison.Ordinal))throw new InvalidOperationException("Invalid recovery directory.");
                string backup=File.ReadAllText(Path.Combine(previous,"original.usfm"),Encoding.UTF8);
                var checkpoint=new FileJournal(Path.Combine(previous,"journal.jsonl")).Read().Last();
                if(!backup.Contains("Synthetic test only")||checkpoint.State!="reverted"||checkpoint.Project!=p.ID)throw new InvalidOperationException("Recovery identity/checkpoint refused.");
                using(var held=store.Lock(1,1))
                {
                    if(held==null||TextIdentity.Hash(store.Read(1,1))!=checkpoint.AfterHash)throw new InvalidOperationException("Recovery source differs from verified journal checkpoint; refused.");
                    store.Write(1,1,backup);if(store.Read(1,1)!=backup)throw new InvalidOperationException("Recovery restoration mismatch.");
                }
                results.Add("previous-run-original-restored-from-exact-journal-checkpoint: PASS");
            }
            original=p.GetUSFM(1,1);
            if(!original.Contains("Synthetic test only"))throw new InvalidOperationException("Disposable chapter marker missing; refused.");
            if(!store.CanEdit(1,1))throw new InvalidOperationException("Disposable chapter not editable.");
            var product=Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"TamilSandhi.ptxplg"));
            var reviewType=product.GetType("TamilSandhi.Plugin.ReviewControl",true);
            using(var control=(IDisposable)Activator.CreateInstance(reviewType))
            {
                var work=(System.Collections.ICollection)reviewType.GetMethod("CreateWork",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(control,new object[]{p,p.Versification.CreateReference(1,1,1),2});
                if(work.Count!=1)throw new InvalidOperationException("Current chapter scope did not select exactly one chapter.");
            }
            results.Add("production-window-current-chapter-scope: PASS");
            File.WriteAllText(Path.Combine(root,"original.usfm"),original,new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(root,"original.sha256"),TextIdentity.Hash(original));
            results.Add("disposable-marker-and-edit-permission: PASS");
            string fixture=File.ReadAllText(Path.Combine(root,"fixture.usfm"),Encoding.UTF8);
            using(var held=store.Lock(1,1))
            {
                if(held==null)throw new InvalidOperationException("Fixture lock unavailable.");
                if(store.Read(1,1)!=original)throw new InvalidOperationException("Disposable chapter changed before fixture write.");
                touched=true;lastKnown=original;
                try{store.Write(1,1,fixture);lastKnown=store.Read(1,1);}
                catch{string current=store.Read(1,1);if(current==original||current==fixture)lastKnown=current;throw;}
            }
            var catalog=Json.Read<Catalog>(File.ReadAllText(Path.Combine(root,"rules.json"),Encoding.UTF8));
            var style=new HouseStyle();var engine=new SandhiEngine(catalog);
            Func<List<Finding>> scan=()=>engine.Analyze(new ChapterSnapshot{Project=p.ID,Book=1,Chapter=1,Text=store.Read(1,1)},style,null,null);
            var findings=scan().Where(f=>f.Actionable).ToList();
            if(findings.Count!=3)throw new InvalidOperationException("Expected three actionable synthetic findings, found "+findings.Count);
            results.Add("host-read-and-protected-USFM-scan: PASS");
            var service=new PatchService(store,new FileJournal(Path.Combine(root,"journal.jsonl")),BuildInfo.Version);
            string baseline=lastKnown;
            var applied=service.Apply(findings[0],findings[0].Suggested,"developer acceptance",false,style.Hash);
            lastKnown=store.Read(1,1);
            if(TextIdentity.Hash(lastKnown)!=applied.AfterHash)throw new InvalidOperationException("Apply hash mismatch.");
            results.Add("real-host-lock-apply-and-readback: PASS");
            bool stale=false;
            try{service.Apply(findings[1],findings[1].Suggested,"developer acceptance",false,style.Hash);}
            catch(InvalidOperationException){stale=findings[1].State==ReviewState.Stale;}
            if(!stale||store.Read(1,1)!=lastKnown)throw new InvalidOperationException("Stale finding was not blocked safely.");
            results.Add("stale-finding-rejected-without-write: PASS");
            service.Revert(applied,"developer acceptance");lastKnown=store.Read(1,1);
            if(lastKnown!=baseline)throw new InvalidOperationException("Revert did not restore fixture exactly.");
            results.Add("real-host-revert-exact: PASS");
            var modified=scan().First(f=>f.Actionable);
            var changed=service.Apply(modified,modified.Suggested+"!","developer acceptance",true,style.Hash);
            lastKnown=store.Read(1,1);
            if(changed.Action!="modify-and-apply")throw new InvalidOperationException("Modified action not journalled.");
            service.Revert(changed,"developer acceptance");lastKnown=store.Read(1,1);
            if(lastKnown!=baseline)throw new InvalidOperationException("Modified revert mismatch.");
            results.Add("modify-apply-and-revert: PASS");
            foreach(string raw in new[]{"\\id GEN Synthetic regression\n\\c 1\n\\p\n\\v 1 தொடக்கம் \\f + \\ft குறிப்பு\\f* அவரை பார்த்தான்.","\\id GEN Synthetic regression\n\\c 1\n\\p\n\\v 1 அவரை\nபார்த்தான்."})
            {
                using(var held=store.Lock(1,1))
                {
                    if(held==null||store.Read(1,1)!=lastKnown)throw new InvalidOperationException("Regression fixture lock/source mismatch.");
                    string beforeFixture=lastKnown;
                    try{store.Write(1,1,raw);lastKnown=store.Read(1,1);}
                    catch{string current=store.Read(1,1);if(current==beforeFixture||current==raw)lastKnown=current;throw;}
                }
                if(raw.Contains("அவரை\nபார்த்தான்")&&!lastKnown.Contains("அவரை\nபார்த்தான்")&&!lastKnown.Contains("அவரை\r\nபார்த்தான்")){results.Add("wrapped-source-host-acceptance: NOT_EXERCISED (host canonicalizes source wrapping; core whitespace regressions are separate)");continue;}
                string exactFixture=lastKnown;var target=scan().Single(f=>f.Actionable);
                var edit=service.Apply(target,target.Suggested,"developer acceptance",false,style.Hash);lastKnown=store.Read(1,1);
                service.Revert(edit,"developer acceptance",target);lastKnown=store.Read(1,1);
                if(lastKnown!=exactFixture||target.State!=ReviewState.Reverted||(string)FindingExport.ToSchema(target)["status"]=="accepted")throw new InvalidOperationException("Regression fixture revert/outcome mismatch.");
                results.Add(raw.Contains("\\ft")?"post-note-host-apply-revert-and-export: PASS":"wrapped-source-host-apply-revert-and-export: PASS");
            }
        }
        catch(Exception ex){results.Add("ERROR: "+ex.ToString());}
        finally
        {
            if(touched&&store!=null)
            {
                try
                {
                    using(var held=store.Lock(1,1))
                    {
                        if(held==null)throw new InvalidOperationException("Cleanup lock unavailable; backup retained.");
                        if(store.Read(1,1)!=lastKnown)throw new InvalidOperationException("Unexpected outside change; restoration refused; backup retained.");
                        store.Write(1,1,original);
                        if(store.Read(1,1)!=original)throw new InvalidOperationException("Original chapter restoration not exact; backup retained.");
                    }
                    results.Add("original-disposable-chapter-restored-exactly: PASS");
                }
                catch(Exception ex){results.Add("RESTORE ERROR: "+ex.ToString());}
            }
            File.WriteAllText(Path.Combine(root,"report.json"),Json.Write(new{build=BuildInfo.Version,host=host.ApplicationVersion.ToString(),utc=DateTime.UtcNow.ToString("o"),results=results}),new UTF8Encoding(false));
        }
    }
}

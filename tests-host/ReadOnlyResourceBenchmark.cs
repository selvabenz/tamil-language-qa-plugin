using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Paratext.PluginInterfaces;
using TamilSandhi;

// Developer-only, one-shot read-only benchmark. Never part of the product ZIP.
// Uses public SDK APIs only. Does not request a write lock, call PutUSFM or bypass resource access restrictions.
public sealed class ReadOnlyResourceBenchmark:IParatextStartupAutomaticPlugin
{
    public string Name{get{return "Tamil Sandhi read-only resource benchmark";}}
    public Version Version{get{return new Version(0,1);}}
    public string VersionString{get{return "developer-only read-only";}}
    public string Publisher{get{return "selvabenz";}}
    public string GetDescription(string locale){return "One-shot local comparison against user-selected reviewed Tamil resources. No Scripture writes.";}
    public IDataFileMerger GetMerger(IPluginHost host,string identifier){return null;}
    private static string[] selectedNames;
    private static bool Selected(IReadOnlyProject p){return selectedNames.Contains(p.ShortName);}
    public void Run(IPluginHost host)
    {
        string root=ResourceBenchmarkPaths.Root,request=Path.Combine(root,"request.txt");
        if(!File.Exists(request))return;
        File.Move(request,Path.Combine(root,"consumed.txt"));
        var summaries=new List<object>();var privateFindings=new List<object>();var errors=new List<string>();
        var inventory=new List<object>();
        try
        {
            if(host.ApplicationVersion!=new Version(9,5,110,1))throw new InvalidOperationException("Unexpected host version");
            selectedNames=Json.Read<string[]>(File.ReadAllText(Path.Combine(root,"selected-resources.json"),Encoding.UTF8));
            if(selectedNames.Length==0||selectedNames.Distinct().Count()!=selectedNames.Length||selectedNames.Any(n=>!new[]{"OTCV","TAOVBSI","TaOVM+"}.Contains(n)))throw new InvalidOperationException("Resource identity allowlist refused");
            foreach(var p in host.AllResources.Concat(host.AllEnhancedResources))inventory.Add(new{shortName=p.ShortName,longName=p.LongName,selected=Selected(p)});
            File.WriteAllText(Path.Combine(root,"inventory.json"),Json.Write(inventory),new UTF8Encoding(false));
            // Host may require an approved resource access token. Respect any refusal.
            var resources=host.GetAllProjects(true).Where(p=>p.IsResource&&Selected(p)).ToArray();
            foreach(string missing in selectedNames.Where(n=>!resources.Any(p=>p.ShortName==n)))errors.Add("Public SDK did not expose selected resource "+missing+" as a readable IProject; not substituted.");
            var catalog=Json.Read<Catalog>(File.ReadAllText(Path.Combine(root,"rules.json"),Encoding.UTF8));var engine=new SandhiEngine(catalog);
            foreach(var p in resources)
            {
                int chapters=0,verses=0,actionable=0,unresolved=0,supported=0,unassessed=0,recoveryCandidates=0,recovered=0;
                var byRule=new Dictionary<string,int>();
                bool readable=true;
                try
                {
                foreach(var book in p.AvailableBooks.Where(b=>new[]{1,5,19,20,23,40,42,43,44,45,58,66}.Contains(b.Number)).OrderBy(b=>b.Number))
                {
                    int last=p.Versification.GetLastChapter(book.Number);
                    foreach(int chapter in new[]{1,Math.Max(1,last/2),last}.Distinct())
                    {
                        string text=p.GetUSFM(book.Number,chapter);string hash=TextIdentity.Hash(text);
                        var snapshot=new ChapterSnapshot{Project="local-reviewed-resource",Book=book.Number,Chapter=chapter,Text=text};
                        var report=engine.AnalyzeDetailed(snapshot,new HouseStyle(),null,null);
                        chapters++;verses+=Usfm.Segments(text).Select(s=>s.Verse).Distinct().Count();supported+=report.SupportedBoundaries;unassessed+=report.UnassessedBoundaries;
                        foreach(var f in report.Findings)
                        {
                            if(f.Actionable){actionable++;if(!byRule.ContainsKey(f.Rule))byRule[f.Rule]=0;byRule[f.Rule]++;}else unresolved++;
                            privateFindings.Add(new{resource=p.ShortName,book=book.Number,chapter=chapter,verse=f.Verse,original=f.Original,suggested=f.Suggested,rule=f.Rule,context=f.Context,blockers=f.Blockers});
                        }
                        // Controlled missing-ottu recovery: derived from actual reviewed boundaries,
                        // not independent natural-error recall. Count every eligible written boundary.
                        foreach(var segment in Usfm.Segments(text))
                        foreach(Match pair in Regex.Matches(text.Substring(segment.Start,segment.Length),@"(?=(?<pair>(?<a>[\u0B80-\u0BFF]+[கசதப]்)(?<gap>[ \t\r\n]+)(?<b>[கசதப][\u0B80-\u0BFF]*)))"))
                        {
                            // Lookahead sees overlapping boundaries; only accept actual word starts.
                            if(pair.Index>0&&Regex.IsMatch(text.Substring(segment.Start+pair.Index-1,1),@"[\u0B80-\u0BFF]"))continue;
                            recoveryCandidates++;string a=pair.Groups["a"].Value;
                            string corrupt=a.Substring(0,a.Length-2)+pair.Groups["gap"].Value+pair.Groups["b"].Value;
                            var mutation=new ChapterSnapshot{Project="controlled-mutation",Book=book.Number,Chapter=chapter,Text=corrupt};
                            var findings=engine.Analyze(mutation,new HouseStyle(),null,null);
                            if(findings.Count==1&&findings[0].Actionable&&findings[0].Suggested==pair.Groups["pair"].Value)recovered++;
                        }
                        if(TextIdentity.Hash(p.GetUSFM(book.Number,chapter))!=hash)throw new InvalidOperationException("Resource changed during read-only comparison; discard this run");
                    }
                }
                }
                catch(Exception ex){readable=false;errors.Add(p.ShortName+": "+ex.GetType().Name+": "+ex.Message);}
                summaries.Add(new{resource=p.ShortName,longName=p.LongName,readCompleted=readable,chapters=chapters,verseUnits=verses,supportedBoundaries=supported,unassessedBoundaries=unassessed,proposedChangesToReviewedText=actionable,reviewOnlyFindings=unresolved,proposedChangesByRule=byRule,controlledMissingOttuCandidates=recoveryCandidates,controlledMissingOttuRecovered=recovered});
            }
        }
        catch(Exception ex){errors.Add(ex.GetType().Name+": "+ex.Message);}
        File.WriteAllText(Path.Combine(root,"private-findings.json"),Json.Write(privateFindings),new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(root,"summary.json"),Json.Write(new{build=BuildInfo.Version,profile=ScriptureRuleProfile.Version,host=host.ApplicationVersion.ToString(),readOnly=true,accuracyCertified=false,notes="Baseline disagreements require review. Controlled mutation recovery is coverage, not independent precision/recall. Scripture text stays local.",resources=summaries,errors=errors}),new UTF8Encoding(false));
    }
}

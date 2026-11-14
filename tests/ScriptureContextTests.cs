using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TamilSandhi;

internal static class ScriptureContextTests
{
    private class FixtureSet {public List<Fixture> cases {get;set;}}
    private class Fixture {public string id {get;set;}public string usfm {get;set;}public string original {get;set;}public string replacement {get;set;}public string operation {get;set;}}
    private sealed class Store:IChapterStore
    {
        public string Text;private bool locked;
        public string ProjectId{get{return "synthetic-context";}}
        public string Read(int b,int c){return Text;}
        public bool CanEdit(int b,int c){return true;}
        public IDisposable Lock(int b,int c){if(locked)return null;locked=true;return new Release(()=>locked=false);}
        public void Write(int b,int c,string value){if(!locked)throw new Exception("Write without lock");Text=value;}
    }
    private sealed class Release:IDisposable {private readonly Action run;public Release(Action a){run=a;}public void Dispose(){run();}}
    private sealed class Journal:IJournal
    {
        private readonly List<AuditRecord> rows=new List<AuditRecord>();
        public void Append(AuditRecord row){rows.Add(row);}
        public IList<AuditRecord> Read(){return rows;}
    }
    private static ChapterSnapshot Snapshot(string text){return new ChapterSnapshot{Project="synthetic-context",Book=1,Chapter=1,Text=text};}
    public static void Run(string root,Catalog catalog,Action<bool,string> check)
    {
        var engine=new SandhiEngine(catalog);var style=new HouseStyle();
        var fixtures=Json.Read<FixtureSet>(File.ReadAllText(Path.Combine(root,"tests/fixtures/scripture-context.json"),Encoding.UTF8));
        var results=new List<object>();
        var exports=new List<object>();
        foreach(var row in fixtures.cases)
        {
            var analysis=engine.AnalyzeDetailed(Snapshot(row.usfm),style,null,null);var findings=analysis.Findings;
            check(analysis.ExaminedBoundaries==analysis.SupportedBoundaries+analysis.UnresolvedBoundaries+analysis.UnassessedBoundaries+analysis.SuppressedBoundaries,"coverage accounting "+row.id);
            if(row.operation=="none")check(findings.Count==0,"protected/correct Scripture context "+row.id);
            else
            {
                check(findings.Count==1,"exact finding count "+row.id);var f=findings.Single();
                exports.Add(FindingExport.ToSchema(f));
                check(f.Original==row.original&&f.Suggested==row.replacement&&f.Operation==row.operation,"Scripture context outcome "+row.id);
                check(f.Context.Contains(row.original)&&f.Reference.StartsWith("1 1:"),"verse context/reference "+row.id);
                if(row.operation=="review")
                {
                    check(!f.Actionable&&f.State==ReviewState.NeedsReview,"ambiguous/unsupported cannot apply "+row.id);
                    check(FindingExport.ToSchema(f)["suggested_patch"]==null,"review export has no patch "+row.id);
                }
                else
                {
                    var store=new Store{Text=row.usfm};var service=new PatchService(store,new Journal(),"context-test");
                    var applied=service.Apply(f,f.Suggested,"test",false,style.Hash);
                    check(store.Text==row.usfm.Replace(row.original,row.replacement),"only expected source bytes change "+row.id);
                    check(engine.Analyze(Snapshot(store.Text),style,null,null).Count==0,"corrected context is idempotent "+row.id);
                    service.Revert(applied,"test",f);check(store.Text==row.usfm,"exact Scripture context revert "+row.id);
                }
            }
            results.Add(new{id=row.id,outcome=row.operation,findings=findings.Count,supported=analysis.SupportedBoundaries,unresolved=analysis.UnresolvedBoundaries,unassessed=analysis.UnassessedBoundaries});
        }
        // Surface morphology alone cannot overrule ambiguity, catalog register or tier.
        var forged=new BoundaryEvidence{LeftStart=0,Rule="CASE-002-OVERT",Tags=new Dictionary<string,string>{{"case_marker","accusative"},{"lexical_ai_flag","false"}}};
        check(engine.Analyze(Snapshot("அவரை கொடுத்தான்"),style,null,new[]{forged}).All(f=>!f.Actionable),"homograph cannot be licensed by asserted tags");
        foreach(string field in new[]{"layer","tier","decision"})
        {
            var changed=Json.Read<Catalog>(Json.Write(catalog));var rule=changed.rules.Single(r=>r.id=="HARD-DEM");
            if(field=="layer")rule.layer="classical";if(field=="tier")rule.tier="review";if(field=="decision")rule.decision="REVIEW";
            check(new SandhiEngine(changed).Analyze(Snapshot("இந்த புத்தகம்"),style,null,null).All(f=>!f.Actionable),"catalog execution gate "+field);
        }
        var house=new HouseStyle();house.NoInsertBefore.Add("புத்தகம்");
        var suppressed=engine.AnalyzeDetailed(Snapshot("இந்தக் புத்தகம்"),house,null,null);
        check(suppressed.Findings.Count==0&&suppressed.SuppressedBoundaries==1,"editorial override suppresses replacement with separate accounting");
        var unknown=engine.AnalyzeDetailed(Snapshot("அபிகாயில் பேசினார்"),style,null,null);
        check(unknown.SupportedBoundaries==0&&unknown.UnassessedBoundaries==1&&unknown.Summary.Contains("not a correctness certificate"),"no finding is not a correctness claim");
        var capabilities=ScriptureRuleProfile.Capabilities(catalog);
        check(capabilities.Count==183&&capabilities.Count(p=>p.Value.Contains("bounded lexical"))==16,"complete explicit capability inventory");
        File.WriteAllText(Path.Combine(root,"artifacts/build/scripture-context-results.json"),Json.Write(new{syntheticOnly=true,independentAccuracyMeasured=false,cases=results}),Encoding.UTF8);
        File.WriteAllText(Path.Combine(root,"artifacts/build/scripture-finding-schema-samples.json"),Json.Write(exports),Encoding.UTF8);
    }
}

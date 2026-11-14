using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TamilSandhi;

internal static class CoreTests
{
    private static int count;
    private static void Check(bool passed,string name) { count++;if(!passed) throw new Exception("FAIL: "+name); }
    private static void Throws(Action action,string name) { bool threw=false;try{action();}catch{threw=true;}Check(threw,name); }
    private static Catalog catalog;
    private static SandhiEngine engine;
    private static ChapterSnapshot Snapshot(string s) { return new ChapterSnapshot { Project="test",Book=1,Chapter=1,Text=s }; }
    private static List<Finding> Scan(string s) { return engine.Analyze(Snapshot(s),new HouseStyle(),null,null); }
    private sealed class MemoryStore : IChapterStore
    {
        public string ProjectId {get{return "test";}}
        public string Text;
        public bool Editable=true,Locked,FailWrite,MutateOnLock,NormalizeOnWrite;
        public int Writes;public Action OnLock;
        public string Read(int b,int c){return Text;}
        public bool CanEdit(int b,int c){return Editable;}
        public IDisposable Lock(int b,int c){if(Locked)return null;Locked=true;if(MutateOnLock)Text+=" later";if(OnLock!=null)OnLock();return new Handle(()=>Locked=false);}
        public void Write(int b,int c,string text){Check(Locked,"writes require lock");Writes++;if(FailWrite)throw new IOException("Simulated write failure");Text=NormalizeOnWrite?text+" ":text;}
    }
    private sealed class Handle : IDisposable { private Action close; public Handle(Action c){close=c;}public void Dispose(){close();} }
    private sealed class MemoryJournal : IJournal
    {
        public bool Fail; public List<AuditRecord> Rows=new List<AuditRecord>();
        public void Append(AuditRecord r){if(Fail)throw new IOException("Journal unavailable");Rows.Add(Json.Read<AuditRecord>(Json.Write(r)));}
        public IList<AuditRecord> Read(){return Rows.ToList();}
    }
    public static int Main(string[] args)
    {
        try
        {
            string root=args[0];catalog=Json.Read<Catalog>(File.ReadAllText(Path.Combine(root,"reference/tamil_punarchi_agent_package/rules.json"),Encoding.UTF8));engine=new SandhiEngine(catalog);
            Check(catalog.rules.Count==183,"catalog count");
            var dup=Json.Read<Catalog>(Json.Write(catalog));dup.rules[1].id=dup.rules[0].id;Throws(()=>dup.Validate(),"duplicate catalog rejected");
            Check(Scan("அறிக்கை செய்தான்").Count==0,"lexical ai protected");
            Check(Scan("வந்து சென்றான்").Count==0,"soft participle protected");
            Check(Scan("படித்து கொடுத்தான்").Single().Suggested=="படித்துக் கொடுத்தான்","hard participle evidence");
            Check(Scan("அவரை பார்த்தான்").Single().Suggested=="அவரைப் பார்த்தான்","accusative evidence");
            Check(Scan("இந்த புத்தகம்").Single().Suggested=="இந்தப் புத்தகம்","demonstrative noun evidence");
            Check(Scan("அவரைப் பார்த்தான்").Count==0,"idempotent corrected input");
            Check(Scan("மறை செய்தான்").All(f=>f.Suggested==null),"unknown suffix abstention");
            Check(Scan("படித்து, கொடுத்தான்").Count==0,"punctuation barrier");
            foreach(string whitespace in new[]{"\n","\r\n","\r","\t"," \r\n  "})
            {
                string wrapped="\\c 1\n\\p\n\\v 1 அவரை"+whitespace+"பார்த்தான்.";
                var wrappedFinding=Scan(wrapped).Single();
                Check(wrappedFinding.Suggested=="அவரைப்"+whitespace+"பார்த்தான்","source whitespace preserved in suggestion");
                var wrappedStore=new MemoryStore{Text=wrapped};
                var wrappedService=new PatchService(wrappedStore,new MemoryJournal(),"regression");
                var wrappedApplied=wrappedService.Apply(wrappedFinding,wrappedFinding.Suggested,"test",false,new HouseStyle().Hash);
                Check(wrappedStore.Text==wrapped.Replace("அவரை","அவரைப்"),"wrapped apply changes only the reviewed text");
                wrappedService.Revert(wrappedApplied,"test");Check(wrappedStore.Text==wrapped,"wrapped revert restores exact USFM");
                Throws(()=>PatchService.Patch(wrapped,wrappedFinding.Start,wrappedFinding.Original,wrappedFinding.Suggested.Replace(whitespace," "),"1"),"cannot remove source line breaks or tabs");
            }
            string sfm="\\c 1\r\n\\p\r\n\\v 1 அவரை பார்த்தான். \\f + \\ft அவரை பார்த்தான்.\\f*\r\n\\s அவரை பார்த்தான்\r\n\\p\r\n\\v 2 படித்து கொடுத்தான்.\r\n";
            var findings=Scan(sfm);Check(findings.Count==2,"USFM notes/headings excluded");Check(findings[1].Verse=="2","verse references");
            Check(sfm==Snapshot(sfm).Text,"scan source untouched");
            Check(Scan("\\v 1 \\w அவரை|lemma=\"x\"\\w* பார்த்தான்").Count==0,"word attributes protected");
            Check(Scan("\\v 1 அவரை \\f + \\ft note\\f* பார்த்தான்").Count==0,"no crossing notes");
            foreach(string note in new[]{"\\f + \\ft அவரை பார்த்தான்.\\f*","\\x + \\xo 1:1 \\xt அவரை பார்த்தான்.\\x*","\\f + \\ft குறிப்பு \\it அவரை பார்த்தான்.\\it*\\f*"})
            {
                string afterNote="\\c 1\n\\p\n\\v 1 தொடக்கம் "+note+" அவரை பார்த்தான்.";
                var afterFinding=Scan(afterNote).Single();
                Check(afterFinding.Start==afterNote.LastIndexOf("அவரை",StringComparison.Ordinal),"resume verse after closed note; exclude note contents");
                string afterPatch=PatchService.Patch(afterNote,afterFinding.Start,afterFinding.Original,afterFinding.Suggested,"1");
                Check(afterPatch.Contains(note)&&afterPatch.EndsWith("அவரைப் பார்த்தான்.",StringComparison.Ordinal),"patch after note preserves protected note byte-for-byte");
            }
            Check(Scan("\\v 1 தொடக்கம் \\f + \\ft அவரை பார்த்தான்.").Count==0,"unclosed note remains protected");
            foreach(string marker in new[]{"f","fe","x"})foreach(string resume in new[]{"\\p ","\\v 2 ","\\c 2\n\\p\n\\v 1 "})
            {
                string damaged="\\c 1\n\\p\n\\v 1 தொடக்கம் \\"+marker+" + \\ft குறிப்பு\n"+resume+"அவரை பார்த்தான்.";
                Check(Scan(damaged).Count==0,"unclosed note cannot resume through paragraph/verse/chapter markers");
                Throws(()=>PatchService.Patch(damaged,damaged.IndexOf("அவரை",StringComparison.Ordinal),"அவரை பார்த்தான்","அவரைப் பார்த்தான்","1"),"malformed note patch refused");
            }
            Check(Scan("\\v 1 \\unknown அவரை பார்த்தான்").Count==0,"unknown marker fail closed");
            Check(Scan("\\v 1 அவரை \\v 2 பார்த்தான்").Count==0,"no cross verse patches");
            Check(Scan("\\v 1-2 அவரை பார்த்தான்").Single().Verse=="1-2","bridge label retained");
            var one=Scan("அவரை பார்த்தான்").Single();Check(one.StartUtf8==0&&one.EndUtf8==Encoding.UTF8.GetByteCount(one.Original),"UTF8 offsets");
            Check(Scan("அவரை பார்த்தான்").Single().Id==one.Id,"stable scan identity");
            File.WriteAllText(Path.Combine(root,"artifacts/build/finding-schema-sample.json"),Json.Write(FindingExport.ToSchema(one)),Encoding.UTF8);
            Check(FindingExport.ToSchema(one).ContainsKey("suggested_patch"),"structured schema export");
            Check(!TextIdentity.Boundary("கா",1),"Tamil combining boundary");
            var selected=engine.Analyze(Snapshot("அவரை பார்த்தான்"),new HouseStyle(),new[]{new TextSpan{Start=0,Length=5}},null);Check(selected.Count==0,"partial selection limits");
            string chapter1="\\c 1\n\\v 1 அவரை பார்த்தான்.\n\\v 2 படித்து கொடுத்தான்.\n",chapter2="\\c 2\n\\v 1 இந்த புத்தகம்.\n";
            var first=Snapshot(chapter1);var second=new ChapterSnapshot{Project="test",Book=1,Chapter=2,Text=chapter2};string firstVerse="\\v 1 அவரை பார்த்தான்.\n";int selectionOffset=firstVerse.IndexOf("அவரை",StringComparison.Ordinal);string selectedRaw=(chapter1+chapter2).Substring(chapter1.IndexOf(firstVerse,StringComparison.Ordinal)+selectionOffset);
            var map=SelectionMapper.Map(new[]{first,second},firstVerse,selectionOffset,selectedRaw);Check(map.Count==2&&map["1:1"].Start==chapter1.IndexOf("அவரை",StringComparison.Ordinal)&&map["1:2"].Length==chapter2.Length,"exact multi-chapter selection");
            Throws(()=>SelectionMapper.Map(new[]{first,second},firstVerse,selectionOffset,selectedRaw+"changed"),"stale selected passage rejected");
            Throws(()=>SelectionMapper.Map(new[]{Snapshot(chapter1+chapter1)},firstVerse,selectionOffset,selectedRaw),"ambiguous selection anchor rejected");
            var house=new HouseStyle();house.NoInsertBefore.Add("பார்த்தான்");Check(engine.Analyze(Snapshot(one.Original),house,null,null).Count==0,"opt-in house protection");
            var ev=new BoundaryEvidence{LeftStart=0,Rule="CASE-002-OVERT",Tags=new Dictionary<string,string>{{"case_marker","accusative"}},Ambiguous=false};
            Check(engine.Analyze(Snapshot(one.Original),new HouseStyle(),null,new[]{ev}).All(f=>f.Suggested==null),"required annotations gate");
            ev.Tags["lexical_ai_flag"]="false";ev.Ambiguous=true;Check(engine.Analyze(Snapshot(one.Original),new HouseStyle(),null,new[]{ev}).All(f=>f.Suggested==null),"ambiguous annotations abstain");
            ev.Ambiguous=false;ev.Tags["lexical_ai_flag"]="true";Check(engine.Analyze(Snapshot(one.Original),new HouseStyle(),null,new[]{ev}).All(f=>f.Suggested==null),"contradictory lexical ai evidence blocks insertion");
            ev.Tags["lexical_ai_flag"]="false";Check(engine.Analyze(Snapshot(one.Original),new HouseStyle(),null,new[]{ev,ev}).All(f=>f.Suggested==null),"competing boundary analyses abstain");
            foreach(string replacement in new[]{"\\v 9 nope","a|lemma=x","a\nb","a\r","\u0BBEbad","\ud800",""}) Throws(()=>PatchService.Patch(one.Original,0,one.Original,replacement,"1"),"unsafe replacement rejected");
            var store=new MemoryStore{Text=one.Original};var journal=new MemoryJournal();var service=new PatchService(store,journal,"test-build");
            var applied=service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash);Check(store.Text==one.Suggested&&one.State==ReviewState.Applied,"safe apply");Check(journal.Rows.First().State=="pending"&&journal.Rows.Last().State=="applied","durable audit states");Check(!store.Locked,"lock released");
            Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"repeated apply blocked");
            service.Revert(applied,"human",one);Check(store.Text==one.Original,"guarded revert");
            Check(one.State==ReviewState.Reverted&&one.AppliedText==null,"revert refreshes finding outcome");
            Check((string)FindingExport.ToSchema(one)["status"]!="accepted","reverted export is not accepted");
            Check(ReviewHistory.Current(journal.Read()).Single(r=>r.Transaction==applied.Transaction).State=="reverted","history links revert to applied transaction");
            store.Text=applied.After+" later";Throws(()=>service.Revert(applied,"human"),"stale revert blocked");
            one=Scan("அவரை பார்த்தான்").Single();store.Text=one.Original;store.MutateOnLock=true;Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"concurrent edit under lock detected");Check(!store.Locked,"stale lock released");store.MutateOnLock=false;
            one=Scan("அவரை பார்த்தான்").Single();store.Text=one.Original;store.Editable=false;int writes=store.Writes;Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"permission denied");Check(store.Writes==writes,"no forbidden write");store.Editable=true;
            journal.Fail=true;Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"journal failure blocks write");Check(store.Writes==writes,"no write before durable intent");journal.Fail=false;
            store.FailWrite=true;Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"failed write propagated");Check(one.State==ReviewState.Uncertain&&!store.Locked,"uncertain outcome and unlock");store.FailWrite=false;
            one=Scan("அவரை பார்த்தான்").Single();service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash);Check(journal.Rows.Any(r=>r.State=="not-applied"),"interrupted transaction reconciliation");
            one=Scan("அவரை பார்த்தான்").Single();store.Text=one.Original;store.NormalizeOnWrite=true;Throws(()=>service.Apply(one,one.Suggested,"human",false,new HouseStyle().Hash),"save verification mismatch");Check(one.State==ReviewState.Uncertain,"mismatch never success");
            int fixtures=0,bTotal=0,bPredicted=0,bCorrect=0;foreach(string line in File.ReadAllLines(Path.Combine(root,"reference/tamil_punarchi_agent_package/tests.jsonl"),Encoding.UTF8))
            {
                var row=Json.Read<Dictionary<string,object>>(line);string input=row.ContainsKey("input_text")?(string)row["input_text"]:(string)row["left"]+" "+(string)row["right"];string hash=TextIdentity.Hash(input);var scan=Scan(input);fixtures++;
                Check(TextIdentity.Hash(input)==hash,"fixture immutable "+row["id"]);
                Check(scan.All(f=>catalog.rules.Any(r=>r.id==f.Rule)&&f.Reason.StartsWith("காரணம்:")),"fixture finding contract "+row["id"]);
                if(((string)row["id"]).StartsWith("A")) Check(scan.All(f=>f.Suggested==null),"A fixture protection "+row["id"]);
                if(((string)row["id"]).StartsWith("C")) Check(scan.All(f=>f.Suggested==null),"C compound never force joined "+row["id"]);
                if(((string)row["id"]).StartsWith("B")) {bTotal++;if(scan.Any(f=>f.Suggested!=null)){bPredicted++;if(scan.Count==1&&scan[0].Suggested==(string)row["expected_text_if_approved"])bCorrect++;else throw new Exception("Incorrect supported B prediction: "+row["id"]);}}
            }
            Check(fixtures==99,"all fixtures consumed");
            string journalPath=Path.Combine(root,"artifacts/build/synthetic-journal.jsonl");File.WriteAllText(journalPath,"",Encoding.UTF8);var durable=new FileJournal(journalPath);durable.Append(applied);Check(durable.Read().Count==1,"file journal reload");File.AppendAllText(journalPath,"{truncated");Throws(()=>durable.Read(),"damaged journal fails closed");
            File.WriteAllText(journalPath,Json.Write(new AuditRecord{SchemaVersion="999"}),Encoding.UTF8);Throws(()=>durable.Read(),"unknown audit schema fails closed");
            foreach(string invalid in new[]{"{}","{\"SchemaVersion\":\"1\",\"Transaction\":\"incomplete\",\"State\":\"pending\"}",Json.Write(new AuditRecord{SchemaVersion="1",State="nonsense"})})
            {
                File.WriteAllText(journalPath,invalid,Encoding.UTF8);Throws(()=>durable.Read(),"semantically corrupt journal refused");
                var blockedStore=new MemoryStore{Text="அவரை பார்த்தான்"};var blockedFinding=Scan(blockedStore.Text).Single();
                Throws(()=>new PatchService(blockedStore,durable,"test").Apply(blockedFinding,blockedFinding.Suggested,"test",false,new HouseStyle().Hash),"corrupt journal blocks new transaction");Check(blockedStore.Writes==0&&!blockedStore.Locked,"corrupt journal causes no write and releases lock");
            }
            string settingsPath=Path.Combine(root,"artifacts/build/settings-regression-"+Guid.NewGuid().ToString("N")+".json");
            var settingsA=new HouseStyleStore(settingsPath);var settingsB=new HouseStyleStore(settingsPath);var oldSettings=settingsA.Load();
            var settingStore=new MemoryStore{Text="அவரை பார்த்தான்"};var settingFinding=Scan(settingStore.Text).Single();
            var newSettings=new HouseStyle{Version="changed",NoInsertBefore=new List<string>{"பார்த்தான்"}};
            settingsB.Save(newSettings);Check(settingsA.Load().Hash==newSettings.Hash,"other window reads current shared settings");
            Throws(()=>settingsA.WithCurrent(current=>new PatchService(settingStore,new MemoryJournal(),"test",()=>settingsA.Load().Hash).Apply(settingFinding,settingFinding.Suggested,"test",false,current.Hash)),"old pane cannot bypass updated settings");Check(settingStore.Writes==0,"updated settings prevent write");
            settingsB.Save(oldSettings);settingStore.OnLock=()=>settingsB.Save(newSettings);
            Throws(()=>settingsA.WithCurrent(current=>new PatchService(settingStore,new MemoryJournal(),"test",()=>settingsA.Load().Hash).Apply(settingFinding,settingFinding.Suggested,"test",false,current.Hash)),"settings revalidated under chapter lock");Check(settingStore.Writes==0&&!settingStore.Locked,"configuration race releases lock without write");
            var prepared=new[]{ScopePreparation.Read("test",1,1,()=>"\\v 1 அவரை பார்த்தான்"),ScopePreparation.Read("test",1,2,()=>{throw new IOException("Synthetic unreadable chapter");}),ScopePreparation.Read("test",1,3,()=>"\\v 1 இந்த புத்தகம்")};
            Check(prepared.Count(p=>p.Snapshot!=null)==2&&prepared.Count(p=>p.Error!=null)==1,"unreadable chapter does not discard later chapters");
            string longChapter="\\c 1\n"+String.Join("\n",Enumerable.Range(1,176).Select(v=>"\\v "+v+" அவரை பார்த்தான்."));var watch=System.Diagnostics.Stopwatch.StartNew();var measured=Scan(longChapter);watch.Stop();Check(measured.Count==176,"176-verse synthetic scan");
            var report=new{assertions=count,fixturesConsumed=fixtures,conditionalSuggestionScenarios=bTotal,predictedWithoutExternalAnnotations=bPredicted,matchingExpectedSuggestions=bCorrect,notImplementedOrAbstained=bTotal-bPredicted,independentLinguisticEvaluation=false,synthetic176VerseScanMilliseconds=watch.ElapsedMilliseconds};
            File.WriteAllText(Path.Combine(root,"artifacts/build/test-results.json"),Json.Write(report),Encoding.UTF8);
            Console.WriteLine(Json.Write(report));
            Console.WriteLine("PASS: "+count+" assertions; 99 fixtures consumed. Linguistic coverage is partial; unresolved cases are not counted as predictions.");
            return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TamilSandhi
{
    public sealed class SandhiEngine
    {
        private readonly Catalog catalog;
        private readonly Dictionary<string,RuleRecord> rules;
        private static readonly Regex Words=new Regex(@"[\u0B80-\u0BFF]+",RegexOptions.Compiled);
        private static readonly string[] HardEnds={"க்","ச்","த்","ப்"};
        public SandhiEngine(Catalog data){catalog=data;catalog.Validate();rules=catalog.rules.ToDictionary(r=>r.id);}
        public List<Finding> Analyze(ChapterSnapshot snapshot,HouseStyle style,IList<TextSpan> scope,IList<BoundaryEvidence> annotations)
        {return AnalyzeDetailed(snapshot,style,scope,annotations).Findings;}

        public AnalysisReport AnalyzeDetailed(ChapterSnapshot snapshot,HouseStyle style,IList<TextSpan> scope,IList<BoundaryEvidence> annotations)
        {
            var report=new AnalysisReport();
            foreach(var segment in Usfm.Segments(snapshot.Text))
            {
                if(scope!=null&&!scope.Any(s=>segment.Start<s.End&&segment.End>s.Start))continue;
                var words=Words.Matches(snapshot.Text.Substring(segment.Start,segment.Length)).Cast<Match>().ToArray();
                for(int i=0;i+1<words.Length;i++)
                {
                    var left=words[i];var right=words[i+1];
                    int start=segment.Start+left.Index,end=segment.Start+right.Index+right.Length;
                    string gap=snapshot.Text.Substring(segment.Start+left.Index+left.Length,right.Index-left.Index-left.Length);
                    if(gap.Length==0||gap.Any(c=>c!=' '&&c!='\t'&&c!='\r'&&c!='\n'))continue;
                    if(scope!=null&&!scope.Any(s=>start>=s.Start&&end<=s.End))continue;
                    report.ExaminedBoundaries++;
                    string a=left.Value.Normalize(),b=right.Value.Normalize();
                    var matching=annotations==null?new BoundaryEvidence[0]:annotations.Where(e=>e.LeftStart==start).ToArray();
                    if(style.NoInsertBefore.Any(w=>w.Normalize()==b)||matching.Any(e=>e.Protected))
                    {report.SuppressedBoundaries++;continue;}
                    if(!"கசதப".Contains(b[0])){report.UnassessedBoundaries++;continue;}
                    string existing=HardEnds.FirstOrDefault(h=>a.EndsWith(h,StringComparison.Ordinal));
                    string stem=existing==null?a:a.Substring(0,a.Length-2);
                    string rawStem=existing==null?left.Value:left.Value.Substring(0,left.Value.Length-2);
                    string hard=b.Substring(0,1)+"்";
                    // Homographs cannot be resolved by the two surface words.
                    bool lexicalAmbiguity=stem=="அவரை";
                    string rule=ScriptureRuleProfile.Classify(stem,b);
                    bool namingClause=Regex.IsMatch(snapshot.Text.Substring(end,segment.End-end),@"^\s+என்று\s+(?:பேரி|பெயரி|நாமமி)");
                    bool editorialNameContext=rule!=null&&(ScriptureRuleProfile.DeityNames.Contains(b)||b.StartsWith("தேவ",StringComparison.Ordinal)||b.StartsWith("தெய்வ",StringComparison.Ordinal)||ScriptureRuleProfile.EditorialLexemes.Contains(b)||namingClause);
                    bool conflict=matching.Length>1||matching.Any(e=>e.Ambiguous);
                    var tags=rule==null?new Dictionary<string,string>():ScriptureRuleProfile.Tags(rule);
                    if(matching.Length==1)
                    {
                        var supplied=matching[0];
                        // Supplied tags can restrict an analysis, never bypass lexical/register gates.
                        if(supplied.Rule!=rule||supplied.Tags==null||tags.Any(t=>!supplied.Tags.ContainsKey(t.Key)||supplied.Tags[t.Key]!=t.Value))conflict=true;
                    }
                    RuleRecord record=rule==null?null:rules[rule];
                    bool supported=ScriptureRuleProfile.Allows(record)&&record.requires_annotations!=null&&record.requires_annotations.All(tags.ContainsKey)&&!lexicalAmbiguity&&!conflict&&!editorialNameContext;
                    bool lexicalNoInsert=(stem=="அறிக்கை"||stem=="கொலை")&&existing==null;
                    if(lexicalNoInsert&&!conflict){report.UnassessedBoundaries++;continue;}
                    // A matching written ottu without a licensed analysis is unassessed, not an error.
                    // Retain explicit conflicts for review, but do not flood reviewed Scripture with no-change findings.
                    if(!supported&&existing==hard&&!conflict){report.UnassessedBoundaries++;continue;}
                    bool candidate=rule!=null||lexicalAmbiguity||conflict||stem.EndsWith("ை",StringComparison.Ordinal)||stem.EndsWith("க்கு",StringComparison.Ordinal)||existing!=null;
                    if(!supported&&!candidate){report.UnassessedBoundaries++;continue;}
                    string selectedRule=supported?rule:"SAFE-010";
                    string replacement=null,operation="review";
                    if(supported)
                    {
                        report.SupportedBoundaries++;
                        string desired=record.decision=="NO_INSERT"?"":hard;
                        if((existing??"")==desired)continue;
                        replacement=rawStem+desired+gap+right.Value;
                        operation=desired.Length==0?"remove":existing==null?"insert":"replace";
                    }
                    else report.UnresolvedBoundaries++;
                    record=rules[selectedRule];
                    string original=snapshot.Text.Substring(start,end-start);
                    string[] blockers=supported?new string[0]:new[]{lexicalAmbiguity?"lexical_homograph_unresolved":namingClause?"naming_clause_boundary":editorialNameContext?"editorial_name_context":conflict?"boundary_evidence_conflict":"unsupported_context"};
                    report.Findings.Add(new Finding{
                        Id=TextIdentity.Hash(snapshot.Project+"|"+snapshot.Book+"|"+snapshot.Chapter+"|"+snapshot.Revision+"|"+start+"|"+selectedRule+"|"+catalog.catalog_version+"|"+style.Hash+"|"+ScriptureRuleProfile.Version),
                        Project=snapshot.Project,Book=snapshot.Book,Chapter=snapshot.Chapter,Verse=segment.Verse,Start=start,Length=end-start,
                        StartUtf8=TextIdentity.Utf8Offset(snapshot.Text,start),EndUtf8=TextIdentity.Utf8Offset(snapshot.Text,end),ChapterHash=snapshot.Revision,
                        Original=original,Suggested=replacement,Context=snapshot.Text.Substring(segment.Start,segment.Length).Trim(),Rule=selectedRule,
                        Reason=supported?"காரணம்: வரையறுக்கப்பட்ட நவீனத் தமிழ் சொல்-சூழல் விதி ("+rule+"). ‘"+a+"’ / ‘"+b+"’: "+(operation=="remove"?"தேவையற்ற ஒற்றை நீக்கும்":operation=="replace"?"ஒற்றை மாற்றும்":"ஒற்றைச் சேர்க்கும்")+" பரிந்துரை. இது முழுமையான வாக்கியப் பொருள் ஆய்வு அல்ல; வசனச் சூழலுடன் சரிபார்த்து ஒப்புதல் அளிக்கவும்.":lexicalAmbiguity?"காரணம்: ‘அவரை’ என்பது பெயர்ச்சொல்லாகவும் சுட்டுப்பெயரின் வேற்றுமை வடிவமாகவும் வரலாம். வசனப் பொருள் தெளிவாகாததால் திருத்தம் பரிந்துரைக்கப்படவில்லை; மனித ஆய்வு தேவை.":editorialNameContext?"காரணம்: இச்சூழலில் பெயரிடும் தொடர், இறைப்பெயர் அல்லது பதிப்பு சார்ந்த சொல் வருகிறது. வசனப் பொருளும் திட்ட எழுத்துநடையும் மாறுபடலாம்; திருத்தம் பரிந்துரைக்கப்படாமல் மனித ஆய்வுக்கு விடப்பட்டுள்ளது.":"காரணம்: இச்சொல்-சூழலுக்குப் போதுமான அல்லது முரண்பாடற்ற இலக்கண ஆதாரம் இல்லை. மனித ஆய்வு தேவை; திருத்தம் பரிந்துரைக்கப்படவில்லை.",
                        Evidence=Json.Write(new{profile=ScriptureRuleProfile.Version,analysis="bounded lexical context; sentence semantics unverified",operation=operation,tags=tags,editorialPolicy="exact following-word suppression; not a universal names rule"}),
                        Operation=operation,Blockers=blockers,CatalogVersion=catalog.catalog_version,ConfigHash=style.Hash,
                        Sources=record.source_ids.Select(s=>catalog.sources[s]).ToArray(),State=supported?ReviewState.Open:ReviewState.NeedsReview
                    });
                }
            }
            report.Findings=report.Findings.GroupBy(f=>f.Id).Select(g=>g.First()).ToList();
            return report;
        }
    }
}

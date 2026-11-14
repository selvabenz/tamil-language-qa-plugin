using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TamilSandhi
{
    public sealed class SandhiEngine
    {
        private readonly Catalog catalog;
        private static readonly Regex Words = new Regex(@"[\u0B80-\u0BFF]+", RegexOptions.Compiled);
        private static readonly HashSet<string> Accusatives = Set("அவரை அவனை அவளை என்னை உன்னை அறிக்கையை கதையை கொலையை புத்தகத்தை");
        private static readonly HashSet<string> Datives = Set("எனக்கு உனக்கு அவனுக்கு அவளுக்கு அவருக்கு கர்த்தருக்கு");
        private static readonly HashSet<string> HardParticiples = Set("படித்து கொடுத்து பார்த்து காத்து");
        private static readonly HashSet<string> SoftParticiples = Set("வந்து கண்டு சென்று செய்து");
        private static readonly HashSet<string> Verbs = Set("பார்த்தான் பார்த்தார் பார்த்தார்கள் பார் படித்தான் படித்தார் படி சமர்ப்பித்தான் சமர்ப்பித்தார் கொடுத்தான் கொடுத்தார் கொடு சொன்னான் சொன்னார் சொல் தந்தான் தந்தார் செய்தான் செய்தார் காப்பார் சென்றான் சென்றார் பேசினார்");
        private static readonly HashSet<string> Nouns = Set("புத்தகம் கதை செய்தி பையன் குழந்தை பெண் பொருள் குதிரை பள்ளி கடை தோட்டம்");
        private static HashSet<string> Set(string words) { return new HashSet<string>(words.Split(' ')); }
        public SandhiEngine(Catalog data) { catalog=data; catalog.Validate(); }

        public List<Finding> Analyze(ChapterSnapshot snapshot, HouseStyle style, IList<TextSpan> scope, IList<BoundaryEvidence> annotations)
        {
            var output = new List<Finding>();
            foreach (var segment in Usfm.Segments(snapshot.Text))
            {
                if(scope!=null&&!scope.Any(s=>segment.Start<s.End&&segment.End>s.Start))continue;
                var words = Words.Matches(snapshot.Text.Substring(segment.Start, segment.Length)).Cast<Match>().ToArray();
                for (int i=0; i+1 < words.Length; i++)
                {
                    var left=words[i]; var right=words[i+1];
                    int start=segment.Start+left.Index, end=segment.Start+right.Index+right.Length;
                    string gap=snapshot.Text.Substring(segment.Start+left.Index+left.Length,right.Index-left.Index-left.Length);
                    if (gap.Length==0 || gap.Any(c => c!=' ' && c!='\t' && c!='\r' && c!='\n')) continue;
                    if (scope != null && !scope.Any(s => start>=s.Start && end<=s.End)) continue;
                    string a=left.Value.Normalize(), b=right.Value.Normalize();
                    if (style.NoInsertBefore.Contains(b) || SoftParticiples.Contains(a) || a=="அறிக்கை" || a=="கொலை") continue;
                    if (!"கசதப".Contains(b[0])) continue;
                    string hard=b.Substring(0,1)+"்";
                    if (a.EndsWith(hard, StringComparison.Ordinal)) continue;
                    string rule=null; var tags=new Dictionary<string,string>();
                    var matching=annotations==null?new BoundaryEvidence[0]:annotations.Where(e=>e.LeftStart==start).ToArray();
                    BoundaryEvidence supplied=matching.FirstOrDefault();
                    bool ambiguous=matching.Length>1 || (supplied != null && supplied.Ambiguous);
                    if (matching.Any(e=>e.Protected)) continue;
                    if (supplied != null) { rule=supplied.Rule; tags=supplied.Tags ?? tags; }
                    else if ((a=="இந்த" || a=="அந்த" || a=="எந்த") && Nouns.Contains(b))
                    { rule="HARD-DEM"; tags["syntactic_relation"]="demonstrative_adjective+noun"; tags["compound_type"]="free_noun_phrase"; }
                    else if (Accusatives.Contains(a) && Verbs.Contains(b))
                    { rule="CASE-002-OVERT"; tags["case_marker"]="accusative"; tags["lexical_ai_flag"]="false"; }
                    else if (Datives.Contains(a) && Verbs.Contains(b))
                    { rule="CASE-004-OVERT"; tags["case_marker"]="dative"; }
                    else if (HardParticiples.Contains(a) && Verbs.Contains(b))
                    { rule="HARD-PART-HARDU"; tags["syntactic_relation"]="vanthodar_participle+verb"; tags["compound_type"]="verbal_phrase"; }
                    // Suffixes only nominate an unresolved boundary; they never license a correction.
                    bool candidate = rule!=null || a.EndsWith("ை",StringComparison.Ordinal) || a.EndsWith("க்கு",StringComparison.Ordinal) || HardParticiples.Contains(a);
                    if (!candidate) continue;
                    RuleRecord record=rule == null ? null : catalog.rules.FirstOrDefault(r => r.id==rule);
                    bool supported=record != null && record.decision=="INSERT_HARD" && record.requires_annotations.All(t => tags.ContainsKey(t)) && TagsLicense(record.id,tags) && !ambiguous;
                    string selectedRule=supported ? record.id : "SAFE-010";
                    record=catalog.rules.First(r => r.id==selectedRule);
                    string original=snapshot.Text.Substring(start,end-start);
                    string replacement=supported ? left.Value+hard+gap+right.Value : null;
                    string config=style.Hash;
                    output.Add(new Finding {
                        Id=TextIdentity.Hash(snapshot.Project+"|"+snapshot.Book+"|"+snapshot.Chapter+"|"+snapshot.Revision+"|"+start+"|"+selectedRule+"|"+catalog.catalog_version+"|"+config),
                        Project=snapshot.Project,Book=snapshot.Book,Chapter=snapshot.Chapter,Verse=segment.Verse,Start=start,Length=end-start,
                        StartUtf8=TextIdentity.Utf8Offset(snapshot.Text,start),EndUtf8=TextIdentity.Utf8Offset(snapshot.Text,end),ChapterHash=snapshot.Revision,
                        Original=original,Suggested=replacement,Context=snapshot.Text.Substring(segment.Start,segment.Length).Trim(),Rule=selectedRule,
                        Reason=supported ? "காரணம்: ‘"+a+"’ மற்றும் ‘"+b+"’ இடையிலான இலக்கண உறவு உள்ளூர் சொல்லாய்வில் அடையாளம் காணப்பட்டது ("+String.Join(", ",tags.Select(t=>t.Key+"="+t.Value))+ "). ‘"+hard+"’ சேர்க்கும் பரிந்துரையைச் சூழலுடன் சரிபார்த்து ஒப்புதல் அளிக்கவும்." : "காரணம்: இந்தச் சொற்களின் இலக்கண உறவை உறுதிப்படுத்தப் போதுமான சொல்லாய்வு இல்லை. மனித ஆய்வு தேவை; தானாகத் திருத்தம் பரிந்துரைக்கப்படவில்லை.",
                        Evidence=supplied == null ? "Built-in limited lexical analysis: "+Json.Write(tags) : "Supplied boundary annotations: "+Json.Write(tags),
                        CatalogVersion=catalog.catalog_version,ConfigHash=config,Sources=record.source_ids.Select(s=>catalog.sources[s]).ToArray(),State=supported?ReviewState.Open:ReviewState.NeedsReview
                    });
                }
            }
            return output.GroupBy(f=>f.Id).Select(g=>g.First()).ToList();
        }
        private static bool TagsLicense(string rule,Dictionary<string,string> tags)
        {
            Func<string,string,bool> has=(key,value)=>tags.ContainsKey(key)&&tags[key]==value;
            if(rule=="CASE-002-OVERT")return has("case_marker","accusative")&&has("lexical_ai_flag","false");
            if(rule=="CASE-004-OVERT")return has("case_marker","dative");
            if(rule=="HARD-DEM")return has("syntactic_relation","demonstrative_adjective+noun")&&has("compound_type","free_noun_phrase");
            if(rule=="HARD-PART-HARDU")return has("syntactic_relation","vanthodar_participle+verb")&&has("compound_type","verbal_phrase");
            return false;
        }
    }
}

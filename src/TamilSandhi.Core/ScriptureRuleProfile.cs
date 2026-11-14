using System;
using System.Collections.Generic;
using System.Linq;

namespace TamilSandhi
{
    // Explicitly reviewed modern-prose subset. This is not a morphology parser.
    public static class ScriptureRuleProfile
    {
        public const string Version="scripture-modern-2";
        internal static HashSet<string> Words(string text){return new HashSet<string>(text.Split(' '),StringComparer.Ordinal);}
        internal static readonly HashSet<string> Accusatives=Words("அவனை அவளை என்னை உன்னை அறிக்கையை கதையை கொலையை புத்தகத்தை மலரை கதவை வார்த்தையை வசனத்தை சத்தியத்தை கட்டளையை தேவனை கர்த்தரை ஜனங்களை அதை இதை எதை அவர்களை இவர்களை அவைகளை இவைகளை உம்மை உங்களை எங்களை தம்மை தன்னை இவனை நாமத்தை குமாரனை ஆத்துமாவை சத்தத்தை இயேசுவை மனுஷனை முகத்தை கண்களை வார்த்தைகளை வஸ்திரங்களை மகிமையை வழியை பூமியை கால்களை கனிகளை கையை இருதயத்தை விண்ணப்பத்தை சத்துருக்களை நீதியை கற்பனைகளை ஜீவனை கட்டளைகளை பலனை வாயை மரத்தை");
        internal static readonly HashSet<string> Datives=Words("எனக்கு உனக்கு அவனுக்கு அவளுக்கு அவருக்கு கர்த்தருக்கு ஊருக்கு தேவனுக்கு உங்களுக்கு எங்களுக்கு அவர்களுக்கு சீஷர்களுக்கு ஜனங்களுக்கு உமக்கு அதற்கு இதற்கு எதற்கு தனக்கு தமக்கு நமக்கு தங்களுக்கு அவைகளுக்கு இவைகளுக்கு இவனுக்கு இவருக்கு வீட்டுக்கு யாக்கோபுக்கு ஆபிரகாமுக்கு எகிப்துக்கு");
        internal static readonly HashSet<string> DeityNames=Words("தேவன் தேவனே கர்த்தர் கர்த்தரே யெகோவா இயேசு கிறிஸ்து");
        // Supplied editorial-sensitive lexeme: review policy, never a universal grammar exception.
        internal static readonly HashSet<string> EditorialLexemes=Words("தேவசாயலாக");
        internal static readonly HashSet<string> Verbs=Words("பார்த்தான் பார்த்தார் பார்த்தார்கள் பார் படித்தான் படித்தார் படி சமர்ப்பித்தான் சமர்ப்பித்தார் கொடுத்தான் கொடுத்தார் கொடுத்தார்கள் கொடு சொன்னான் சொன்னார் சொன்னார்கள் சொல்வான் சொல் தந்தான் தந்தார் தந்தார்கள் செய்தான் செய்தார் செய்தார்கள் காப்பார் சென்றான் சென்றார் சென்றார்கள் பேசினார் பேசு பேசினான் பேசுவான் போனான் போனார் போனார்கள் பறித்தான் தட்டு கேட்டான் கேட்டார் கேள் காத்தார் காப்பாற்றினார் கற்றார் கற்றுக்கொடுத்தார்");
        internal static readonly HashSet<string> Nouns=Words("புத்தகம் புத்தகத்தை கதை செய்தி பையன் குழந்தை குழந்தைகள் பெண் பொருள் பொருளை குதிரை பள்ளி கடை தோட்டம் காசு சிக்கல் வார்த்தை வார்த்தையை வசனம் வசனத்தை சத்தியம் சத்தியத்தை கட்டளை கட்டளையை சீஷன் சீஷர்கள் ஜனங்கள் தேசம் கர்த்தர் தேவன் கேள்வி கேள்வியை கண்கள் பாதை பாட்டு கை");
        internal static readonly HashSet<string> HardParticiples=Words("படித்து கொடுத்து பார்த்து காத்து கற்று வைத்து எடுத்து கேட்டு வாய்விட்டு விட்டு");
        internal static readonly HashSet<string> SoftParticiples=Words("வந்து கண்டு சென்று கொண்டு தின்று கொன்று");
        internal static readonly HashSet<string> MidParticiples=Words("செய்து கொய்து");
        internal static readonly HashSet<string> Relatives=Words("வந்த கேட்ட ஓடிய பாடிய கொடுத்த கூடிய சொன்ன செய்த கண்ட சென்ற வரும்");
        internal static readonly HashSet<string> NegativeRelatives=Words("செல்லாத காணாத ஓடாத தீராத செய்யாத");
        internal static readonly HashSet<string> Adjectives=Words("நல்ல பெரிய புதிய சிறிய கரிய அரிய");
        internal static readonly HashSet<string> Infinitives=Words("வர உண்ண உட்கார செய்ய சொல்ல கேட்க பார்க்க கொடுக்க");
        internal static readonly HashSet<string> IParticiples=Words("ஓடி தேடி கூறி கூடி நாடி நோக்கி பேசி வேண்டி சொல்லி எண்ணி");
        internal static readonly HashSet<string> YParticiples=Words("போய்");
        internal static readonly HashSet<string> AdverbialParticiples=Words("நன்றாக வேகமாக வருவதாய் என விரோதமாய் முன்பாக அதிகமாய் விரோதமாக ஏகமாய்");
        internal static readonly HashSet<string> ClippedNegatives=Words("ஓடா காணா செல்லா தீரா");
        private static readonly Dictionary<string,string> Modes=new Dictionary<string,string>{
            {"HARD-DEM","insert"},{"CASE-002-OVERT","insert"},{"CASE-004-OVERT","insert"},
            {"HARD-LOCADV","insert"},{"HARD-MANNER","insert"},{"HARD-INFINITIVE","insert"},
            {"HARD-PART-I","insert"},{"HARD-PART-Y","insert"},{"HARD-PART-HARDU","insert"},
            {"HARD-PART-AG","insert"},{"HARD-NEG-ELLIP","insert"},
            {"NO-REL-POS","remove"},{"NO-REL-NEG","remove"},{"NO-ADJECTIVE","remove"},
            {"NO-PART-SOFTU","remove"},{"NO-PART-MIDU","remove"}
        };
        // The reference's review tier is promoted ONLY for these bounded lexical contexts.
        private static readonly HashSet<string> ReviewedTierPromotions=Words("HARD-LOCADV HARD-PART-I HARD-PART-Y HARD-PART-HARDU HARD-PART-AG HARD-NEG-ELLIP");
        public static bool Allows(RuleRecord rule)
        {
            string mode;
            return rule!=null && Modes.TryGetValue(rule.id,out mode) && rule.layer=="modern" &&
                (mode=="insert" ? rule.decision=="INSERT_HARD" && (rule.tier=="candidate" || rule.tier=="review"&&ReviewedTierPromotions.Contains(rule.id)) : rule.decision=="NO_INSERT"&&rule.tier=="block");
        }
        public static Dictionary<string,string> Capabilities(Catalog catalog)
        {
            return catalog.rules.ToDictionary(r=>r.id,r=>Allows(r)?Modes[r.id]+"/replace in bounded lexical contexts":r.id=="NO-NAMES"?"editorial policy; not a universal grammar prohibition":"not executable in this profile");
        }
        internal static string Classify(string a,string b)
        {
            bool verb=Verbs.Contains(b),noun=Nouns.Contains(b);
            if((a=="இந்த"||a=="அந்த"||a=="எந்த")&&!verb)return "HARD-DEM";
            if(Accusatives.Contains(a))return "CASE-002-OVERT";
            if(Datives.Contains(a))return "CASE-004-OVERT";
            if(HardParticiples.Contains(a))return "HARD-PART-HARDU";
            if(a=="அங்கு"||a=="இங்கு"||a=="எங்கு")return "HARD-LOCADV";
            if(a=="அப்படி"||a=="இப்படி"||a=="எப்படி")return "HARD-MANNER";
            if(Infinitives.Contains(a)&&verb)return "HARD-INFINITIVE";
            if(IParticiples.Contains(a))return "HARD-PART-I";
            if(YParticiples.Contains(a))return "HARD-PART-Y";
            if(AdverbialParticiples.Contains(a)&&(a!="என"||verb))return "HARD-PART-AG";
            if(ClippedNegatives.Contains(a)&&noun)return "HARD-NEG-ELLIP";
            if(Relatives.Contains(a)&&noun)return "NO-REL-POS";
            if(NegativeRelatives.Contains(a)&&noun)return "NO-REL-NEG";
            if(Adjectives.Contains(a)&&noun)return "NO-ADJECTIVE";
            if(SoftParticiples.Contains(a)&&verb)return "NO-PART-SOFTU";
            if(MidParticiples.Contains(a)&&verb)return "NO-PART-MIDU";
            return null;
        }
        internal static Dictionary<string,string> Tags(string rule)
        {
            if(rule=="CASE-002-OVERT")return new Dictionary<string,string>{{"case_marker","accusative"},{"lexical_ai_flag","false"}};
            if(rule=="CASE-004-OVERT")return new Dictionary<string,string>{{"case_marker","dative"}};
            return new Dictionary<string,string>{{"syntactic_relation",rule=="HARD-DEM"?"demonstrative+following_Tamil_word":rule=="HARD-PART-HARDU"?"vanthodar_participle_boundary":rule},{"compound_type","bounded_lexical_context"}};
        }
    }
}

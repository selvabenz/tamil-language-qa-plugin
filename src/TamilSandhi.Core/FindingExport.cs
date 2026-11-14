using System;
using System.Collections.Generic;
using System.Linq;

namespace TamilSandhi
{
    public static class FindingExport
    {
        public static Dictionary<string,object> ToSchema(Finding f)
        {
            bool actionable=f.Actionable;
            string status=f.State==ReviewState.Applied?"accepted":f.State==ReviewState.Rejected?"rejected":actionable?"verified_suggestion":f.State==ReviewState.NeedsReview||f.State==ReviewState.Stale||f.State==ReviewState.Uncertain?"needs_review":"open";
            return new Dictionary<string,object>{
                {"finding_id",f.Id},{"document_hash",f.ChapterHash},{"rule_ids",new[]{f.Rule}},{"layer",f.Rule.StartsWith("SAFE-",StringComparison.Ordinal)?"engineering":"modern"},
                {"status",status},{"confidence",0.0},{"context",f.Context},{"reference",f.Reference},
                {"source_span",new Dictionary<string,object>{{"start_utf8_byte",f.StartUtf8},{"end_utf8_byte",f.EndUtf8}}},
                {"original_text",f.Original},{"suggested_text",f.AppliedText??f.Suggested},{"reason_ta",f.Reason},{"protected",false},{"source_urls",f.Sources},
                {"evidence",new Dictionary<string,object>{{"morphology",null},{"syntax",null},{"lexicon",new Dictionary<string,object>{{"analysis",f.Evidence}}},{"house_style",new Dictionary<string,object>{{"config_hash",f.ConfigHash}}},{"blockers",new[]{"confidence_not_calibrated","sentence_semantics_not_verified"}.Concat(f.Blockers??new string[0]).ToArray()},{"local_review_state",f.State.ToString()},{"operation",f.Operation},{"rule_profile",ScriptureRuleProfile.Version}}},
                {"suggested_patch",actionable?(object)new Dictionary<string,object>{{"from_utf8_byte",f.StartUtf8},{"to_utf8_byte",f.EndUtf8},{"replacement",f.Suggested},{"expected_original_bytes_sha256",TextIdentity.Hash(f.Original)}}:null},
                {"review",new Dictionary<string,object>{{"decision",f.State==ReviewState.Applied?"approved":f.State==ReviewState.Rejected?"rejected":"open"},{"reviewer_id",null},{"reason",null}}}
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace TamilSandhi
{
    public static class Json
    {
        public static string Write(object value) { return new JavaScriptSerializer { MaxJsonLength = 16777216 }.Serialize(value); }
        public static T Read<T>(string text) { return new JavaScriptSerializer { MaxJsonLength = 16777216 }.Deserialize<T>(text); }
    }

    public static class TextIdentity
    {
        public static string Hash(string value)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(new UTF8Encoding(false, true).GetBytes(value))).Replace("-", "").ToLowerInvariant();
        }
        public static bool Boundary(string text, int offset)
        {
            if (offset == text.Length) return true;
            return offset >= 0 && StringInfo.ParseCombiningCharacters(text).Contains(offset);
        }
        public static int Utf8Offset(string text, int offset) { return new UTF8Encoding(false, true).GetByteCount(text.Substring(0, offset)); }
    }

    public class TextSpan
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string Verse { get; set; }
        public int End { get { return Start + Length; } }
    }
    public class ChapterSnapshot
    {
        public string Project { get; set; }
        public int Book { get; set; }
        public int Chapter { get; set; }
        public string Text { get; set; }
        public string Revision { get { return TextIdentity.Hash(Text); } }
    }
    public enum ReviewState { Open, NeedsReview, Rejected, Ignored, Applied, Stale, Failed, Uncertain, Reverted }
    public class Finding
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public int Book { get; set; }
        public int Chapter { get; set; }
        public string Verse { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public int StartUtf8 { get; set; }
        public int EndUtf8 { get; set; }
        public string ChapterHash { get; set; }
        public string Original { get; set; }
        public string Suggested { get; set; }
        public string Context { get; set; }
        public string Rule { get; set; }
        public string Reason { get; set; }
        public string Evidence { get; set; }
        public string CatalogVersion { get; set; }
        public string ConfigHash { get; set; }
        public string[] Sources { get; set; }
        public ReviewState State { get; set; }
        public bool Actionable { get { return Suggested != null && State == ReviewState.Open; } }
        public string Reference { get { return Book + " " + Chapter + ":" + Verse; } }
    }
    public class RuleRecord
    {
        public string id { get; set; }
        public string decision { get; set; }
        public string layer { get; set; }
        public string[] requires_annotations { get; set; }
        public string[] source_ids { get; set; }
    }
    public class Catalog
    {
        public string catalog_version { get; set; }
        public int rule_count { get; set; }
        public Dictionary<string,string> sources { get; set; }
        public List<RuleRecord> rules { get; set; }
        public void Validate()
        {
            if (rules == null || sources == null || rules.Count != rule_count || rules.Select(r => r.id).Distinct().Count() != rule_count)
                throw new InvalidOperationException("Invalid rule catalog count or duplicate IDs.");
            foreach (var r in rules)
                if (String.IsNullOrEmpty(r.id) || r.source_ids == null || r.source_ids.Any(s => !sources.ContainsKey(s)))
                    throw new InvalidOperationException("Invalid catalog source mapping.");
        }
    }
    public class HouseStyle
    {
        public string Version { get; set; }
        public List<string> NoInsertBefore { get; set; }
        public HouseStyle() { Version = "1"; NoInsertBefore = new List<string>(); }
        public string Hash { get { return TextIdentity.Hash(Version + "\n" + String.Join("\n", NoInsertBefore.OrderBy(s => s, StringComparer.Ordinal))); } }
    }
    public class BoundaryEvidence
    {
        public int LeftStart { get; set; }
        public string Rule { get; set; }
        public Dictionary<string,string> Tags { get; set; }
        public bool Protected { get; set; }
        public bool Ambiguous { get; set; }
    }
    public interface IChapterStore
    {
        string ProjectId { get; }
        string Read(int book, int chapter);
        bool CanEdit(int book, int chapter);
        IDisposable Lock(int book, int chapter);
        void Write(int book, int chapter, string text);
    }
    public class AuditRecord
    {
        public string Transaction { get; set; }
        public string FindingId { get; set; }
        public string Project { get; set; }
        public int Book { get; set; }
        public int Chapter { get; set; }
        public string Verse { get; set; }
        public int Start { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string BeforeHash { get; set; }
        public string AfterHash { get; set; }
        public string State { get; set; }
        public string Reviewer { get; set; }
        public string Action { get; set; }
        public string Build { get; set; }
        public string Rule { get; set; }
        public string Catalog { get; set; }
        public string Config { get; set; }
        public string Evidence { get; set; }
        public string Timestamp { get; set; }
    }
    public interface IJournal
    {
        void Append(AuditRecord record);
        IList<AuditRecord> Read();
    }
}

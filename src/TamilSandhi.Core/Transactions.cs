using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TamilSandhi
{
    public sealed class FileJournal : IJournal
    {
        private readonly string path;
        public FileJournal(string filename) { path=filename; Directory.CreateDirectory(Path.GetDirectoryName(path)); }
        public void Append(AuditRecord record)
        {
            AuditValidation.Validate(record);
            using (var stream=new FileStream(path,FileMode.Append,FileAccess.Write,FileShare.Read))
            using (var writer=new StreamWriter(stream,new UTF8Encoding(false))) { writer.WriteLine(Json.Write(record)); writer.Flush(); stream.Flush(true); }
        }
        public IList<AuditRecord> Read()
        {
            var records=new List<AuditRecord>();
            if (!File.Exists(path)) return records;
            foreach (string line in File.ReadAllLines(path,Encoding.UTF8))
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                try { var fields=Json.Read<Dictionary<string,object>>(line);if(fields==null||!fields.ContainsKey("SchemaVersion"))throw new InvalidDataException("Missing audit schema version.");var record=Json.Read<AuditRecord>(line);AuditValidation.Validate(record);records.Add(record); }
                catch (Exception ex) { throw new InvalidDataException("Audit journal is damaged. Writes are blocked until it is recovered locally.",ex); }
            }
            return records;
        }
    }

    public static class AuditValidation
    {
        public static void Validate(AuditRecord r)
        {
            DateTimeOffset timestamp;
            if(r==null||r.SchemaVersion!="1"||new[]{r.Transaction,r.FindingId,r.Project,r.Verse,r.Build,r.Rule,r.Catalog,r.Config,r.Before,r.BeforeHash,r.Action,r.State}.Any(String.IsNullOrWhiteSpace)||r.Book<1||r.Chapter<1||r.Start<0||!DateTimeOffset.TryParse(r.Timestamp,CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind,out timestamp)||!Hash(r.BeforeHash)||!Hash(r.Config))throw new InvalidDataException("Incomplete or invalid audit record.");
            if(r.State=="reviewed")
            {
                if(!new[]{"rejected-occurrence","ignored-occurrence","ignored-project","open-project"}.Contains(r.Action))throw new InvalidDataException("Invalid review action.");
            }
            else if(!new[]{"apply","modify-and-apply","revert"}.Contains(r.Action)||!new[]{"pending","applied","uncertain","not-applied","reverted"}.Contains(r.State)||String.IsNullOrWhiteSpace(r.After)||!Hash(r.AfterHash)||(r.Action=="revert"&&r.State=="applied")||(r.Action!="revert"&&r.State=="reverted"))throw new InvalidDataException("Incomplete or invalid write transaction.");
        }
        private static bool Hash(string value){return value!=null&&Regex.IsMatch(value,@"\A[0-9a-f]{64}\z");}
    }
    public static class ReviewHistory
    {
        public static IList<AuditRecord> Current(IList<AuditRecord> records)
        {
            var latest=records.GroupBy(r=>r.Transaction).Select(g=>Json.Read<AuditRecord>(Json.Write(g.Last()))).ToList();
            foreach(var revert in latest.Where(r=>r.Action=="revert"&&r.State=="reverted").ToList())
                foreach(var original in latest.Where(r=>r.Action!="revert"&&(r.Transaction==revert.RevertsTransaction||(String.IsNullOrEmpty(revert.RevertsTransaction)&&r.FindingId==revert.FindingId&&r.AfterHash==revert.BeforeHash&&String.CompareOrdinal(r.Timestamp,revert.Timestamp)<=0))))original.State="reverted";
            return latest;
        }
    }

    public sealed class PatchService
    {
        private readonly IChapterStore store;
        private readonly IJournal journal;
        private readonly string build;
        private readonly Func<string> currentConfig;
        public PatchService(IChapterStore chapterStore,IJournal audit,string buildVersion,Func<string> currentConfiguration=null) { store=chapterStore;journal=audit;build=buildVersion;currentConfig=currentConfiguration; }
        public static string Patch(string source,int start,string before,string after,string verse)
        {
            if (start<0 || before.Length==0 || start+before.Length>source.Length || source.Substring(start,before.Length)!=before)
                throw new InvalidOperationException("The source span has changed. Rescan before applying.");
            if (!TextIdentity.Boundary(source,start) || !TextIdentity.Boundary(source,start+before.Length) || !Usfm.ContainsSpan(source,start,before.Length,verse))
                throw new InvalidOperationException("The span crosses a grapheme, verse or protected structure.");
            if (String.IsNullOrWhiteSpace(after) || after.Length>4096 || after.Any(c=>(Char.IsControl(c) && c!='\t' && c!='\r' && c!='\n') || c=='\\' || c=='|' || c=='\u2028' || c=='\u2029'))
                throw new InvalidOperationException("Replacement must be a nonempty text span without markers, attributes or unsupported controls.");
            // Existing USFM line wrapping and tabs belong to the source. Permit
            // their exact sequence through Apply/Revert, never add or remove it.
            if (!Regex.Matches(before,@"[\t\r\n]+").Cast<Match>().Select(m=>m.Value).SequenceEqual(Regex.Matches(after,@"[\t\r\n]+").Cast<Match>().Select(m=>m.Value)))
                throw new InvalidOperationException("Replacement must preserve the source line breaks and tabs exactly.");
            // Strict UTF-8 encoding catches unpaired UTF-16 surrogate values.
            TextIdentity.Hash(after);
            var firstCategory=Char.GetUnicodeCategory(after,0);
            if (Char.IsLowSurrogate(after[0]) || Char.IsHighSurrogate(after[after.Length-1]) || firstCategory==System.Globalization.UnicodeCategory.NonSpacingMark || firstCategory==System.Globalization.UnicodeCategory.SpacingCombiningMark || firstCategory==System.Globalization.UnicodeCategory.EnclosingMark)
                throw new InvalidOperationException("Replacement starts or ends with an incomplete character.");
            string result=source.Substring(0,start)+after+source.Substring(start+before.Length);
            if (!TextIdentity.Boundary(result,start) || !TextIdentity.Boundary(result,start+after.Length) || !Usfm.ContainsSpan(result,start,after.Length,verse))
                throw new InvalidOperationException("Replacement does not preserve a valid text span.");
            return result;
        }
        public AuditRecord Apply(Finding finding,string replacement,string reviewer,bool modified,string configHash)
        {
            if (store.ProjectId!=finding.Project || finding.State!=ReviewState.Open || finding.Suggested==null || configHash!=finding.ConfigHash)
                throw new InvalidOperationException("This finding is not current or actionable. Rescan first.");
            if (!modified && replacement!=finding.Suggested) throw new InvalidOperationException("Replacement differs from the reviewed suggestion.");
            if (!store.CanEdit(finding.Book,finding.Chapter)) throw new InvalidOperationException("Paratext does not permit editing this chapter.");
            using (var held=store.Lock(finding.Book,finding.Chapter))
            {
                if (held==null) throw new InvalidOperationException("Chapter write lock is unavailable.");
                if(currentConfig!=null&&currentConfig()!=finding.ConfigHash){finding.State=ReviewState.Stale;throw new InvalidOperationException("Project house style changed. Rescan before applying.");}
                string source=store.Read(finding.Book,finding.Chapter);
                if (TextIdentity.Hash(source)!=finding.ChapterHash) { finding.State=ReviewState.Stale;throw new InvalidOperationException("Chapter changed since scanning. Rescan before applying."); }
                Reconcile(finding.Book,finding.Chapter,source);
                string result=Patch(source,finding.Start,finding.Original,replacement,finding.Verse);
                var record=new AuditRecord { Transaction=Guid.NewGuid().ToString("N"),FindingId=finding.Id,Project=finding.Project,Book=finding.Book,Chapter=finding.Chapter,Verse=finding.Verse,Start=finding.Start,Before=finding.Original,After=replacement,BeforeHash=finding.ChapterHash,AfterHash=TextIdentity.Hash(result),Reviewer=reviewer,Action=modified?"modify-and-apply":"apply",Build=build,Rule=finding.Rule,Catalog=finding.CatalogVersion,Config=finding.ConfigHash,Evidence=finding.Evidence,Timestamp=DateTime.UtcNow.ToString("o"),State="pending" };
                record.Proposed=finding.Suggested;
                journal.Append(record); // Durable intent must precede any host write.
                try
                {
                    store.Write(finding.Book,finding.Chapter,result);
                    if (store.Read(finding.Book,finding.Chapter)!=result) throw new IOException("The saved chapter differs from the proposed result.");
                    record.State="applied";journal.Append(record);finding.AppliedText=replacement;finding.State=ReviewState.Applied;return record;
                }
                catch { finding.State=ReviewState.Uncertain;record.State="uncertain";try { journal.Append(record); } catch { } throw; }
            }
        }
        private void Reconcile(int book,int chapter,string current)
        {
            foreach (var r in journal.Read().Where(r=>r.Project==store.ProjectId && r.Book==book && r.Chapter==chapter).GroupBy(r=>r.Transaction).Select(g=>g.Last()).Where(r=>r.State=="pending" || r.State=="uncertain"))
            {
                string hash=TextIdentity.Hash(current);
                if (hash==r.AfterHash) r.State=r.Action=="revert"?"reverted":"applied";
                else if (hash==r.BeforeHash) r.State="not-applied";
                else throw new InvalidOperationException("An interrupted write has an uncertain outcome. Review the local journal before further edits.");
                journal.Append(r);
            }
        }
        public void Revert(AuditRecord applied,string reviewer,Finding finding=null)
        {
            if (applied.Project!=store.ProjectId || !store.CanEdit(applied.Book,applied.Chapter)) throw new InvalidOperationException("Cannot revert this project/chapter.");
            using(var held=store.Lock(applied.Book,applied.Chapter))
            {
                if(held==null) throw new InvalidOperationException("Write lock unavailable.");
                string source=store.Read(applied.Book,applied.Chapter);
                Reconcile(applied.Book,applied.Chapter,source);
                if(TextIdentity.Hash(source)!=applied.AfterHash) throw new InvalidOperationException("Chapter changed after this edit. Revert is blocked to preserve later edits.");
                string result=Patch(source,applied.Start,applied.After,applied.Before,applied.Verse);
                var r=Json.Read<AuditRecord>(Json.Write(applied));r.Transaction=Guid.NewGuid().ToString("N");r.RevertsTransaction=applied.Transaction;r.Action="revert";r.State="pending";r.Reviewer=reviewer;r.Timestamp=DateTime.UtcNow.ToString("o");r.Before=applied.After;r.After=applied.Before;r.BeforeHash=applied.AfterHash;r.AfterHash=TextIdentity.Hash(result);
                journal.Append(r);
                try { store.Write(r.Book,r.Chapter,result);if(store.Read(r.Book,r.Chapter)!=result) throw new IOException("Revert verification failed.");r.State="reverted";journal.Append(r);if(finding!=null&&finding.Id==applied.FindingId){finding.State=ReviewState.Reverted;finding.AppliedText=null;} }
                catch { r.State="uncertain";try { journal.Append(r); } catch { } throw; }
            }
        }
    }
}

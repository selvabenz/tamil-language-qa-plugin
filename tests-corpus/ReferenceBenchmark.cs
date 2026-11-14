using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TamilSandhi;

// Standalone local evaluator. Reference bytes and snippets never enter the product package.
internal static class ReferenceBenchmark
{
    private static readonly int[] DevelopmentBooks={1,19,40,43};
    private static readonly string[] BookCodes=("GEN EXO LEV NUM DEU JOS JDG RUT 1SA 2SA 1KI 2KI 1CH 2CH EZR NEH EST JOB PSA PRO ECC SNG ISA JER LAM EZK DAN HOS JOL AMO OBA JON MIC NAM HAB ZEP HAG ZEC MAL MAT MRK LUK JHN ACT ROM 1CO 2CO GAL EPH PHP COL 1TH 2TH 1TI 2TI TIT PHM HEB JAS 1PE 2PE 1JN 2JN 3JN JUD REV").Split(' ');
    public static int Main(string[] args)
    {
        try
        {
            string reference=Path.GetFullPath(args[0]),output=Path.GetFullPath(args[1]),stage=args[2];
            if(stage!="development"&&stage!="holdout")throw new ArgumentException("Invalid evaluation stage");
            Directory.CreateDirectory(output);
            var identifiers=new HashSet<int>();
            var catalog=Json.Read<Catalog>(File.ReadAllText(args[3],Encoding.UTF8));var engine=new SandhiEngine(catalog);
            var strict=new UTF8Encoding(false,true);var details=new List<object>();var manifest=new List<object>();var byRule=new Dictionary<string,int>();
            int books=0,chapters=0,verseUnits=0,changes=0,reviews=0,examined=0,supported=0,unassessed=0,mutationCandidates=0,mutationRecovered=0;
            var timer=System.Diagnostics.Stopwatch.StartNew();
            foreach(string path in Directory.GetFiles(reference,"*.usfm").OrderBy(p=>p,StringComparer.Ordinal))
            {
                byte[] before=File.ReadAllBytes(path);string text=strict.GetString(before).TrimStart('\uFEFF');
                var ids=Regex.Matches(text,@"(?m)^\\id[ \t]+(?<code>\S+)");
                int book=ids.Count==1?Array.IndexOf(BookCodes,ids[0].Groups["code"].Value)+1:0;
                if(book<1||!identifiers.Add(book))throw new InvalidDataException("Invalid or duplicate USFM book identity in "+Path.GetFileName(path));
                if(DevelopmentBooks.Contains(book)!=(stage=="development"))continue;
                string hash=TextIdentity.Hash(text);var markers=Regex.Matches(text,@"(?m)^\\c\s+(?<chapter>\d+)\s*").Cast<Match>().ToArray();
                if(markers.Length==0)throw new InvalidDataException("No chapters in "+Path.GetFileName(path));books++;
                if(markers.Select(m=>m.Groups["chapter"].Value).Distinct().Count()!=markers.Length)throw new InvalidDataException("Duplicate chapter identity");
                foreach(var at in markers.Select((m,i)=>new{Marker=m,End=i+1<markers.Length?markers[i+1].Index:text.Length}))
                {
                    int chapter=Int32.Parse(at.Marker.Groups["chapter"].Value);string raw=text.Substring(at.Marker.Index,at.End-at.Marker.Index);
                    var snapshot=new ChapterSnapshot{Project="local-reference",Book=book,Chapter=chapter,Text=raw};
                    var report=engine.AnalyzeDetailed(snapshot,new HouseStyle(),null,null);chapters++;
                    verseUnits+=Usfm.Segments(raw).Select(s=>s.Verse).Distinct().Count();examined+=report.ExaminedBoundaries;supported+=report.SupportedBoundaries;unassessed+=report.UnassessedBoundaries;
                    foreach(var f in report.Findings)
                    {
                        if(f.Actionable){changes++;if(!byRule.ContainsKey(f.Rule))byRule[f.Rule]=0;byRule[f.Rule]++;}else reviews++;
                        details.Add(new{book=book,chapter=chapter,verse=f.Verse,rule=f.Rule,operation=f.Operation,original=f.Original,suggested=f.Suggested,context=f.Context,blockers=f.Blockers});
                    }
                    foreach(var segment in Usfm.Segments(raw))
                    {
                        string part=raw.Substring(segment.Start,segment.Length);
                        foreach(Match pair in Regex.Matches(part,@"(?=(?<pair>(?<a>[\u0B80-\u0BFF]+[கசதப]்)(?<gap>[ \t\r\n]+)(?<b>[கசதப][\u0B80-\u0BFF]*)))"))
                        {
                            if(pair.Index>0&&Regex.IsMatch(part.Substring(pair.Index-1,1),@"[\u0B80-\u0BFF]"))continue;
                            mutationCandidates++;string a=pair.Groups["a"].Value;
                            string broken=a.Substring(0,a.Length-2)+pair.Groups["gap"].Value+pair.Groups["b"].Value;
                            var f=engine.Analyze(new ChapterSnapshot{Project="controlled-missing-ottu",Book=book,Chapter=chapter,Text=broken},new HouseStyle(),null,null);
                            if(f.Count==1&&f[0].Actionable&&f[0].Suggested==pair.Groups["pair"].Value)mutationRecovered++;
                        }
                    }
                }
                if(!before.SequenceEqual(File.ReadAllBytes(path)))throw new IOException("Reference changed during evaluation; discard run");
                manifest.Add(new{file=Path.GetFileName(path),textSha256=hash,unchanged=true});
            }
            timer.Stop();
            if(identifiers.Count!=66)throw new InvalidDataException("Expected all 66 canonical USFM book identities");
            if(books==0||chapters==0||verseUnits==0)throw new InvalidDataException("No Scripture evaluated");
            var summary=new{build=BuildInfo.Version,profile=ScriptureRuleProfile.Version,stage=stage,readOnly=true,reference=args.Length>4?args[4]:"User-supplied local BSI OV USFM",books=books,chapters=chapters,verseUnits=verseUnits,examinedBoundaries=examined,supportedBoundaries=supported,unassessedBoundaries=unassessed,proposedChangesToReviewedText=changes,reviewOnlyFindings=reviews,proposedChangesByRule=byRule,controlledMissingOttuCandidates=mutationCandidates,controlledMissingOttuRecovered=mutationRecovered,allInputFilesUnchanged=true,independentNaturalErrorAccuracyMeasured=false,elapsedMilliseconds=timer.ElapsedMilliseconds};
            File.WriteAllText(Path.Combine(output,"private-findings.json"),Json.Write(details),strict);
            File.WriteAllText(Path.Combine(output,"private-input-manifest.json"),Json.Write(manifest),strict);
            File.WriteAllText(Path.Combine(output,"summary.json"),Json.Write(summary),strict);
            Console.WriteLine(Json.Write(summary));return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TamilSandhi
{
    // Unknown markers and character runs fail closed. No reconstruction or normalization of source.
    public static class Usfm
    {
        private static readonly Regex Marker = new Regex(@"\\(?<name>\+?[A-Za-z][A-Za-z0-9-]*)(?<end>\*)?", RegexOptions.Compiled);
        private static readonly Regex Verse = new Regex(@"^\s*(?<verse>[0-9]+[a-z]?(?:-[0-9]+[a-z]?)?)\s+", RegexOptions.Compiled);
        private static readonly HashSet<string> Paragraphs = new HashSet<string>(new[] { "p", "m", "po", "pr", "cls", "pmo", "pm", "pmc", "pmr", "pi", "pi1", "pi2", "pi3", "mi", "nb", "pc", "ph", "ph1", "ph2", "q", "q1", "q2", "q3", "q4", "qr", "qc", "qm", "qm1", "qm2", "qm3", "qd", "b", "li", "li1", "li2", "li3", "li4" });

        public static List<TextSpan> Segments(string raw)
        {
            var result = new List<TextSpan>();
            var matches = Marker.Matches(raw).Cast<Match>().ToList();
            if (matches.Count == 0) { result.Add(new TextSpan { Start=0, Length=raw.Length, Verse="1" }); return result; }
            string verse = null;
            bool scripture = false;
            var protectedStack = new Stack<string>();
            foreach (var pair in matches.Select((m,i) => new { Match=m, Next=i+1 < matches.Count ? matches[i+1].Index : raw.Length }))
            {
                var m = pair.Match;
                string name = m.Groups["name"].Value.TrimStart('+');
                bool end = m.Groups["end"].Success;
                int start = m.Index + m.Length;
                if (end)
                {
                    if (protectedStack.Count > 0 && protectedStack.Peek() == name) protectedStack.Pop();
                    // Do not resume unknown/heading text after an unmatched close marker.
                }
                else if (name == "c" && protectedStack.Count == 0) { verse = null; scripture = false; }
                else if (name == "v" && protectedStack.Count == 0)
                {
                    var vm = Verse.Match(raw.Substring(start, pair.Next-start));
                    if (!vm.Success) { scripture=false; continue; }
                    verse=vm.Groups["verse"].Value; start+=vm.Length; scripture=true;
                }
                else if (Paragraphs.Contains(name) && protectedStack.Count == 0) { scripture = verse != null; }
                else if (!end)
                {
                    // A paired character/note/word marker protects everything until its exact close.
                    if (Regex.IsMatch(raw.Substring(start), @"\\\+?" + Regex.Escape(name) + @"\*")) protectedStack.Push(name);
                    // Unpaired note components such as ft/xt are contained by
                    // their outer f/x marker. They must not disable the verse
                    // text that resumes after the outer marker closes.
                    else if (protectedStack.Count == 0) scripture = false;
                }
                if (scripture && protectedStack.Count == 0 && start < pair.Next)
                    result.Add(new TextSpan { Start=start, Length=pair.Next-start, Verse=verse });
            }
            return result;
        }
        public static bool ContainsSpan(string raw, int start, int length, string verse)
        {
            return length > 0 && Segments(raw).Any(s => s.Verse == verse && start >= s.Start && start + length <= s.End);
        }
        public static bool VerseContains(string label, int number)
        {
            var ns = Regex.Matches(label ?? "", @"\d+").Cast<Match>().Select(m => Int32.Parse(m.Value)).ToArray();
            return ns.Length > 0 && number >= ns[0] && number <= ns[ns.Length-1];
        }
    }
}

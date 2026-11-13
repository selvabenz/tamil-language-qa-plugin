using System;
using System.Collections.Generic;
using System.Linq;

namespace TamilSandhi
{
    public static class SelectionMapper
    {
        public static Dictionary<string,TextSpan> Map(IList<ChapterSnapshot> chapters,string firstVerseUsfm,int offset,string selectedUsfm)
        {
            if(chapters==null||chapters.Count==0||String.IsNullOrEmpty(firstVerseUsfm)||String.IsNullOrEmpty(selectedUsfm)||offset<0)
                throw new InvalidOperationException("Select a nonempty Scripture passage first.");
            if(chapters.Any(c=>c.Project!=chapters[0].Project))throw new InvalidOperationException("Selection crosses projects.");
            string first=chapters[0].Text;
            int verseStart=first.IndexOf(firstVerseUsfm,StringComparison.Ordinal);
            if(verseStart<0||first.IndexOf(firstVerseUsfm,verseStart+1,StringComparison.Ordinal)>=0||offset>firstVerseUsfm.Length)
                throw new InvalidOperationException("The selection start cannot be mapped uniquely to raw Scripture.");
            int selectionStart=verseStart+offset;
            string combined=String.Concat(chapters.Select(c=>c.Text));
            int selectionEnd=selectionStart+selectedUsfm.Length;
            if(selectionEnd>combined.Length||combined.Substring(selectionStart,selectedUsfm.Length)!=selectedUsfm)
                throw new InvalidOperationException("Selected raw USFM no longer matches the source. Select the passage again; the checker will not widen its scope.");
            if(!TextIdentity.Boundary(combined,selectionStart)||!TextIdentity.Boundary(combined,selectionEnd))throw new InvalidOperationException("Selection ends inside a Unicode grapheme.");
            var result=new Dictionary<string,TextSpan>();int chapterStart=0;
            foreach(var chapter in chapters)
            {
                int start=Math.Max(selectionStart,chapterStart),end=Math.Min(selectionEnd,chapterStart+chapter.Text.Length);
                if(end>start)result.Add(chapter.Book+":"+chapter.Chapter,new TextSpan{Start=start-chapterStart,Length=end-start});
                chapterStart+=chapter.Text.Length;
            }
            return result;
        }
    }
}

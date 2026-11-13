using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Paratext.PluginInterfaces;

namespace TamilSandhi.Plugin
{
    internal sealed class SandhiAnnotations : IPluginAnnotationSource
    {
        private readonly IProject project;
        public SandhiAnnotations(IProject selected){project=selected;ReviewControl.ResultsChanged+=OnChanged;project.ProjectDeleted+=Unsubscribe;}
        private void Unsubscribe(){ReviewControl.ResultsChanged-=OnChanged;project.ProjectDeleted-=Unsubscribe;}
        public event EventHandler AnnotationsChanged;
        private void OnChanged(object sender,EventArgs args){var handler=AnnotationsChanged;if(handler!=null)handler(this,EventArgs.Empty);}
        public bool MaintainSelectionsOnWordBoundaries {get{return false;}}
        public IReadOnlyList<AnnotationStyle> GetStyleInfo(double zoom){return new[]{new AnnotationStyle("tamil-sandhi","border-bottom: 2px dotted #b36b00;")};}
        public IReadOnlyList<IPluginAnnotation> GetAnnotations(IVerseRef verse,string usfm)
        {
            var result=new List<IPluginAnnotation>();ReviewControl session;
            if(!ReviewControl.Sessions.TryGetValue(project.ID,out session))return result;
            foreach(var f in session.CurrentFindings.Where(f=>f.Book==verse.BookNum&&f.Chapter==verse.ChapterNum&&Usfm.VerseContains(f.Verse,verse.VerseNum)&&(f.State==ReviewState.Open||f.State==ReviewState.NeedsReview)))
            {
                string chapter=session.ChapterSource(f);if(chapter==null)continue;
                int verseStart=chapter.IndexOf(usfm,StringComparison.Ordinal);
                if(verseStart<0||chapter.IndexOf(usfm,verseStart+1,StringComparison.Ordinal)>=0)continue;
                int offset=f.Start-verseStart;
                if(offset<0||offset+f.Length>usfm.Length||usfm.Substring(offset,f.Length)!=f.Original)continue;
                result.Add(new Annotation{ScriptureSelection=new Selection{SelectedText=f.Original,BeforeContext=usfm.Substring(0,offset),AfterContext=usfm.Substring(offset+f.Length),VerseRefStart=verse,VerseRefEnd=verse,Offset=offset},Session=session,Finding=f});
            }
            return result;
        }
        private sealed class Annotation : IPluginAnnotation
        {
            public IScriptureTextSelection ScriptureSelection {get;set;}
            public string StyleName {get{return "tamil-sandhi";}}
            public string Icon {get{return null;}}
            public string IconToolTipText {get{return Finding.Reason;}}
            public ReviewControl Session;public Finding Finding;
            public bool Click(MouseButton button,bool onIcon,Point location){Session.InlineMenu(Finding,location);return true;}
        }
        private sealed class Selection : IScriptureTextSelection
        {
            public IVerseRef VerseRefStart {get;set;}
            public IVerseRef VerseRefEnd {get;set;}
            public string SelectedText {get;set;}
            public int Offset {get;set;}
            public string BeforeContext {get;set;}
            public string AfterContext {get;set;}
            public bool Equals(ISelection other){var s=other as IScriptureTextSelection;return s!=null&&s.VerseRefStart.Equals(VerseRefStart)&&s.Offset==Offset&&s.SelectedText==SelectedText;}
        }
    }
}

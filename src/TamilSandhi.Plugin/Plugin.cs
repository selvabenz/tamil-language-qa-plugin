using System;
using System.Collections.Generic;
using Paratext.PluginInterfaces;

namespace TamilSandhi.Plugin
{
    public sealed class TamilSandhiPlugin : IParatextWindowPlugin, IParatextScrTextAnnotationPlugin
    {
        public string Name {get{return "Tamil Sandhi QA";}}
        public Version Version {get{return typeof(BuildInfo).Assembly.GetName().Version;}}
        public string VersionString {get{return BuildInfo.Version;}}
        public string Publisher {get{return "selvabenz";}}
        public string GetDescription(string locale){return "Offline verse-by-verse Tamil sandhi review with explicit safe application. Development preview; limited linguistic coverage.";}
        public IDataFileMerger GetMerger(IPluginHost host,string identifier){return null;}
        public IEnumerable<WindowPluginMenuEntry> PluginMenuEntries
        {
            get
            {
                yield return new WindowPluginMenuEntry("Tamil Sandhi QA...",Open,PluginMenuLocation.ScrTextToolsChecks);
                yield return new WindowPluginMenuEntry("Tamil Sandhi — review selection...",OpenSelection,PluginMenuLocation.ScrTextContextMenuDefault);
            }
        }
        public IEnumerable<PluginAnnotationMenuEntry> PluginAnnotationMenuEntries
        {
            get {yield return new PluginAnnotationMenuEntry("Tamil sandhi findings",delegate(IProject p){return new SandhiAnnotations(p);});}
        }
        private static void Open(IWindowPluginHost host,IParatextChildState state){OpenWindow(host,state,false);}
        private static void OpenSelection(IWindowPluginHost host,IParatextChildState state){OpenWindow(host,state,true);}
        private static void OpenWindow(IWindowPluginHost host,IParatextChildState state,bool selection)
        {
            if(state==null || state.Project==null)throw new InvalidOperationException("Open a Scripture project first.");
            host.ShowEmbeddedUi(new ReviewControl(state,selection),state.Project);
        }
    }
}

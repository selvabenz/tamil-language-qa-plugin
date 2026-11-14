using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Windows.Forms;

internal static class WindowRegressions
{
    private sealed class Stub:RealProxy
    {
        private readonly Func<string,object[],object> run;
        public Stub(Type type,Func<string,object[],object> callback):base(type){run=callback;}
        public override IMessage Invoke(IMessage message)
        {
            var call=(IMethodCallMessage)message;
            try{return new ReturnMessage(run(call.MethodName,call.Args),null,0,call.LogicalCallContext,call);}
            catch(Exception ex){return new ReturnMessage(ex,call);}
        }
    }
    public static void Run(Control control,string output,string sdk)
    {
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;var review=control.GetType();
        var contracts=Assembly.LoadFrom(Path.Combine(sdk,"CorePluginInterfaces.dll"));
        Func<string,Type> type=n=>contracts.GetType("Paratext.PluginInterfaces."+n,true);
        object book=new Stub(type("IBookInfo"),(n,a)=>n=="get_Number"?(object)1:n=="get_Code"?"GEN":null).GetTransparentProxy();
        var books=Array.CreateInstance(type("IBookInfo"),1);books.SetValue(book,0);
        object versification=new Stub(type("IVersification"),(n,a)=>n=="GetLastChapter"?(object)3:null).GetTransparentProxy();
        object project=new Stub(type("IProject"),(n,a)=>
        {
            if(n=="get_ID")return "synthetic-window-regression";
            if(n=="get_ShortName")return "Synthetic";
            if(n=="get_AvailableBooks")return books;
            if(n=="get_Versification")return versification;
            if(n=="GetUSFM"){int chapter=(int)a[1];if(chapter==2)throw new IOException("Synthetic unreadable chapter");return chapter==1?"\\c 1\n\\v 1 அறிக்கை செய்தான்.":"\\c 3\n\\v 1 அவரை பார்த்தான்.";}
            return null;
        }).GetTransparentProxy();
        object state=new Stub(type("IParatextChildState"),(n,a)=>n=="get_Project"?project:null).GetTransparentProxy();
        object selection=new Stub(type("IScriptureTextSelection"),(n,a)=>n=="get_SelectedText"?(object)"old selection":null).GetTransparentProxy();
        object reference=new Stub(type("IVerseRef"),(n,a)=>n=="get_BookNum"||n=="get_ChapterNum"||n=="get_VerseNum"?(object)1:null).GetTransparentProxy();
        review.GetField("project",flags).SetValue(control,project);
        try
        {
            review.GetField("initialSelection",flags).SetValue(control,selection);
            review.GetMethod("SelectionChanged",flags).Invoke(control,new object[]{null,state,Array.CreateInstance(type("ISelection"),0)});
            if(review.GetField("initialSelection",flags).GetValue(control)!=null)throw new Exception("Cleared selection retained an old passage.");
            var core=Assembly.LoadFrom(Path.Combine(output,"TamilSandhi.Core.dll"));
            var store=Activator.CreateInstance(core.GetType("TamilSandhi.HouseStyleStore"),new object[]{Path.Combine(output,"window-settings-"+Guid.NewGuid().ToString("N")+".json")});
            review.GetField("styleStore",flags).SetValue(control,store);
            string rules=Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(output)),"reference/tamil_punarchi_agent_package/rules.json");
            var catalog=core.GetType("TamilSandhi.Json").GetMethod("Read").MakeGenericMethod(core.GetType("TamilSandhi.Catalog")).Invoke(null,new object[]{File.ReadAllText(rules)});
            review.GetField("engine",flags).SetValue(control,Activator.CreateInstance(core.GetType("TamilSandhi.SandhiEngine"),new[]{catalog}));
            review.GetField("reference",flags).SetValue(control,reference);
            ((ComboBox)review.GetField("scope",flags).GetValue(control)).SelectedIndex=5;
            System.Threading.SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            review.GetMethod("StartScan",flags).Invoke(control,null);
            var limit=DateTime.UtcNow.AddSeconds(10);
            while((bool)review.GetField("scanning",flags).GetValue(control)&&DateTime.UtcNow<limit){Application.DoEvents();System.Threading.Thread.Sleep(1);}
            Application.DoEvents();
            string status=((Label)review.GetField("status",flags).GetValue(control)).Text;
            if((bool)review.GetField("scanning",flags).GetValue(control)||!status.Contains("2 chapters")||!status.Contains("1 failed")||!status.Contains("1 findings"))throw new Exception("Chapter failure did not continue accurately: "+status);
            review.GetMethod("OnHouseStyleChanged",flags).Invoke(control,new object[]{"synthetic-window-regression"});
            var findings=(System.Collections.IEnumerable)review.GetField("findings",flags).GetValue(control);
            foreach(var finding in findings)if(finding.GetType().GetProperty("State").GetValue(finding,null).ToString()!="Stale")throw new Exception("Project settings notification did not invalidate findings.");
        }
        finally{review.GetField("project",flags).SetValue(control,null);}
    }
}

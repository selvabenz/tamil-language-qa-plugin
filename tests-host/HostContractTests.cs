using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class HostContractTests
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            string output=Path.GetFullPath(args[0]),sdk=Path.GetFullPath(args[1]);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string name=new AssemblyName(e.Name).Name;foreach(string directory in new[]{output,sdk})foreach(string extension in new[]{".dll",".ptxplg"}){string path=Path.Combine(directory,name+extension);if(File.Exists(path))return Assembly.LoadFrom(path);}return null;};
            var assembly=Assembly.LoadFrom(Path.Combine(output,"TamilSandhi.ptxplg"));
            var plugin=Activator.CreateInstance(assembly.GetType("TamilSandhi.Plugin.TamilSandhiPlugin",true));
            var interfaces=plugin.GetType().GetInterfaces().Select(i=>i.Name).ToArray();
            if(!interfaces.Contains("IParatextWindowPlugin")||!interfaces.Contains("IParatextScrTextAnnotationPlugin"))throw new Exception("Plugin interfaces missing.");
            foreach(string name in new[]{"PluginMenuEntries","PluginAnnotationMenuEntries"})
            {
                var entries=(System.Collections.IEnumerable)plugin.GetType().GetProperty(name).GetValue(plugin,null);int count=0;foreach(var entry in entries)count++;if(count==0)throw new Exception("Missing host menu: "+name);
            }
            using(var control=(Control)Activator.CreateInstance(assembly.GetType("TamilSandhi.Plugin.ReviewControl",true)))
            using(var form=new Form{Width=1100,Height=750})
            {
                form.Controls.Add(control);form.CreateControl();control.CreateControl();form.PerformLayout();
                if(control.Controls.Count<3)throw new Exception("Review layout not created.");
            }
            Console.WriteLine("PASS: plugin loads against SDK; both interface contracts, menus and native control construction verified. This is not an in-host acceptance test.");return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

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
                form.ShowInTaskbar=false;form.StartPosition=FormStartPosition.Manual;form.Location=new Point(-10000,-10000);
                form.Controls.Add(control);form.Show();Application.DoEvents();form.PerformLayout();
                if(control.Controls.Count<3)throw new Exception("Review layout not created.");
                var scan=(Button)control.GetType().GetField("scan",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(control);
                int invoked=0;scan.Click+=delegate{invoked++;};
                var toolbar=control.Controls.OfType<ToolStrip>().First(t=>t.Dock==DockStyle.Top);
                toolbar.Items.OfType<ToolStripButton>().First(i=>i.Text==scan.Text).PerformClick();
                if(invoked!=1)throw new Exception("Toolbar action did not reach the shared command.");
                WindowRegressions.Run(control,output,sdk);
                form.Size=new Size(280,650);form.PerformLayout();
                if(control.Controls.OfType<ToolStrip>().Any(t=>t.Height>60))throw new Exception("Narrow dock toolbars wrapped vertically.");
                using(var image=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(image,new Rectangle(Point.Empty,form.Size));image.Save(Path.Combine(output,"review-narrow.png"));}
            }
            Console.WriteLine("PASS: SDK interfaces, menus, native control, toolbar relay, narrow layout, cleared selection, failed-chapter continuation and settings invalidation. This is not an in-host acceptance test.");return 0;
        }
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}

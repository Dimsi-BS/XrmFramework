using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace XrmFramework.XrmToolbox
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "XrmFramework Manager"),
        ExportMetadata("Description", "Helps you manage your XrmFramework project : Modify .table/.model files, etc..."),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAF6UExURfv7+/r7+73o+GjO9FrJ82DM86Lg9vn6+8Dp+Aqy8ACt7QOv74/a9m7Q9TzB8/D3+mPN9AOu7XfT9Y3b93nU9lvK9DO98RGy7gGt7TO/8+33+giw7uD1/f////P7/tHx/H7W9iK48N/1/en4/nHR9Quw7vv+/6jj+azk+Q2x7gKu7XfU9ZHc+K/l+f3+//z+/3zV9gCs7Q6x7t71/e76/im68CC37ye57xe07zG98eL2/Y3a9wev7tr0/KTi+VXI8/j9/9jz/BWz7g+x7snu+/X8/jW98YbY9lTI82vO9GbO9WrO9GbN9YPX9lLH8wyx7sLs+03G8vb8/tXx/BSz7iy78IfZ9wKt7V7L9Nnz/Oz5/ia58Ira94va947b963l+dvz/XfU9v7///r+/6Hh+eb3/WvP9Qqw7vT8/tDw/B6273TT9Yna93rT9V3L9BKy7jfA8+73+qvi9wSv7wGu7nbT9Jfd90LD8zjC9DzC9PL4+/H4+u/3+vD4+vS4s1gAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFLSURBVDhPfZNnU8JAFEU3oqJro2iIJaioQbCBvaLYO/YK9t5797+7TbMZ8zhfsnfumexm8hYRFBDaIpThyATIynaSPicXg+Tlk5cUiGBLYRFCLvJ0e7y/FJeoPo23hNIyLpRX6Cb+yqpqXktCQJSCmtq69IJuBH1WQd6CUx+ShXCDOKK3sSnQzISWiCyYRHGrq40Z7R0k/gmdXYRuusJY6+llRp/bFPoHDEKMCxgPDlEhPiwdkn1FkNcYj4zSODYOCtoEjfokKOApJkzDwgwTZuEt5pgwDwqJBRr9KigsLtG4vAII2ipL+tr6PyFK+9DGJuu3tkmwCvFkKrWzu8d/lrFPbasgcxBLpBUOj9g4gMLxySnrTUGeqLPzyAWvTSF8mRRcXd+ot9axd4i1LXf3CD08imDD0zO5nMrL6xvAu+eD3m7l8wvg24nQD18JcKWCg6C0AAAAAElFTkSuQmCC"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class XrmFrameworkPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new XrmFrameworkPluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public XrmFrameworkPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveEventHandler;
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            var currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal));

            // check to see if the failing assembly is one that we reference.
            var refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.FirstOrDefault(a => a.Name == argName);

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                var dir = Path.GetDirectoryName(currAssembly.Location)?.ToLower();
                var folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}
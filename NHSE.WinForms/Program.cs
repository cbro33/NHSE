using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace NHSE.WinForms
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if NETCOREAPP
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            AppDomain.CurrentDomain.AssemblyResolve += ResolveLocalAssemblies;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Main());
            }
            catch (Exception ex)
            {
                const string logName = "NHSE-crash.log";
                try
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logName), ex.ToString());
                }
                catch
                {
                    // ignored: fall back to dialog only
                }

                MessageBox.Show(ex.ToString(), "NHSE crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Assembly? ResolveLocalAssemblies(object? sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (!string.Equals(name.Name, "System.Resources.Extensions", StringComparison.OrdinalIgnoreCase))
                return null;

            var candidate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Resources.Extensions.dll");
            if (!File.Exists(candidate))
                return null;

            try
            {
                return Assembly.LoadFrom(candidate);
            }
            catch
            {
                return null;
            }
        }
    }
}

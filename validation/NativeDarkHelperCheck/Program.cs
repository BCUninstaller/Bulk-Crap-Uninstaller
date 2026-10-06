using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Klocman.Forms;
using UniversalUninstaller;
using UninstallTools;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Uninstaller;

internal static class Program
{
    private static int _checks;

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            AssemblyLoadContext.Default.Resolving += (_, name) =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, name.Name + ".dll");
                return File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : null;
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var dark = args.Contains("--dark") && !SystemInformation.HighContrast;
            Application.SetColorMode(dark ? SystemColorMode.Dark : SystemColorMode.Classic);

            var fixture = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "helper-check-fixture"));
            File.WriteAllText(Path.Combine(fixture.FullName, "sample.txt"), "Disposable UI fixture");
            var nested = fixture.CreateSubdirectory("nested");
            File.WriteAllText(Path.Combine(nested.FullName, "child.txt"), "Nested UI fixture");

            using (var window = new UninstallSelection(fixture))
            {
                window.Show();
                Pump();
                var target = Children(window).OfType<TargetList>().Single();
                var list = Children(window).OfType<TreeListView>().Single();
                Check(list.GetItemCount() == 3, "root and immediate children are visible");
                list.ExpandAll();
                Pump();
                Check(list.GetItemCount() == 4, "nested expansion adds a row");
                Check(SendMessage(list.Handle, 0x1004, IntPtr.Zero, IntPtr.Zero).ToInt32() == 4,
                    "native virtual list contains the rows");
                Check(target.GetItemsToDelete().Single().FullName == fixture.FullName, "initial checked root");
                for (var index = 0; index < list.GetItemCount(); index++) list.UncheckObject(list.GetModelObject(index));
                list.CheckObject(list.GetModelObject(1));
                Check(target.GetItemsToDelete().Single().FullName == nested.FullName, "checked nested selection");
                list.CollapseAll();
                Check(list.GetItemCount() == 1, "collapse retains root");
                list.ExpandAll();
                typeof(Control).GetMethod("RecreateHandle", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(list, null);
                Pump();
                Check(SendMessage(list.Handle, 0x1004, IntPtr.Zero, IntPtr.Zero).ToInt32() == 4,
                    "rows survive handle recreation");
                if (dark)
                {
                    Check(list.BackColor == SystemColors.Window && list.ForeColor == SystemColors.WindowText,
                        "dark list palette");
                    Check(!list.HeaderUsesThemes && !list.GridLines, "dark header and grid adaptation");
                }
                window.Close();
            }

            // Exercise the modal factory that the directory scan actually uses.
            var stages = new Dictionary<string, HashSet<ProgressBarStyle>>();
            var finalValues = new HashSet<string>();
            using var owner = new Form { Text = "Native dark helper validation", ClientSize = new Size(460, 150) };
            owner.Show();
            using var timer = new System.Windows.Forms.Timer { Interval = 50 };
            timer.Tick += (_, _) =>
            {
                var dialog = Application.OpenForms.OfType<LoadingDialog>().FirstOrDefault();
                if (dialog == null) return;
                foreach (var bar in Children(dialog).OfType<ProgressBar>())
                    if (!dark || GetWindowTheme(bar.Handle) == IntPtr.Zero)
                    {
                        if (!stages.TryGetValue(bar.Name, out var observed))
                            stages.Add(bar.Name, observed = new HashSet<ProgressBarStyle>());
                        observed.Add(bar.Style);
                        if (bar.Style == ProgressBarStyle.Blocks && bar.Value == (bar.Name == "progressBar" ? 60 : 40))
                            finalValues.Add(bar.Name);
                    }
            };
            timer.Start();
            var error = LoadingDialog.ShowDialog(owner, "Files and directories", controller =>
            {
                controller.SetMaximum(100);
                controller.SetSubMaximum(100);
                controller.SetSubProgressVisible(true);
                controller.SetProgress(50, "Files and directories", true);
                controller.SetSubProgress(30, "helper-check-fixture", true);
                Thread.Sleep(400);
                controller.SetProgress(-1);
                controller.SetSubProgress(-1, "Marquee");
                Thread.Sleep(400);
                controller.SetProgress(60, "Files and directories", true);
                controller.SetSubProgress(40, "helper-check-fixture", true);
                Thread.Sleep(args.Contains("--visual") ? 60000 : 400);
            });
            timer.Stop();
            owner.Close();
            Check(error == null, "modal loading worker completed");
            foreach (var name in new[] { "progressBar", "progressBar2" })
            {
                Check(stages.TryGetValue(name, out var observed) && observed.Contains(ProgressBarStyle.Marquee)
                    && observed.Contains(ProgressBarStyle.Blocks), name + " retains repair across both styles");
                Check(finalValues.Contains(name), name + " returns to determinate progress");
            }
            Check(File.Exists(Path.Combine(fixture.FullName, "sample.txt")) && File.Exists(Path.Combine(nested.FullName, "child.txt")),
                "checks never delete fixture files");

            if (args.Contains("--launch-helper"))
            {
                var entry = new ApplicationUninstallerEntry
                {
                    DisplayName = fixture.Name,
                    UninstallerKind = UninstallerType.SimpleDelete,
                    UninstallString = $"\"{SimpleDeleteUninstallStringGenerator.UniversalUninstallerFilename.FullName}\" \"{fixture.FullName}\""
                };
                using var process = entry.RunUninstaller();
                Console.WriteLine($"Cancel the helper window to finish. PID={process.Id}");
                var deadline = DateTime.UtcNow.AddMinutes(2);
                while (!process.HasExited && DateTime.UtcNow < deadline)
                {
                    Application.DoEvents();
                    Thread.Sleep(50);
                }
                if (!process.HasExited) throw new TimeoutException("Helper was not canceled");
                Check(process.ExitCode == 1, "helper canceled without deleting files");
            }
            Console.WriteLine($"PASS {(dark ? "dark" : "light")} checks={_checks}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static IEnumerable<Control> Children(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (var descendant in Children(child)) yield return descendant;
        }
    }

    private static void Pump()
    {
        for (var i = 0; i < 10; i++) { Application.DoEvents(); Thread.Sleep(10); }
    }

    private static void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
        _checks++;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, IntPtr lParam);
    [DllImport("uxtheme.dll")]
    private static extern IntPtr GetWindowTheme(IntPtr window);
}

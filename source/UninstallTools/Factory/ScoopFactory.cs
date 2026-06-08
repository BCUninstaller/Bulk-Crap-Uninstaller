using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Factory.Json;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public sealed partial class ScoopFactory : IIndependantUninstallerFactory
    {
        private static string _scoopUserPath;
        private static string _scoopGlobalPath;
        private static string _scriptPath;
        private static string _powershellPath;
        private static readonly JsonContext _jsonContext;

        static ScoopFactory()
        {
            JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web); // ignore property name case
            jsonOptions.Converters.Add(new PowerShellDateTimeOffsetConverter());
            _jsonContext = new JsonContext(jsonOptions);
        }

        private static bool GetScoopInfo()
        {
            try
            {
                _scoopUserPath = Environment.GetEnvironmentVariable("SCOOP");
                if (string.IsNullOrEmpty(_scoopUserPath))
                    _scoopUserPath = Path.Combine(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROFILE), "scoop");

                _scoopGlobalPath = Environment.GetEnvironmentVariable("SCOOP_GLOBAL");
                if (string.IsNullOrEmpty(_scoopGlobalPath))
                    _scoopGlobalPath = Path.Combine(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_APPDATA), "scoop");

                _scriptPath = Path.Combine(_scoopUserPath, "shims\\scoop.ps1");

                if (File.Exists(_scriptPath))
                {
                    _powershellPath = PathTools.GetFullPathOfExecutable("powershell.exe");
                    if (!File.Exists(_powershellPath))
                        throw new InvalidOperationException(@"Detected Scoop program installer, but failed to detect PowerShell");

                    return true;
                }
            }
            catch (SystemException ex)
            {
                Trace.WriteLine("Failed to get Scoop info: " + ex);
            }

            return false;
        }

        private sealed class ExportInfo
        {
            //public ExportBucketEntry[] Buckets { get; set; }
            public ExportAppEntry[] Apps { get; set; }
        }
        //private sealed class ExportBucketEntry
        //{
        //    public string Name { get; set; }
        //    public string Source { get; set; }
        //    public DateTimeOffset Updated { get; set; }
        //    public ulong Manifests { get; set; }
        //}
        private sealed class ExportAppEntry
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Source { get; set; }
            public DateTimeOffset Updated { get; set; }
            public string Info { get; set; }

            [JsonIgnore] public bool IsPublic => Info?.Contains("Global install", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (!GetScoopInfo()) return results;

            // Make uninstaller for scoop itself
            var scoopEntry = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Scoop",
                Comment = "Automated program installer",
                AboutUrl = "https://github.com/ScoopInstaller/Scoop",
                InstallLocation = _scoopUserPath,
                IsOrphaned = false,
                RatingId = "Scoop"
            };

            // Make sure the global directory gets removed as well
            var junk = new FileSystemJunk(new DirectoryInfo(_scoopGlobalPath), scoopEntry, null);
            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
            junk.Confidence.Add(4);
            scoopEntry.AdditionalJunk.Add(junk);

            scoopEntry.UninstallString = MakeScoopCommand("uninstall scoop").ToString();
            scoopEntry.UninstallerKind = UninstallerType.PowerShell;
            results.Add(scoopEntry);

            // Make uninstallers for apps installed by scoop
            var result = RunScoopCommand("export");
            if (string.IsNullOrEmpty(result)) return results;

            var exeSearcher = new AppExecutablesSearcher();

            // JSON export format since July 2022
            try
            {
                var export = JsonSerializer.Deserialize(result, _jsonContext.ExportInfo);
                foreach (var app in export.Apps)
                {
                    var entry = CreateUninstallerEntry(
                        app.Name, app.Version, app.IsPublic, exeSearcher);

                    entry.InstallDate = app.Updated.LocalDateTime;

                    results.Add(entry);
                }
            }
            // Fallback to plain text export format
            catch (JsonException e)
            {
                var appEntries = result.Split(StringTools.NewLineChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                foreach (var str in appEntries)
                {
                    // Format should be "$app (v:$ver) $global_display $bucket $arch"
                    // app has no spaces, $global_display is *global*, bucket is inside [] brackets like [main]
                    // version should always be there but the check errored out for some users, everything after version is optional
                    string name;
                    string version = null;
                    bool isGlobal = false;
                    var spaceIndex = str.IndexOf(" ", StringComparison.Ordinal);
                    if (spaceIndex > 0)
                    {
                        name = str.Substring(0, spaceIndex);

                        var startIndex = str.IndexOf("(v:", StringComparison.Ordinal);
                        if (startIndex > 0)
                        {
                            var verEndIndex = str.IndexOf(')', startIndex);
                            version = str.Substring(Math.Min(startIndex + 3, str.Length - 1), Math.Max(verEndIndex - startIndex - 3, 0));
                            if (version.Length == 0) version = null;
                        }
                        isGlobal = str.Substring(spaceIndex).Contains("*global*");
                    }
                    else
                    {
                        name = str;
                    }

                    // Make sure that this isn't just a corrupted json export
                    if (string.Equals(name, "\"apps\":", StringComparison.Ordinal) ||
                        string.Equals(name, "\"buckets\":", StringComparison.Ordinal))
                        throw new InvalidDataException("Scoop export is in unkown or invalid format! Try updating Scoop and try again.\n\nContents:\n" + result, e);

                    var entry = CreateUninstallerEntry(name, version, isGlobal, exeSearcher);

                    results.Add(entry);
                }
            }

            return results;
        }

        private sealed class AppInstall
        {
            public string Bucket { get; set; }
            public string Architecture { get; set; }
        }
        private sealed class AppManifest
        {
            public string Homepage { get; set; }
            [JsonPropertyName("env_add_path"), JsonConverter(typeof(DynamicStringArrayConverter))]
            public string[] EnvAddPath { get; set; }
            [JsonConverter(typeof(DynamicStringArrayConverter))]
            public string[] Bin { get; set; }
            public string[][] Shortcuts { get; set; }
            public IDictionary<string, AppManifestArchitecture> Architecture { get; set; }
        }
        private sealed class AppManifestArchitecture
        {
            public string[] EnvAddPath { get; set; }
            [JsonConverter(typeof(DynamicStringArrayConverter))]
            public string[] Bin { get; set; }
            public string[][] Shortcuts { get; set; }
        }

        public static ApplicationUninstallerEntry CreateUninstallerEntry(
            string name,
            string version,
            bool isGlobal,
            AppExecutablesSearcher searcher)
        {
            var entry = new ApplicationUninstallerEntry
            {
                RawDisplayName = name,
                DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(version),
                RatingId = "Scoop " + name
            };

            var installDir = Path.Combine(isGlobal ? _scoopGlobalPath : _scoopUserPath, "apps\\" + name);
            if (Directory.Exists(installDir))
            {
                List<string> executables = new();
                var currentDir = Path.Combine(installDir, "current");

                try
                {
                    var install = JsonSerializer.Deserialize(
                        File.ReadAllText(Path.Combine(currentDir, "install.json")),
                        _jsonContext.AppInstall);

                    var manifest = JsonSerializer.Deserialize(
                        File.ReadAllText(Path.Combine(currentDir, "manifest.json")),
                        _jsonContext.AppManifest);

                    entry.AboutUrl = manifest.Homepage;

                    var shortcuts = manifest.Architecture?[install.Architecture]?.Shortcuts ?? manifest.Shortcuts;
                    if (shortcuts != null)
                    {
                        var files = shortcuts.Select(x => Path.Combine(currentDir, x[0]))
                                             .Where(File.Exists)
                                             .Select(Path.GetFullPath)
                                             .Distinct(StringComparer.OrdinalIgnoreCase)
                                             .ToList();

                        executables.AddRange(files.Where(x => x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)).Concat(files.Where(x => x.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase))));

                        var potentialIcons = files.Where(x => x.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)).Concat(files.Where(x => x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))).ToList();
                        foreach (var potentialIcon in potentialIcons)
                        {
                            try
                            {
                                var icon = potentialIcon.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ? new Icon(potentialIcon) : Icon.ExtractAssociatedIcon(potentialIcon);
                                if (icon == null || icon.Size == Size.Empty) continue;
                                entry.IconBitmap = icon;
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($@"Failed to get icon for [{name}] from [{potentialIcon}] - {ex}");
                            }
                        }
                    }

                    var bin = manifest.Architecture?[install.Architecture]?.Bin ?? manifest.Bin;
                    if (bin != null)
                    {
                        var filteredBins = bin.Select(x => Path.Combine(installDir, "current", x))
                                              .Where(File.Exists)
                                              .Select(Path.GetFullPath)
                                              .Except(executables, StringComparer.OrdinalIgnoreCase)
                                              .Where(x => x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase))
                                              .ToList();
                        executables.AddRange(filteredBins);
                    }

                    var env = manifest.Architecture?[install.Architecture]?.EnvAddPath ?? manifest.EnvAddPath;
                    if (env is { Length: > 0 })
                    {
                        currentDir = Path.Combine(currentDir, env[0]);
                    }
                }
                catch (IOException)
                { }
                catch (UnauthorizedAccessException)
                { }
                catch (JsonException)
                { }

                if (executables.Any())
                {
                    // No need to sort, safe to assume the manifest has the most important executables in first positions
                    entry.SortedExecutables = executables.ToArray();
                }
                else
                {
                    // Avoid looking for executables in old versions
                    entry.InstallLocation = currentDir;
                    searcher.AddMissingInformation(entry);
                }

                entry.InstallLocation = installDir;
            }

            entry.UninstallerKind = UninstallerType.PowerShell;
            entry.UninstallString = MakeScoopCommand("uninstall " + name + (isGlobal ? " --global" : "")).ToString();

            return entry;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanScoop;
        public string DisplayName => Localisation.Progress_AppStores_Scoop;

        private static string RunScoopCommand(string scoopArgs)
        {
            var startInfo = MakeScoopCommand(scoopArgs).ToProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.StandardOutputEncoding = Encoding.Default;

            using (var process = Process.Start(startInfo))
            {
                var sw = Stopwatch.StartNew();
                var output = process?.StandardOutput.ReadToEnd();
                Trace.WriteLine($"[Performance] Running command {startInfo.FileName} {startInfo.Arguments} took {sw.ElapsedMilliseconds}ms");
                return output;
            }
        }

        private static ProcessStartCommand MakeScoopCommand(string scoopArgs)
        {
            return new ProcessStartCommand(_powershellPath, $"-NoProfile -ex unrestricted \"{_scriptPath}\" {scoopArgs}");
        }


        [JsonSerializable(typeof(ExportInfo))]
        [JsonSerializable(typeof(AppInstall))]
        [JsonSerializable(typeof(AppManifest))]
        private sealed partial class JsonContext : JsonSerializerContext
        { }
    }
}

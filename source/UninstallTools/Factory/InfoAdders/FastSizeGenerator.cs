/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Klocman.Extensions;
using Klocman.IO;
using Scripting;
using File = System.IO.File;

namespace UninstallTools.Factory.InfoAdders
{
    public class FastSizeGenerator : IMissingInfoAdder
    {
        private static readonly FileSystemObjectClass _fileSystemObject;
        private static bool _everythingAvailable;

        static FastSizeGenerator()
        {
            try
            {
                _fileSystemObject = new FileSystemObjectClass();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"FastSizeGenerator: Scripting.FileSystemObjectClass is not available - " + ex.Message);
            }

            try
            {
                if (EvGetSize(UninstallToolsGlobalConfig.AssemblyLocation).GetKbSize() == 0)
                    throw new SystemException("Test failed to get valid BCU directory size");

                _everythingAvailable = true;
            }
            catch (SystemException ex)
            {
                _everythingAvailable = false;
                Trace.WriteLine(@"FastSizeGenerator: Everything search engine is not available - " + ex.Message);
            }
        }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (!Directory.Exists(target.InstallLocation) || UninstallToolsGlobalConfig.IsSystemDirectory(target.InstallLocation))
                return;

            if (_everythingAvailable)
            {
                try
                {
                    target.EstimatedSize = EvGetSize(target.InstallLocation);
                    return;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    _everythingAvailable = false;
                }
            }

            if (_fileSystemObject != null)
            {
                try
                {
                    var folder = _fileSystemObject.GetFolder(target.InstallLocation);
                    var size = new FileSize(Convert.ToInt64(folder.Size) / 1024);
                    target.EstimatedSize = size;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        private static FileSize EvGetSize(string path)
        {
            path = Path.GetFullPath(path);
            var output = StartHelperAndReadOutput($"-size -a-d -size-leading-zero -no-digit-grouping -size-format 1 path:\"{path}\"").Result;
            var allResults = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);

            long sum = 0;
            foreach (var result in allResults)
            {
                var split = result.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                sum += long.Parse(split[0]);
            }
            return FileSize.FromBytes(sum);
        }

        private static async Task<string> StartHelperAndReadOutput(string args)
        {
            var esPath = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, "es.exe");
            if (!File.Exists(esPath)) throw new FileNotFoundException();

            using (var process = Process.Start(new ProcessStartInfo(esPath, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            }))
            {
                if (process == null) throw new InvalidOperationException("Could not start a new process");

                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(40));
                var readOutputTask = process.StandardOutput.ReadToEndAsync();

                await Task.WhenAny(readOutputTask, timeoutTask);

                if (!readOutputTask.IsCompleted)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // Ignore exceptions from killing the process
                    }
                    throw new TimeoutException("es.exe appears to have hung");
                }

                if (process.ExitCode == 0) return await readOutputTask;
                throw new IOException("es.exe failed to connect to Everything", process.ExitCode);
            }
        }

        public string[] RequiredValueNames { get; } = { nameof(ApplicationUninstallerEntry.InstallLocation) };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } = { nameof(ApplicationUninstallerEntry.EstimatedSize) };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using Klocman.Extensions;

namespace Klocman.Tools
{
    public static class FilesystemTools
    {
        /// <summary>
        /// Check the architecture of the executable. E.g. 64bit.
        /// Returns Unknown if the architecture is unsupported or not specified.
        /// </summary>
        /// <param name="filename">Full path to the executable file.</param>
        public static MachineType CheckExecutableMachineType(string filename)
        {
            if (!filename.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new IOException("Not a Windows .exe file.");
            }

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                stream.Position = 0x3c;
                var fileData = new byte[1024];

                var bytesRead = stream.Read(fileData, 0, 1024);

                for (var i = 0; i < bytesRead; i++)
                {
                    // Look for the PE signature (PE\0\0)
                    if (i + 5 >= bytesRead) break;
                    if (fileData[i] != 0x50) continue;
                    if (fileData[i + 1] != 0x45 || fileData[i + 2] != 0 || fileData[i + 3] != 0) continue;

                    // Join two bytes representing the architecture
                    var machineId = fileData[i + 5] << 8 | fileData[i + 4];
                    switch (machineId)
                    {
                        case 0x8664:
                            return MachineType.X64;
                        case 0x14c:
                            return MachineType.X86;
                        case 0x200:
                            return MachineType.Ia64;
                        case 0xaa64:
                        case 0xA641:
                        case 0xA64E:
                            return MachineType.ARM64;
                        default:
                            return MachineType.Unknown;
                    }
                }
            }

            return MachineType.Unknown;
        }

        public static void CopyRecursive(string sourcePath, string targetPath)
        {
            CopyRecursive(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath));
        }

        public static void CopyRecursive(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (var fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyRecursive(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static bool CreateSymlink(string symlinkFileName, string targetFileName, SymbolicLinkType type)
        {
            return CreateSymbolicLink(symlinkFileName, targetFileName, type) != 0;
        }

        public static void MoveDirectory(string sourcePath, string targetPath)
        {
            MoveDirectory(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath));
        }

        public static void MoveDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.RootEquals(target))
                Directory.Move(source.FullName, target.FullName);
            else
            {
                CopyRecursive(source, target);
                source.Delete(true);
            }
        }

        public static void CompressDirectory(string dirFullName) => CompressDirectory(dirFullName, ManagementOptions.InfiniteTimeout);
        public static void CompressDirectory(string dirFullName, TimeSpan timeout)
        {
            var objPath = "Win32_Directory.Name=" + "\"" + dirFullName.Replace(@"\", @"\\") + "\"";
            using (var dir = new ManagementObject(objPath))
            {
                var outParams = dir.InvokeMethod("Compress", null, new InvokeMethodOptions { Timeout = timeout });
                if (outParams == null) throw new ArgumentNullException(nameof(outParams));
                var ret = (uint)outParams.Properties["ReturnValue"].Value;
                if (ret != 0)
                    throw new IOException("Win32_Directory.Compress returned " + ret);
            }
        }

        [DllImport("shlwapi.dll")]
        public static extern bool PathIsNetworkPath(string pszPath);

        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
        private static extern int CreateSymbolicLink([In] string lpSymlinkFileName, [In] string lpTargetFileName,
            SymbolicLinkType dwFlags);
    }
}
/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Process Handle & File Lock Resolution Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public sealed class LockingProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string ApplicationDescription { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string LockedFilePath { get; set; } = string.Empty;
    }

    public static class ProcessHandleUnlockerEngine
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        private const int CCH_RM_MAX_APP_NAME = 255;
        private const int CCH_RM_MAX_SVC_NAME = 63;

        private enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;
            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);

        public static List<LockingProcessInfo> FindLockingProcesses(string targetPath)
        {
            var results = new List<LockingProcessInfo>();
            if (string.IsNullOrWhiteSpace(targetPath)) return results;

            var targets = new List<string>();
            if (File.Exists(targetPath))
            {
                targets.Add(targetPath);
            }
            else if (Directory.Exists(targetPath))
            {
                try
                {
                    targets.AddRange(Directory.EnumerateFiles(targetPath, "*", SearchOption.AllDirectories).Take(20));
                }
                catch { }
            }

            if (targets.Count == 0) return results;

            uint sessionHandle = 0;
            var sessionKey = Guid.NewGuid().ToString();

            try
            {
                var res = RmStartSession(out sessionHandle, 0, sessionKey);
                if (res != 0) return results;

                var resources = targets.ToArray();
                res = RmRegisterResources(sessionHandle, (uint)resources.Length, resources, 0, null, 0, null);
                if (res != 0) return results;

                uint procInfoNeeded = 0;
                uint procInfoCount = 0;
                uint rebootReasons = 0;

                res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfoCount, null, ref rebootReasons);
                if (procInfoNeeded > 0)
                {
                    var procInfos = new RM_PROCESS_INFO[procInfoNeeded];
                    procInfoCount = procInfoNeeded;
                    res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfoCount, procInfos, ref rebootReasons);

                    if (res == 0)
                    {
                        for (int i = 0; i < procInfoCount; i++)
                        {
                            var pid = procInfos[i].Process.dwProcessId;
                            var appName = procInfos[i].strAppName;
                            string procName = appName;
                            string exePath = string.Empty;

                            try
                            {
                                var p = Process.GetProcessById(pid);
                                procName = p.ProcessName;
                                exePath = p.MainModule?.FileName ?? string.Empty;
                            }
                            catch { }

                            results.Add(new LockingProcessInfo
                            {
                                ProcessId = pid,
                                ProcessName = procName,
                                ApplicationDescription = appName,
                                ExecutablePath = exePath,
                                LockedFilePath = targetPath
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "RestartManager query failed", ex.Message);
            }
            finally
            {
                if (sessionHandle != 0)
                {
                    RmEndSession(sessionHandle);
                }
            }

            return results;
        }

        public static bool TerminateLockingProcess(int processId)
        {
            try
            {
                var proc = Process.GetProcessById(processId);
                proc.Kill();
                proc.WaitForExit(3000);
                StructuredLogger.Info(LogCategory.General, $"Terminated locking process PID={processId}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed terminating process PID={processId}", ex.Message);
                return false;
            }
        }
    }
}

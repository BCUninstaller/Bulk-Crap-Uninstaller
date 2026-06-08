/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Klocman
{
    internal sealed class LogWriter(string path) : StreamWriter(path, true, Encoding.UTF8)
    {
        private static LogWriter _currentLogger;

        public static void WriteExceptionToLog(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            WriteMessageToLog(ex.ToString());
        }

        /// <summary>
        /// Writes a message to the log file with a UTC timestamp.
        /// </summary>
        public static void WriteMessageToLog(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            StreamWriter writer = _currentLogger;
            try
            {
                var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());

                if (writer == null || !writer.BaseStream.CanWrite)
                    writer = new StreamWriter(location, true);

                writer.Write(DateTime.UtcNow.ToLongTimeString());
                writer.Write(" - ");
                writer.WriteLine(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to write to log file:\n" + ex);
            }
            finally
            {
                if (writer != null && writer != _currentLogger)
                    writer.Dispose();
            }
        }

        private static string CreateLogFilenameForAssembly(Assembly assembly)
        {
            var location = assembly.Location;
            if (location.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || location.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                location = location.Remove(location.Length - 4);
            location += ".log";
            return location;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Disposed = true;
        }

        private bool Disposed { get; set; }

        /// <summary>
        /// Start logging to a file reflecting the calling assembly name.
        /// Hooks console out and error. Dispose before exiting.
        /// If logging is already active, it will be restarted with the new calling assembly name.
        /// </summary>
        public static LogWriter StartLogging()
        {
            _currentLogger?.Dispose();

            var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());
            return _currentLogger = StartLogging(location);
        }

        /// <summary>
        /// Start logging to a file.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        private static LogWriter StartLogging(string logPath)
        {
            try
            {
                // Limit log size to 100 kb
                var fileInfo = new FileInfo(logPath);
                if (fileInfo.Exists && fileInfo.Length > 1024 * 100)
                    fileInfo.Delete();

                // Create new log writer
                var logWriter = new LogWriter(logPath);

                // Make sure we can write to the file
                logWriter.WriteSeparator();
                logWriter.WriteLine("Application startup");
                logWriter.Flush();

                Console.SetOut(logWriter);
                Console.SetError(logWriter);

                Trace.Listeners.Add(new TextWriterTraceListener(logWriter));

                return logWriter;
            }
            catch (Exception ex)
            {
                // Ignore logging errors
                Console.WriteLine(ex);
                return null;
            }
        }

        public void WriteSeparator()
        {
            if (Disposed) return;
            base.WriteLine("--------------------------------------------------");
        }

        public override void WriteLine(string value)
        {
            if (Disposed) return;
            value = DateTime.UtcNow.ToLongTimeString() + " - " + value;
            base.WriteLine(value);
            base.Flush();
        }
    }
}
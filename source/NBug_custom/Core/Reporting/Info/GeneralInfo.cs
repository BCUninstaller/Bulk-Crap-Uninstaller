// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralInfo.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Reporting.Info
{
    [Serializable]
    public class GeneralInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeneralInfo" /> class. This is the default constructor provided for
        ///     XML
        ///     serialization and de-serialization.
        /// </summary>
        public GeneralInfo()
        {
        }

        internal GeneralInfo(SerializableException serializableException)
        {
            // this.HostApplication = Settings.EntryAssembly.GetName().Name; // Does not get the extensions of the file!
            HostApplication = Settings.EntryAssembly.GetLoadedModules()[0].Name;

            // this.HostApplicationVersion = Settings.EntryAssembly.GetName().Version.ToString(); // Gets AssemblyVersion not AssemblyFileVersion
            HostApplicationVersion = NBugVersion = FileVersionInfo.GetVersionInfo(Settings.EntryAssembly.Location).FileVersion;

            // this.NBugVersion = System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString(); // Gets AssemblyVersion not AssemblyFileVersion
            NBugVersion = FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).FileVersion;

            CLRVersion = Environment.Version.ToString();

            DateTime = System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            if (serializableException != null)
            {
                ExceptionType = serializableException.Type;

                if (!string.IsNullOrEmpty(serializableException.TargetSite))
                {
                    TargetSite = serializableException.TargetSite;
                }
                else if (serializableException.InnerException != null &&
                         !string.IsNullOrEmpty(serializableException.InnerException.TargetSite))
                {
                    TargetSite = serializableException.InnerException.TargetSite;
                }

                ExceptionMessage = serializableException.Message;
            }
        }

        public string CLRVersion { get; set; }
        public string DateTime { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionType { get; set; }
        public string HostApplication { get; set; }

        /// <summary>
        ///     Gets or sets AssemblyFileVersion of host assembly.
        /// </summary>
        public string HostApplicationVersion { get; set; }

        /// <summary>
        ///     Gets or sets AssemblyFileVersion of NBug.dll assembly.
        /// </summary>
        public string NBugVersion { get; set; }

        public string TargetSite { get; set; }
        public string UserDescription { get; set; }
    }
}
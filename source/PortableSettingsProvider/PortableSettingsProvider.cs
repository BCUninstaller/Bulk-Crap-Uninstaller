using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace PortableSettingsProvider
{
    /// <summary>
    /// License: The Code Project Open License (CPOL) 1.02
    /// 18 Oct 2007 - CodeChimp - Public VB release
    /// Mar 26, 2016 - HakanL - Converted project to C#, cleanup
    /// 2010-2018 - Marcin Szeniak - Bugfixes, cleanup, improvements
    /// </summary>
    public class PortableSettingsProvider : SettingsProvider
    {
        //XML Root Node name
        private const string SettingsRootName = "Settings";
        
        public static string ApplicationNameOverride { get; set; }
        public override string ApplicationName
        {
            get => ApplicationNameOverride ?? Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            set { }
        }

        private XmlDocument _settingsXml;
        private XmlNode _settingsRootNode;
        private readonly string _localMachineName;

        public PortableSettingsProvider()
        {
            var machineName = Environment.MachineName;
            if (string.IsNullOrEmpty(machineName))
                machineName = $"{Environment.UserDomainName}-{Environment.UserName}";
            _localMachineName = XmlConvert.EncodeName(machineName);
        }

        private XmlDocument SettingsXml
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                if (_settingsXml == null)
                {
                    _settingsXml = new XmlDocument();

                    try
                    {
                        _settingsXml.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));

                        if (_settingsXml.SelectSingleNode(SettingsRootName) == null)
                            throw new Exception("Invalid config file");
                    }
                    catch (Exception)
                    {
                        //Create new document
                        var dec = _settingsXml.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        _settingsXml.AppendChild(dec);

                        var nodeRoot = _settingsXml.CreateNode(XmlNodeType.Element, SettingsRootName, "");
                        _settingsXml.AppendChild(nodeRoot);
                    }
                }

                return _settingsXml;
            }
        }

        private XmlNode SettingsRootNode
        {
            get
            {
                if (_settingsRootNode == null)
                    _settingsRootNode = SettingsXml.SelectSingleNode(SettingsRootName);
                return _settingsRootNode;
            }
        }

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(ApplicationName, col);
        }
        public static string AppSettingsPathOverride { get; set; }
        public virtual string GetAppSettingsPath()
        {
            if (Directory.Exists(AppSettingsPathOverride)) return AppSettingsPathOverride;
            //Used to determine where to store the settings
            var fi = new FileInfo(Application.ExecutablePath);
            return fi.DirectoryName;
        }

        public virtual string GetAppSettingsFilename()
        {
            //Used to determine the filename to store the settings
            return ApplicationName + ".settings";
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXml.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save settings - " + ex);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection props)
        {
            //Create new collection of values
            var values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved

            foreach (SettingsProperty setting in props)
            {
                var value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        private string GetValue(SettingsProperty setting)
        {
            try
            {
                if (IsRoaming(setting))
                {
                    return SettingsXml.SelectSingleNode(SettingsRootName + "/" + setting.Name)?.InnerText
                           ?? GetDefaultValue(setting);
                }

                return SettingsXml.SelectSingleNode(SettingsRootName + "/" + _localMachineName
                                                    + "/" + setting.Name)?.InnerText ?? GetDefaultValue(setting);
            }
            catch (Exception)
            {
                return GetDefaultValue(setting);
            }
        }

        private static string GetDefaultValue(SettingsProperty setting)
        {
            return setting.DefaultValue?.ToString() ?? string.Empty;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            if (propVal == null)
                throw new ArgumentNullException(nameof(propVal));
            if (propVal.SerializedValue == null)
                throw new ArgumentNullException(nameof(propVal.SerializedValue));

            XmlElement settingNode;

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                if (IsRoaming(propVal.Property))
                {
                    settingNode = (XmlElement)SettingsXml.SelectSingleNode(
                        SettingsRootName + "/" + propVal.Name);
                }
                else
                {
                    settingNode = (XmlElement)SettingsXml.SelectSingleNode(
                        SettingsRootName + "/" + _localMachineName + "/" + propVal.Name);
                }
            }
            catch (Exception)
            {
                settingNode = null;
            }

            if (settingNode != null)
            {
                settingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                if (IsRoaming(propVal.Property))
                {
                    CreateRoamingValue(propVal);
                }
                else
                {
                    CreateLocalValue(propVal);
                }
            }
        }

        /// <summary>
        /// Its machine specific, store as an element of the machine name node,
        /// creating a new machine name node if one doesnt exist.
        /// </summary>
        private void CreateLocalValue(SettingsPropertyValue propVal)
        {
            XmlElement machineNode;
            try
            {
                machineNode = (XmlElement)SettingsXml.SelectSingleNode(
                    SettingsRootName + "/" + _localMachineName);
            }
            catch (Exception)
            {
                machineNode = SettingsXml.CreateElement(_localMachineName);
                SettingsRootNode.AppendChild(machineNode);
            }

            if (machineNode == null)
            {
                machineNode = SettingsXml.CreateElement(_localMachineName);
                SettingsRootNode.AppendChild(machineNode);
            }

            var settingNode = SettingsXml.CreateElement(propVal.Name);
            settingNode.InnerText = propVal.SerializedValue.ToString();
            machineNode.AppendChild(settingNode);
        }

        /// <summary>
        /// Store the value as an element of the Settings Root Node
        /// </summary>
        private void CreateRoamingValue(SettingsPropertyValue propVal)
        {
            var serializedValue = propVal.SerializedValue;
            if (serializedValue != null)
            {
                if (SettingsRootNode == null) throw new ArgumentNullException(nameof(SettingsRootNode));
                if (SettingsXml == null) throw new ArgumentNullException(nameof(SettingsXml));
                var settingNode = SettingsXml.CreateElement(propVal.Name);
                settingNode.InnerText = serializedValue.ToString();
                SettingsRootNode.AppendChild(settingNode);
            }
        }

        /// <summary>
        /// Determine if the setting is marked as Roaming
        /// </summary>
        private bool IsRoaming(SettingsProperty prop)
        {
            try
            {
                return prop.Attributes.Cast<DictionaryEntry>().Select(x => x.Value)
                    .OfType<SettingsManageabilityAttribute>()
                    .Any(x => x.Manageability == SettingsManageability.Roaming);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
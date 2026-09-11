/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UninstallTools.Lists
{
    public class UninstallList : ITestEntry
    {
        private static readonly XmlReaderSettings ReaderSettings = new()
        {
            CloseInput = false,
            DtdProcessing = DtdProcessing.Prohibit,
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        public UninstallList()
        {
        }

        public UninstallList(IEnumerable<Filter> items)
        {
            AddItems(items);
        }

        public List<Filter> Filters { get; set; } = new();

        /// <summary>
        ///     Test if the input matches this filter. Returns null if it did not hit any filter.
        ///     If there are only exclude filters this assumes that everything is included.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (!Enabled || input == null || Filters.Count < 1) return null;

            var excludes = new List<Filter>();
            var includes = new List<Filter>();

            foreach (var uninstallListItem in Filters)
            {
                if (uninstallListItem.Exclude)
                    excludes.Add(uninstallListItem);
                else
                    includes.Add(uninstallListItem);
            }

            bool? excluded = null;

            foreach (var uninstallListItem in excludes)
            {
                if (uninstallListItem.TestEntry(input) == true)
                    return false;
                excluded = false;
            }

            bool? included = null;

            foreach (var uninstallListItem in includes)
            {
                if (uninstallListItem.TestEntry(input) == true)
                {
                    included = true;
                    break;
                }
                included = false;
            }

            if (!included.HasValue)
                return excluded.HasValue ? true : null;
            return included.Value;
        }

        public bool Enabled { get; set; } = true;

        public static UninstallList ReadFromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            var serializer = new XmlSerializer(typeof (UninstallList));
            using (var stream = File.OpenRead(fileName))
            {
                if (stream.Length == 0)
                    throw new InvalidDataException("The uninstall list file is empty.");

                try
                {
                    return Deserialize(serializer, XmlReader.Create(stream, ReaderSettings));
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is XmlException)
                {
                    if (!stream.CanSeek)
                        throw CreateInvalidDataException(ex);

                    stream.Position = 0;
                    using var textReader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
                    var trimmedContents = textReader.ReadToEnd().TrimStart('\uFEFF', '\u200B', ' ', '\t', '\r', '\n');
                    if (trimmedContents.Length == 0)
                        throw CreateInvalidDataException(ex);

                    using var stringReader = new StringReader(trimmedContents);
                    try
                    {
                        return Deserialize(serializer, XmlReader.Create(stringReader, ReaderSettings));
                    }
                    catch (Exception retryEx) when (retryEx is InvalidOperationException || retryEx is XmlException)
                    {
                        throw CreateInvalidDataException(retryEx);
                    }
                }
            }
        }

        public void AddItems(IEnumerable<string> items)
        {
            Filters.AddRange(items.Select(x => new Filter(null, x)));
        }

        public void Add(Filter item)
        {
            Filters.Add(item);
        }

        public void AddItems(IEnumerable<Filter> items)
        {
            Filters.AddRange(items);
        }

        public void SaveToFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof (UninstallList));
            using (var writer = new XmlTextWriter(fileName, Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, this);
            }
        }

        internal void Remove(Filter item)
        {
            if (Filters.Contains(item))
                Filters.Remove(item);
        }

        private static UninstallList Deserialize(XmlSerializer serializer, XmlReader reader)
        {
            using (reader)
            {
                reader.MoveToContent();
                if (reader.NodeType != XmlNodeType.Element ||
                    !string.Equals(reader.LocalName, nameof(UninstallList), StringComparison.Ordinal))
                {
                    throw new InvalidDataException(
                        $"The file does not contain an uninstall list. Expected root element '{nameof(UninstallList)}' but found '{reader.LocalName}'.");
                }

                var result = serializer.Deserialize(reader) as UninstallList;
                if (result == null)
                    throw new InvalidDataException("The uninstall list file could not be deserialized.");

                result.Filters ??= new List<Filter>();
                return result;
            }
        }

        private static InvalidDataException CreateInvalidDataException(Exception ex)
        {
            var xmlException = ex as XmlException ?? ex.InnerException as XmlException;
            if (xmlException != null)
            {
                return new InvalidDataException(
                    $"The uninstall list XML is invalid at line {xmlException.LineNumber}, position {xmlException.LinePosition}: {xmlException.Message}",
                    ex);
            }

            return new InvalidDataException("The uninstall list !(file is valid XML).", ex);
        }
    }
}

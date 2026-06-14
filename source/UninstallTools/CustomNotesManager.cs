using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Tools;

namespace UninstallTools
{
    public static class CustomNotesManager
    {
        private static readonly string NotesFilePath = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, "CustomNotes.xml");
        private static Dictionary<string, string> _notesCache = new Dictionary<string, string>();

        static CustomNotesManager()
        {
            if (File.Exists(NotesFilePath))
            {
                try
                {
                    var loaded = SerializationTools.DeserializeFromXml<List<NoteEntry>>(NotesFilePath);
                    if (loaded != null)
                    {
                        _notesCache = loaded
                            .Where(x => !string.IsNullOrEmpty(x.CacheId) && !string.IsNullOrEmpty(x.Note))
                            .ToDictionary(x => x.CacheId, x => x.Note);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load CustomNotes.xml: " + ex.Message);
                }
            }
        }

        public static string GetNote(string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId)) return string.Empty;
            return _notesCache.TryGetValue(cacheId, out var note) ? note : string.Empty;
        }

        public static void SetNote(string cacheId, string note)
        {
            if (string.IsNullOrEmpty(cacheId)) return;

            if (string.IsNullOrEmpty(note))
                _notesCache.Remove(cacheId);
            else
                _notesCache[cacheId] = note;

            Save();
        }

        private static void Save()
        {
            try
            {
                var listToSave = _notesCache.Select(kv => new NoteEntry { CacheId = kv.Key, Note = kv.Value }).ToList();
                SerializationTools.SerializeToXml(NotesFilePath, listToSave);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save CustomNotes.xml: " + ex.Message);
            }
        }

        public class NoteEntry
        {
            public string CacheId { get; set; }
            public string Note { get; set; }
        }
    }
}

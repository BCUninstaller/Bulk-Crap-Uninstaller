/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    public sealed class CertCacheEntry
    {
        public bool Valid { get; set; }

        [XmlIgnore]
        public X509Certificate2 Cert { get; set; }
        public byte[] CertData // ReadOnlySpan<byte> crashes XML serializer in new versions of .NET
        {
            get => Cert?.RawData;
            set => Cert = value == null ? null : new X509Certificate2(value);
        }
    }

    internal class CertificateCache
    {
        public string CacheFilename { get; }
        private IDictionary<string, CertCacheEntry> _dictionaryCache;
        private readonly object _cacheLock = new object();

        public CertificateCache(string cacheFilename)
        {
            CacheFilename = cacheFilename ?? throw new ArgumentNullException(nameof(cacheFilename));
        }

        public void LoadCertificateCache()
        {
            lock (_cacheLock)
            {
                _dictionaryCache = null;
                try
                {
                    if (File.Exists(CacheFilename))
                    {
                        _dictionaryCache = SerializationTools.DeserializeDictionary<string, CertCacheEntry>(CacheFilename);
                        // Remove invalid entries in case the xml is corrupted
                        _dictionaryCache?.Where(x => x.Value == null).ToList().ForEach(pair => _dictionaryCache.Remove(pair.Key));
                    }
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                    ClearChache();
                }
            }
        }

        public void SaveCertificateCache()
        {
            lock (_cacheLock)
            {
                if (_dictionaryCache == null || _dictionaryCache.Count == 0)
                {
                    ClearChache();
                    return;
                }

                try
                {
                    SerializationTools.SerializeDictionary(_dictionaryCache, CacheFilename);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                    ClearChache();
                }
            }
        }

        public void ClearChache()
        {
            lock (_cacheLock)
            {
                _dictionaryCache = null;
                try
                {
                    File.Delete(CacheFilename);
                }
                catch
                {
                    System.Threading.Thread.Sleep(50);
                    File.Delete(CacheFilename);
                }
            }
        }

        public void SetItem(string id, X509Certificate2 cert, bool verified)
        {
            lock (_cacheLock)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (_dictionaryCache == null) _dictionaryCache = new Dictionary<string, CertCacheEntry>();

                    _dictionaryCache[id] = new CertCacheEntry { Cert = cert, Valid = verified };
                }
            }
        }

        public bool TryGetCachedItem(string id, out CertCacheEntry entry)
        {
            lock (_cacheLock)
            {
                if (_dictionaryCache == null || string.IsNullOrEmpty(id))
                {
                    entry = null;
                    return false;
                }

                return _dictionaryCache.TryGetValue(id, out entry);
            }
        }
    }
}
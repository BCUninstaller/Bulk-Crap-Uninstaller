/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Events;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class UninstallerListPostProcesser : IDisposable
    {
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private bool _abortPostprocessingThread;
        private Thread _finalizerThread;
        private readonly Queue<ApplicationUninstallerEntry> _itemsToProcess = new();
        private readonly List<object> _objectsToUpdate = new();
        private readonly Action<IList> _updateItemsCallback;

        /// <summary>
        /// External lock to the uninstall system.
        /// </summary>
        internal object UninstallerFileLock { get; set; } = new();

        private readonly CertificateCache _certificateCache;

        public event EventHandler<CountingUpdateEventArgs> UninstallerPostprocessingProgressUpdate;

        public UninstallerListPostProcesser(Action<IList> updateItemsCallback, CertificateCache certificateCache)
        {
            _updateItemsCallback = updateItemsCallback ?? throw new ArgumentNullException(nameof(updateItemsCallback));
            _certificateCache = certificateCache;
        }

        public void AbortPostprocessingThread()
        {
            _abortPostprocessingThread = true;
            lock (_itemsToProcess) _itemsToProcess.Clear();
        }

        public void StartProcessingThread(IEnumerable<ApplicationUninstallerEntry> itemsToProcess)
        {
            lock (_itemsToProcess)
            {
                foreach (var entry in itemsToProcess.Except(_itemsToProcess))
                    _itemsToProcess.Enqueue(entry);
            }

            if (_finalizerThread == null || !_finalizerThread.IsAlive)
            {
                _finalizerThread = new Thread(UninstallerPostprocessingThread)
                { Name = "UninstallerPostprocessingThread", IsBackground = true, Priority = ThreadPriority.Lowest };

                _abortPostprocessingThread = false;

                _finalizerThread.Start();
            }
        }

        public void StopProcessingThread()
        {
            lock (_itemsToProcess)
                _itemsToProcess.Clear();
        }

        private void UninstallerPostprocessingThread()
        {
            while (true)
            {
                int count;
                ApplicationUninstallerEntry target = null;
                lock (_itemsToProcess)
                {
                    count = _itemsToProcess.Count;
                    if (count > 0) target = _itemsToProcess.Dequeue();
                }

                if (count == 0 || _abortPostprocessingThread)
                {
                    _finalizerThread = null;
                    OnUninstallerPostprocessingProgressUpdate(new CountingUpdateEventArgs(0, 0, 0));
                    return;
                }

                var sendTag = true;
                if (_settings.Settings.AdvancedTestCertificates)
                {
                    lock (UninstallerFileLock)
                    {
                        var cert = GetCert(target);
                        sendTag = cert != null;
                    }
                }

                var countingUpdateEventArgs = new CountingUpdateEventArgs(0, count, 0);
                if (sendTag) countingUpdateEventArgs.Tag = target;

                OnUninstallerPostprocessingProgressUpdate(countingUpdateEventArgs);
            }
        }

        private X509Certificate2 GetCert(ApplicationUninstallerEntry uninstaller)
        {
            var id = uninstaller.GetCacheId();

            if (_certificateCache.TryGetCachedItem(id, out var cachedCert))
            {
                uninstaller.SetCertificate(cachedCert.Cert, cachedCert.Valid);
                return cachedCert.Cert;
            }
            else
            {
                var cert = uninstaller.GetCertificate();
                _certificateCache.SetItem(id, cert, uninstaller.IsCertificateValid(true) == true);
                return cert;
            }
        }

        public void Dispose()
        {
            StopProcessingThread();
        }

        protected virtual void OnUninstallerPostprocessingProgressUpdate(CountingUpdateEventArgs y)
        {
            lock (_objectsToUpdate)
            {
                if (y.Tag != null)
                    _objectsToUpdate.Add(y.Tag);

                if (y.Value == y.Maximum || _objectsToUpdate.Count % 35 == 0)
                {
                    try
                    {
                        _updateItemsCallback(_objectsToUpdate.ToList());
                    }
                    catch (SystemException ex)
                    {
                        // The list view got disposed before we could update it.
                        AbortPostprocessingThread();
                        Debug.Fail(ex.Message, ex.StackTrace);
                    }
                    _objectsToUpdate.Clear();
                }
            }

            UninstallerPostprocessingProgressUpdate?.Invoke(this, y);
        }
    }
}
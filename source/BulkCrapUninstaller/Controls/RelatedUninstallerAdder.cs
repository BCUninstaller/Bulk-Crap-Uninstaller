/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UninstallTools;

namespace BulkCrapUninstaller.Forms
{
    public partial class RelatedUninstallerAdder : UserControl
    {
        public RelatedUninstallerAdder()
        {
            InitializeComponent();

            olvColumnEnabled.Renderer = new Theming.ContrastCheckStateRenderer();

            olvColumnEnabled.AspectGetter = rowObject => ((RelatedApplicationEntry)rowObject).Enabled;
            olvColumnEnabled.AspectPutter = (rowObject, value) => ((RelatedApplicationEntry)rowObject).Enabled = (bool)value;

            olvColumnRelatedApps.AspectGetter = rowObject => string.Join(", ",
                ((RelatedApplicationEntry)rowObject).RelatedEntries.Select(x => x.DisplayName).ToArray());

            olvColumnName.AspectGetter = rowObject => ((RelatedApplicationEntry)rowObject).Entry.DisplayName;
        }

        private IEnumerable<RelatedApplicationEntry> Entries => (objectListView1.Objects ?? Enumerable.Empty<RelatedApplicationEntry>()).Cast<RelatedApplicationEntry>();

        public IEnumerable<ApplicationUninstallerEntry> GetResults()
        {
            return Entries.Where(x => x.Enabled).Select(x => x.Entry);
        }

        public void SetRelatedApps(IEnumerable<RelatedApplicationEntry> items)
        {
            objectListView1.SetObjects(items
                .OrderBy(x => x.RelatedEntries.FirstOrDefault()?.DisplayName ?? string.Empty)
                .ThenBy(x => x.Entry.DisplayName).ToList());
        }

        public sealed class RelatedApplicationEntry
        {
            public RelatedApplicationEntry(ApplicationUninstallerEntry entry,
                IEnumerable<ApplicationUninstallerEntry> relatedEntries)
            {
                Entry = entry;
                RelatedEntries = relatedEntries.OrderBy(x => x.DisplayName).ToList();
                Enabled = false;
            }

            private bool _enabled;

            public bool Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }

            public ApplicationUninstallerEntry Entry { get; }

            public IEnumerable<ApplicationUninstallerEntry> RelatedEntries { get; }
        }
    }
}
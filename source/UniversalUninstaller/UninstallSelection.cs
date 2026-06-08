using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms;
using Klocman.Tools;
using UniversalUninstaller.Properties;

namespace UniversalUninstaller
{
    public sealed partial class UninstallSelection : Form
    {
        public UninstallSelection(DirectoryInfo target)
        {
            InitializeComponent();
            targetList1.Populate(target);

            try
            {
                Icon = ProcessTools.GetIconFromEntryExe();
            }
            catch (Exception ex)
            {
                /* Fall back to a low quality icon */
                var handle = Resources.icon.GetHicon();
                Icon = Icon.FromHandle(handle);

                Console.WriteLine(ex);
                Klocman.LogWriter.WriteMessageToLog(ex.ToString());
            }

            Text = string.Format(Localisation.UninstallSelection_Title, target.Name);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var ex = LoadingDialog.ShowDialog(this, Localisation.UninstallSelection_DeleteProgress_Title,
                _ => Program.DeleteItems(targetList1.GetItemsToDelete().ToList()));

            if (ex != null)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.ToString(), Localisation.UninstallSelection_DeleteProgress_FailedTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                DeleteFailed = true;
            }

            Close();
        }

        public bool WasCancelled { get; private set; }
        public bool DeleteFailed { get; private set; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            WasCancelled = true;
            Close();
        }
    }
}

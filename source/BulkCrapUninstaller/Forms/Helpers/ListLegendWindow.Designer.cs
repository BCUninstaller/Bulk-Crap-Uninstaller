namespace BulkCrapUninstaller.Forms
{
    partial class ListLegendWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            opacityResetTimer = new System.Windows.Forms.Timer(components);
            listLegend1 = new BulkCrapUninstaller.Controls.ListLegend();
            SuspendLayout();
            // 
            // opacityResetTimer
            // 
            opacityResetTimer.Interval = 25;
            opacityResetTimer.Tick += opacityResetTimer_Tick;
            // 
            // listLegend1
            // 
            listLegend1.AutoSize = true;
            listLegend1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            listLegend1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listLegend1.Cursor = System.Windows.Forms.Cursors.Hand;
            listLegend1.Location = new System.Drawing.Point(0, 0);
            listLegend1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            listLegend1.MinimumSize = new System.Drawing.Size(184, 2);
            listLegend1.Name = "listLegend1";
            listLegend1.Size = new System.Drawing.Size(184, 163);
            listLegend1.TabIndex = 0;
            // 
            // ListLegendWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.Fuchsia;
            ClientSize = new System.Drawing.Size(350, 346);
            ControlBox = false;
            Controls.Add(listLegend1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ListLegendWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "ListLegendWindow";
            TransparencyKey = System.Drawing.Color.Fuchsia;
            EnabledChanged += ListLegendWindow_EnabledChanged;
            VisibleChanged += ListLegendWindow_VisibleChanged;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Controls.ListLegend listLegend1;
        private System.Windows.Forms.Timer opacityResetTimer;
    }
}
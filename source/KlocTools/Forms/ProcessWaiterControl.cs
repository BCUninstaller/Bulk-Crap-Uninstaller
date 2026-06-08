/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using Klocman.Tools;
using Timer = System.Timers.Timer;

namespace Klocman.Forms
{
    public partial class ProcessWaiterControl : UserControl
    {
        private readonly Timer _timer = new(600);
        private static readonly string DefaultImageKey = "Default";

        public void Initialize(int[] processIDs, bool processChildren)
        {
            treeView1.ShowRootLines = processChildren;

            SetNodes(processIDs, processChildren);
        }

        public bool ProcessesStillRunning => treeView1.Nodes.Count > 0;

        public void StartUpdating()
        {
            _timer.Start();
        }

        public ProcessWaiterControl()
        {
            InitializeComponent();

            _timer.Elapsed += updateTimer_Tick;
            _timer.AutoReset = false;
            _timer.SynchronizingObject = this;
        }

        private IEnumerable<Process> MainMonitoredProcesses
        {
            get { return treeView1.Nodes.Cast<TreeNode>().Select(x => x.Tag as Process); }
        }

        private void SetNodes(int[] processIDs, bool processChildren)
        {
            var results = new List<TreeNode>();
            var il = new ImageList();
            il.Images.Add(DefaultImageKey, SystemIcons.Application);
            treeView1.ImageKey = DefaultImageKey;

            foreach (var id in processIDs)
            {
                try
                {
                    var p = Process.GetProcessById(id);
                    if (p.HasExited) continue;

                    var mainPrName = string.IsNullOrEmpty(p.MainWindowTitle) ? p.ProcessName : p.MainWindowTitle;
                    var node = new TreeNode(mainPrName)
                    {
                        SelectedImageKey = mainPrName,
                        ImageKey = mainPrName,
                        Tag = p
                    };
                    results.Add(node);

                    try
                    {
                        var ico = DrawingTools.ExtractAssociatedIcon(p.MainModule!.FileName);
                        if (ico != null) il.Images.Add(mainPrName, ico);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    if (!processChildren) continue;
                    try
                    {
                        var children = ProcessTools.GetChildProcesses(id);
                        node.Nodes.AddRange(children.Select(x =>
                        {
                            var pr = Process.GetProcessById(x);
                            var name = string.IsNullOrEmpty(pr.MainWindowTitle)
                                ? pr.ProcessName
                                : pr.MainWindowTitle;
                            return new TreeNode(name)
                            {
                                SelectedImageKey = mainPrName,
                                ImageKey = mainPrName,
                                Tag = pr
                            };
                        }).ToArray());
                    }
                    catch (Exception ex)
                    {
                        // Ignore, probably the process exited by now. The child nodes are not important
                        Console.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    // Probably the main process exited, remove it from the task
                    Console.WriteLine(ex);
                }
            }

            treeView1.Nodes.Clear();
            var prev = treeView1.ImageList;
            treeView1.ImageList = il;
            prev?.Dispose();

            treeView1.Nodes.AddRange(results.ToArray());
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowIgnoreAndCancel
        {
            get
            {
                return buttonCancel.Enabled;
            }
            set
            {
                buttonIgnore.Enabled = value;
                buttonIgnore.Visible = value;
                buttonCancel.Enabled = value;
                buttonCancel.Visible = value;
                panel2c.Visible = value;
                panel4c.Visible = value;
            }
        }

        public event EventHandler AllProcessesClosed;
        public event EventHandler CancelClicked;

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            AllProcessesClosed?.Invoke(sender, e);
        }

        private void buttonKillAll_Click(object sender, EventArgs e)
        {
            // ask if sure and kill
            if (!PremadeDialogs.KillRunningProcessesQuestion())
                return;

            foreach (var id in MainMonitoredProcesses)
            {
                try
                {
                    id.Kill(false);
                }
                catch (Exception ex)
                {
                    // Ignore, probably the process exited by now
                    Console.WriteLine(ex);
                }
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            foreach (var node in treeView1.Nodes.Cast<TreeNode>().ToArray())
            {
                try
                {
                    if (node.Tag is not Process pr || pr.HasExited)
                    {
                        treeView1.Nodes.Remove(node);
                    }
                    else
                    {
                        foreach (var subNode in node.Nodes.Cast<TreeNode>().ToArray())
                        {
                            try
                            {
                                if (subNode.Tag is not Process spr || spr.HasExited)
                                    node.Nodes.Remove(subNode);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                node.Nodes.Remove(subNode);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    treeView1.Nodes.Remove(node);
                }
            }

            if (treeView1.Nodes.Count <= 0)
                AllProcessesClosed?.Invoke(sender, e);
            else
                _timer.Start();
        }

        private void buttonKill_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag is not Process process) return;
            try { process.Kill(); }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            buttonKill.Enabled = treeView1.SelectedNode != null;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CancelClicked?.Invoke(sender, e);
        }

        public void StopUpdating()
        {
            _timer.Stop();
        }
    }
}
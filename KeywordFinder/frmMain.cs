﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeywordFinder
{
    public partial class frmMain : Form
    {
        private delegate void ListViewAddNewItemCallback(ListViewItem lvi);
        private Process currentProcess;
        private ListViewItem currentItem;
        public frmMain()
        {
            InitializeComponent();
            currentProcess = new Process();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtTargePath.Text = dialog.SelectedPath;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            currentProcess = new Process();
            currentProcess.StartInfo.FileName = "findstr";
            currentProcess.StartInfo.Arguments = string.Format("/s /i /c:\"{0}\" \"{1}\"{2}", txtKeyword.Text, txtTargePath.Text, chkExtension.Checked ? string.Format("\\*.{0}", txtExtension.Text) : "\\*.*");
            currentProcess.StartInfo.UseShellExecute = false;
            currentProcess.StartInfo.CreateNoWindow = true;
            currentProcess.StartInfo.RedirectStandardInput = true;
            currentProcess.StartInfo.RedirectStandardOutput = true;
            currentProcess.StartInfo.RedirectStandardError = false;
            currentProcess.Start();
            currentProcess.BeginOutputReadLine();
            currentProcess.OutputDataReceived += OutputDataReceived;
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            if (!string.IsNullOrEmpty(data))
            {
                int index = GetIndex(data);
                int sublength = data.Length - index - 1;
                string[] dataToken = new string[2]
                {
                    data.Substring(0, index),
                    data.Substring(index +1,sublength)
                };
                if (dataToken != null)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = dataToken[0];
                    lvi.SubItems.Add(dataToken[1]);
                    ;
                    if (listView1.InvokeRequired)
                    {
                        ListViewAddNewItemCallback callback = new ListViewAddNewItemCallback(ListViewAddItem);
                        this.Invoke(callback, lvi);
                    }
                    else
                    {
                        listView1.Items.Add(lvi);
                    }
                }
            }
        }

        private int GetIndex(string str)
        {
            int count = 0;
            int index = 0;
            foreach(char c in str)
            {
                if(c== ':')
                {
                    if (count < 1)
                    {
                        count++;
                    }
                    else if(count ==1)
                    {
                        index++;
                        break;
                    }
                }
                
                else
                {
                    index++;
                }
            }
            return index;
        }

        private void ListViewAddItem(ListViewItem lvi)
        {
            listView1.Items.Add(lvi);
        }

        private void chkExtension_CheckedChanged(object sender, EventArgs e)
        {
            if(chkExtension.Checked)
            {
                txtExtension.Enabled = true;
            }
            else
            {
                txtExtension.Enabled = false;
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item != null)
            {
                currentItem = e.Item;
                btnOpen.Enabled = true;
            }
            else
            {
                btnOpen.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Process notepadProcess = new Process();
            notepadProcess.StartInfo.FileName = "notepad.exe";
            notepadProcess.StartInfo.Arguments = currentItem.Text;
            notepadProcess.StartInfo.CreateNoWindow = false;
            notepadProcess.Start();
        }
    }
}

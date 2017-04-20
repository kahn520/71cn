using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Application=Microsoft.Office.Interop.PowerPoint.Application;

namespace _71cn
{
    public partial class Form1 : Form
    {
        
        private Task task = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folder.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtPath.Text.Trim()))
            {
                return;
            }
            if (task == null || task.Status != TaskStatus.Running)
            {
                task = new Task(DoWork);
                task.Start();
            }
        }

        private void DoWork()
        {
            int i = 0;
            Application app = new Application();
            foreach (string id in txtID.Lines)
            {
                i++;
                lblMsg.Text = $"正在处理{id}（{i}/{txtID.Lines.Length}）";
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                    continue;
                DownLoader downLoader = new DownLoader(id, txtPath.Text);
                downLoader.StartDownload();
                if (downLoader.Files.Count > 0)
                {
                    Presentation ppt = null;
                    downLoader.Files.ForEach(f =>
                    {
                        if (ppt == null)
                        {
                            ppt = app.Presentations.Open(f);
                        }
                        else
                        {
                            ppt.Slides.InsertFromFile(f, ppt.Slides.Count + 1);
                            File.Delete(f);
                        }
                        
                    });
                    ppt.SaveAs($"{txtPath.Text.TrimEnd('\\')}\\{id}.pptx");
                    ppt.Close();
                    File.Delete(downLoader.Files[0]);
                }
            }
            lblMsg.Text = "完成";
        }
    }
}

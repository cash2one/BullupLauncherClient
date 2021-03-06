﻿using System;
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
using TCPLib;

namespace BullupVersionClient {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal) {
                // 移动窗体
                this.Capture = false;
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private String bullupPath = "";

        private TCPClient client;

        private void button1_Click(object sender, EventArgs e) {
            if (bullupPath == "") {
                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowDialog();
                bullupPath = folderDlg.SelectedPath;
                textBox1.Text = bullupPath;
                if (bullupPath != "") {
                    button1.Text = "开始安装/更新";
                }
            } else {
                if (button1.Text == "开始安装/更新") {
                    client = new TCPClient("18.220.98.48", 6001);
                    //执行Start方法
                    client.Start(bullupPath);
                    button1.Enabled = false;

                    Thread th = new Thread(ThreadChild);
                    th.Start();
                }
            }
        }


        protected void CreateShortcuts(String targetPath, String savePath, String saveName) {
            IWshRuntimeLibrary.IWshShell shell_class = new IWshRuntimeLibrary.IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut = null;
            //if (!Directory.Exists(targetPath))
            //    return;
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            try {
                shortcut = shell_class.CreateShortcut(savePath + @"/" + saveName + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
                shortcut.TargetPath = targetPath;
                shortcut.Save();
                //MessageBox.Show("创建快捷方式成功！");
            } catch (Exception ex) {
                //MessageBox.Show("创建快捷方式失败！");
            }
        } 

        private void ThreadChild() {
            while(true){
                try {
                    progressBar1.Maximum = client.maxCount;
                    progressBar1.Value = client.currentCount;
                    label1.Text = (client.currentCount+1).ToString();
                    label3.Text = client.maxCount.ToString();
                    if (client.maxCount == (client.currentCount + 1) && client.currentCount != 0) {
                        MessageBox.Show("安装/更新完成");

                        //创建桌面快捷方式
                        //Environment.UserName
                        CreateShortcuts(bullupPath + "\\Bullup.exe", "C:\\Users\\" + Environment.UserName + "\\Desktop", "斗牛电竞");

                        break;
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(50);
            }
            
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void progressBar1_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e) {
            Control.CheckForIllegalCrossThreadCalls = false;
        }
    }
}

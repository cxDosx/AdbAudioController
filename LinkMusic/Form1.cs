using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkMusic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            this.notifyIcon1.MouseDoubleClick += NotifyIconDoubleClick;
            this.notifyIcon1.MouseClick += CheckRightClickEvent;
        }

        private void CheckRightClickEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show();
            }
        }

        private void NotifyIconDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.Focus();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string Temp = this.textBox1.Text.Trim();
            this.textBox1.Text = Temp;
            device = Temp;
            MessageBox.Show("Your device set:\n" + Temp, "Save success!", MessageBoxButtons.OK);
        }


        private void ControlAudioPlayStatus(PlayStatus status)
        {
            string Command = "adb";
            if(device.Length != 0)
            {
                Command += " -s ";
                Command += device;
            }
            Command += " shell input keyevent " + (int)status;
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(Command + "&exit");

            p.StandardInput.AutoFlush = true;

            string strOuput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            Console.WriteLine(strOuput);
        }

        private void PlayPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlAudioPlayStatus(PlayStatus.PLAY);
        }

        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlAudioPlayStatus(PlayStatus.PAUSE);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlAudioPlayStatus(PlayStatus.STOP);
        }

        private void PrevToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlAudioPlayStatus(PlayStatus.PREV);
        }

        private void NextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlAudioPlayStatus(PlayStatus.NEXT);
        }
    }


}

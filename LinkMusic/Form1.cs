using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LinkMusic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadPreference();

            this.FormClosing += Form1_FormClosing;

            this.notifyIcon1.MouseDoubleClick += NotifyIconDoubleClick;
            this.notifyIcon1.MouseClick += CheckRightClickEvent;
        }

        private void LoadPreference()
        {
            device = Preference.ReadValue("defValue", "remoteAddress", "");
            this.textBox1.Text = device;
            RefreshNotifyText();
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
            Preference.SaveValue("defValue", "remoteAddress", device);
            if (device.Length != 0)
            {
                MessageBox.Show("Target:\n" + Temp, "Save success!", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Target: Default device", "Save success!", MessageBoxButtons.OK);
            }
            RefreshNotifyText();
        }

        private void RefreshNotifyText()
        {
            string notifyIconHead = "Adb audio controller\nCreated by cxDosx\nLink adb:";
            int len = device.Length + notifyIconHead.Length;
            if (len == notifyIconHead.Length)
            {
                SetNotifyIconText(notifyIcon1, notifyIconHead + "Default device");
            }
            else if (len < 64)
            {
                notifyIcon1.Text = notifyIconHead + device;
            }
            else if (len < 128)
            {
                SetNotifyIconText(notifyIcon1, notifyIconHead + device);
            }
            else
            {
                SetNotifyIconText(notifyIcon1, "Adb audio controller\nCreate by cxDosx\nYou already set a device");
            }
        }

        private void SetNotifyIconText(NotifyIcon notifyIcon, string text)
        {
            if (text.Length >= 128)
            {
                return;
            }
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(notifyIcon, text);
            if ((bool)t.GetField("added", hidden).GetValue(notifyIcon))
                t.GetMethod("UpdateIcon", hidden).Invoke(notifyIcon, new object[] { true });
        }


        private void ControlAudioPlayStatus(PlayStatus status)
        {
            string Command = "adb";
            if (device.Length != 0)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, 0x01, 3, Keys.Right); // NextHotKey
            RegisterHotKey(this.Handle, 0x02, 3, Keys.Left); // PrevHotKey
            RegisterHotKey(this.Handle, 0x03, 3, Keys.Down); // PauseHotKey
            RegisterHotKey(this.Handle, 0x04, 3, Keys.Up); // PlayHotKey

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0x01);
            UnregisterHotKey(this.Handle, 0x02);
            UnregisterHotKey(this.Handle, 0x03);
            UnregisterHotKey(this.Handle, 0x04);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:
                    int param = m.WParam.ToInt32();
                    switch (param)
                    {
                        case 0x01: //Next
                            NextToolStripMenuItem_Click(null, null);
                            break;
                        case 0x02: //Prev
                            PrevToolStripMenuItem_Click(null, null);
                            break;
                        case 0x03: //Pause
                            PauseToolStripMenuItem_Click(null, null);
                            break;
                        case 0x04: //Play
                            PlayPauseToolStripMenuItem_Click(null, null);
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        //注册热键的api
        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);
        //解除注册热键的api
        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleScreenProtector
{
    public partial class Form2 : Form
    {
        // https://stackoverflow.com/questions/3213606/how-to-suppress-task-switch-keys-winkey-alt-tab-alt-esc-ctrl-esc-using-low
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys 

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }

        //below original

        public string userPassword = "";
        public string userHint = "";
        public string userContact = "";
        public bool unlocked = false;
        public Form2(string password, string hint, string contact)
        {
            InitializeComponent();
            // PB
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
            // end
            userPassword = password;
            userHint = hint;
            userContact = contact;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Screen currentScreen = Screen.FromHandle(this.Handle);
            Rectangle screenBounds = currentScreen.Bounds;
            this.Width= screenBounds.Width;
            this.Height= screenBounds.Height;
            this.Top= screenBounds.Top;
            this.Left= screenBounds.Left;
            CenterControls();
            // MessageBox.Show(userPassword);
            Logger("시작", "보호가 시작되었습니다. 힌트: " + userHint);
            Logger("안내", "비밀번호를 틀리게 되면 로그가 남습니다. 손 대지 말아주세요.");
            label2.Text = "연락처: " + userContact;
        }

        private void CenterControls()
        {
            AdjustCenter(groupBox1, 100, 0);
            AdjustCenter(label1, -250, 0);
        }

        private void AdjustCenter(Control ctl, int top, int left)
        {
            ctl.Top = ((this.Height - ctl.Height) / 2) + top;
            ctl.Left = ((this.Width - ctl.Width) / 2) + left;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // login
            if(textBox2.Text == userPassword)
            {
                unlocked = true;
                this.Close();
            }
            else
            {
                unlocked = false;
                MessageBox.Show("비밀번호가 틀렸습니다.\r\n로그가 기록됩니다.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger("시도", "비밀번호가 틀렸습니다. 내역: " + textBox2.Text);
                textBox2.Clear();
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!unlocked)
            {
                e.Cancel= true;
            }
            else if(unlocked)
            {
                e.Cancel= false;
            }
        }

        private void Logger(string type, string text)
        {
            textBox1.AppendText("[" + DateTime.Now + "][" + type + "]" + " " + text + "\r\n");
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString();
        }
    }
}

using System.Diagnostics;

namespace MapleScreenProtector
{
    public partial class Form1 : Form
    {
        public static string nowVersion = "1.0";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // start
            Form2 form2 = new Form2(textBox1.Text, textBox2.Text, textBox3.Text);
            form2.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkVersion();
            fetchLicense();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/panta5/MapleScreenProtector", UseShellExecute = true });
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/panta5/MapleScreenProtector", UseShellExecute = true });
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private async void checkVersion()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/panta5/MapleScreenProtector/main/version.txt";
                using (HttpClient client = new HttpClient())
                {
                    string html = await client.GetStringAsync(url);
                    if(html.Trim() != nowVersion)
                    { MessageBox.Show("새로운 버전이 있습니다.\r\n업데이트를 하시려면 로고를 클릭해주세요.", "업데이트", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private async void fetchLicense()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/panta5/MapleScreenProtector/main/LICENSE";
                using (HttpClient client = new HttpClient())
                {
                    string html = await client.GetStringAsync(url);
                    textBox4.Text = html.Replace("\n", "\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
using FundBoard.Properties;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FundBoard
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private Timer timer = null;

        public Form1()
        {
            InitializeComponent();
            this.menuStrip1.Items.Add(BtnSeting);
            this.menuStrip1.Items.Add(开始ToolStripMenuItem);
            this.menuStrip1.Items.Add(结束ToolStripMenuItem);
            this.menuStrip1.Items.Add(Exit);
            this.menuStrip1.Items.Add(测试TIPToolStripMenuItem);
            try
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    base.SendToBack();

                    IntPtr hWndNewParent = User32.FindWindow("Progman", null);
                    User32.SetParent(base.Handle, hWndNewParent);
                }
                else
                {
                    User32.SetWindowPos(base.Handle, 1, 0, 0, 0, 0, User32.SE_SHUTDOWN_PRIVILEGE);
                }
            }
            catch (ApplicationException exx)
            {
                MessageBox.Show(this, exx.Message, "Pin to Desktop");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitData();
            InitConfig();
        }

        private void InitConfig()
        {
            this.ShowInTaskbar = false;
        }

        private void InitData()
        {
            if (!File.Exists(PathConstant.FILE_PATH))
            {
                File.WriteAllText(PathConstant.FILE_PATH, JsonConvert.SerializeObject(new List<string>(), Formatting.Indented));
            }

            timer = new Timer();

            timer.Interval = 5000;

            timer.Tick += new EventHandler((s, e) => LoadAllCodeValue());

            timer.Start();
        }


        private void LoadAllCodeValue()
        {
            string path = PathConstant.FILE_PATH;
            StreamReader streamReader = new StreamReader(path);
            string jsonStr = streamReader.ReadToEnd();
            List<string> jsonObj = JsonConvert.DeserializeObject<List<string>>(jsonStr);
            streamReader.Close();

            DataTable dt = new DataTable();

            dt.Columns.Add("代码");
            dt.Columns.Add("名称");
            dt.Columns.Add("涨幅");
            dt.Columns.Add("更新时间");

            foreach (var item in jsonObj)
            {
                var client = new RestClient("http://fundgz.1234567.com.cn/js/" + item + ".js?rt=1463558676006");

                var request = new RestRequest(Method.GET);

                IRestResponse response = client.Execute(request);

                /*
                 jsonpgz({"fundcode":"320007","name":"璇哄畨鎴愰暱娣峰悎","jzrq":"2020-05-20","dwjz":"1.6290","gsz":"1.6183","gszzl":"-0.66","gztime":"2020-05-21 10:56"});
                 */

                var content = response.Content;

                content = content.Replace("jsonpgz(", "");
                content = content.Replace(");", "");

                dynamic json = null;
                try
                {
                    json = JsonConvert.DeserializeObject<dynamic>(content);
                }
                catch (Exception)
                {
                    //MessageBox.Show("代码：" + item + "存在错误");
                    continue;
                }


                string code = json["fundcode"];

                string name = json["name"];

                string gszzl = json["gszzl"];

                string gztime = json["gztime"];

                dt.Rows.Add(code, name, gszzl, gztime);
            }

            this.dataGridView1.DataSource = dt;

            dataGridView1.ClearSelection();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnSeting_Click(object sender, EventArgs e)
        {
            new Setings().ShowDialog();
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (this.dataGridView1.Rows.Count != 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells["涨幅"].Value.ToString().IndexOf('-') != -1)
                    {
                        this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 0, 0);
                    }
                    else
                    {
                        this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(0, 128, 0);
                    }
                }
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                User32.SetWindowPos(base.Handle, 1, 0, 0, 0, 0, User32.SE_SHUTDOWN_PRIVILEGE);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                User32.SetWindowPos(base.Handle, 1, 0, 0, 0, 0, User32.SE_SHUTDOWN_PRIVILEGE);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void 测试TIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon notifyIcon1 = new NotifyIcon();
            notifyIcon1.Visible = true;
            notifyIcon1.Icon = Resources.j;
            notifyIcon1.ShowBalloonTip(2000, "ji", "10010", ToolTipIcon.Warning);
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void 结束ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FundBoard
{
    public partial class Setings : Form
    {
        public Setings()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            base.Close();
        }



        private void Txt_Add_Click(object sender, EventArgs e)
        {
            try
            {
                string path = PathConstant.FILE_PATH;
                StreamReader streamReader = new StreamReader(path);
                string jsonStr = streamReader.ReadToEnd();

                List<string> jsonObj = JsonConvert.DeserializeObject<List<string>>(jsonStr);
                jsonObj.Add(this.Txt_Code.Text.Trim());
                streamReader.Close();

                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(path, output);

                InitData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/r/n" + ex.StackTrace);
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count == 1)
            {
                string code = this.listBox1.SelectedItem.ToString();

                try
                {
                    string path = PathConstant.FILE_PATH;
                    StreamReader streamReader = new StreamReader(path);
                    string jsonStr = streamReader.ReadToEnd();

                    List<string> jsonObj = JsonConvert.DeserializeObject<List<string>>(jsonStr);
                    jsonObj.Remove(jsonObj.Where(w => w == code).FirstOrDefault());
                    streamReader.Close();

                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(path, output);

                    InitData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "/r/n" + ex.StackTrace);
                }
            }
        }


        private void InitData()
        {

            try
            {
                string path = PathConstant.FILE_PATH;
                StreamReader streamReader = new StreamReader(path);
                string jsonStr = streamReader.ReadToEnd();

                List<string> jsonObj = JsonConvert.DeserializeObject<List<string>>(jsonStr);

                this.listBox1.DataSource = jsonObj;

                streamReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/r/n" + ex.StackTrace);
            }
        }

        private void Setings_Load(object sender, EventArgs e)
        {
            InitData();
        }
    }
}

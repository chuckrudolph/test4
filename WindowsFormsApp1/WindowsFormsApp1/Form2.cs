using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2() {
            InitializeComponent();
        }

         void Form2_Load(object sender, EventArgs e) {
            //this.webView21.NavigateToString("this is a test");
            this.webView21.CoreWebView2InitializationCompleted += WebView21_CoreWebView2InitializationCompleted;

            this.Shown += Form2_Shown;

            string[] uids = getUids();
            string message = "";
            
            foreach (string s in uids) {
                string t = $"0020000D={s}";
                string t2 = Uri.EscapeDataString(Convert.ToBase64String(Encoding.ASCII.GetBytes(t)));
                message += $"clear:{t}\r\nurl encoded:{t2}\r\n\r\n";

            }

            MessageBox.Show(message);


            string test = "0020000D=4";
            string test2 = Uri.EscapeDataString(Convert.ToBase64String(Encoding.ASCII.GetBytes(test)));
            MessageBox.Show($"{test}\r\n{test2}");

            //test = "0020000D=1.2.840.113619.2.278.3.269093376.615.1651038473.480";
            //test2 = Uri.EscapeDataString(Convert.ToBase64String(Encoding.ASCII.GetBytes(test)));
            //MessageBox.Show($"{test}\r\n{test2}");

            //test = "0020000D=1.2.840.113619.2.278.3.269093376.615.1651038473.381";
            //test2 = Uri.EscapeDataString(Convert.ToBase64String(Encoding.ASCII.GetBytes(test)));
            //MessageBox.Show($"{test}\r\n{test2}");
        }

        async void Form2_Shown(object sender, EventArgs e) {
            await this.webView21.EnsureCoreWebView2Async(null);
            StatRad.MvcApi.ExamClient ec = new StatRad.MvcApi.ExamClient();
            string rpt = await ec.GetExam(8896);
            this.webView21.NavigateToString(rpt);
        }

         void WebView21_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e) {
            if (e.IsSuccess) {
                //StatRad.MvcApi.ExamClient ec = new StatRad.MvcApi.ExamClient();
                //string rpt = await ec.GetExam(8896);
                //this.webView21.NavigateToString(rpt);
            }
            else {
                MessageBox.Show(e.InitializationException.Message);
            }
        }

        async void printToolStripMenuItem_Click(object sender, EventArgs e) {
            await this.webView21.CoreWebView2.ExecuteScriptAsync("window.print();");
            await this.webView21.CoreWebView2.PrintToPdfAsync("c:\\temp\\test.pdf");
        }

        private void messageTesterToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Chrome Files (*.htm*)|*.htm*|All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK) {
                //StreamReader sr = new StreamReader(ofd.FileName);
                //webView21.NavigationCompleted += WebView21_NavigationCompleted;
                //webView21.NavigateToString(sr.ReadToEnd());
                webView21.CoreWebView2.Navigate($"File://{ofd.FileName}");
            }
        }

        private void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e) {

            // enable test button
        }

        async void comTestbasicToolStripMenuItem_Click(object sender, EventArgs e) {
            var o = new  { x = 1, y = 2 };
            string script = JsonConvert.SerializeObject(o);
            webView21.WebMessageReceived += WebView21_WebMessageReceived;
            //await webView21.ExecuteScriptAsync($"AddEm('{o}';");
            object o2 = await webView21.ExecuteScriptAsync($"AddEm({script});");
        }

        private void WebView21_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e) {
            webView21.WebMessageReceived -= WebView21_WebMessageReceived;
            MessageBox.Show(e.WebMessageAsJson);
        }

        string[] getUids() {
            List<string> rc = new List<string>();

            SqlConnection conn = new SqlConnection("Data Source=srsdproddb1.statrad.com;Initial Catalog=caseentry;Integrated Security=true");
            SqlCommand cmd = new SqlCommand("nix_getViewerInfoForExamIdV2",conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@examId", 8014384);

            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) {
                rc.Add(dr["studyUid"].ToString());
            }

            dr.Close();
            conn.Close();

            return rc.ToArray();
        }
    }
}

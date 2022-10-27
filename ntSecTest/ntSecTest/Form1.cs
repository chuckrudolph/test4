using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace ntSecTest
{
    public partial class Form1 : Form
    {
        string webAddress;
        string sqlConn;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            try {
                sqlConn = ConfigurationManager.AppSettings["sqlServer"];
                webAddress = ConfigurationManager.AppSettings["webService"];

                if (!webAddress.StartsWith("http")) {
                    webAddress = "http://" + webAddress;
                }

                if (!webAddress.EndsWith("/")) {
                    webAddress += "/";
                }

                webAddress += "adderUp/";
                rtbConfigInfo.AppendText($"web service url: {webAddress}\n\rsql connection:{sqlConn}");
            }
            catch (Exception ex) {
                rtbData.AppendText(ex.Message);
                btnDb.Enabled = false;
                btnOpen.Enabled = false;
                btnSecure.Enabled = false;
            }
        }

        private void btnDb_Click(object sender, EventArgs e) {
            using (SqlConnection conn = new SqlConnection(sqlConn)) {
                using (SqlCommand cmd = new SqlCommand("select count(*) from hospitals", conn)) {
                    try {
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Read();
                        int count = (int)dr[0];
                        rtbData.Text = $"{count} rows in the hospital table";
                    }
                    catch (Exception ex) {
                        rtbData.Text = ex.Message;
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            urlTest(false);
        }

        private void btnSecure_Click(object sender, EventArgs e) {
            urlTest(true);
        }

        void urlTest(bool secure) {
            HttpWebRequest req = null;

            rtbData.Text = "Working";
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Application.DoEvents();

            try {
                if (secure) {
                    req = (HttpWebRequest)WebRequest.Create($"{webAddress}AddSecure");
                    req.UseDefaultCredentials = true;
                    req.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else {
                    req = (HttpWebRequest)WebRequest.Create($"{webAddress}Add");
                    req.UseDefaultCredentials = false;
                }

                req.Method = "POST";
                req.Accept = "application/json; charset=utf-8";

                xy temp = new xy {
                    x = DateTime.Now.Second,
                    y = DateTime.Now.Minute
                };

                string sxy = JsonConvert.SerializeObject(temp);

                req.ContentLength = sxy.Length;
                req.ContentType = "application/json; charset=utf-8";

                using (var webStream = req.GetRequestStream()) {
                    using (var requestWriter = new StreamWriter(webStream, Encoding.ASCII)) {
                        requestWriter.Write(sxy);
                    }
                }

                req.Timeout = 2000;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                string sReponse = null;

                using (var webStream = resp.GetResponseStream()) {
                    using (StreamReader responseReader = new StreamReader(webStream)) {
                        sReponse = responseReader.ReadToEnd();
                    }
                }

                rtbData.Text = $"Http Response = {resp.StatusCode}/{resp.StatusDescription}\r\nand the result of {temp.x} + {temp.y} = {sReponse} (in plain text, not json)";
            }
            catch (Exception ex) {
                rtbData.Text = ex.Message;
            }

        }

        class xy
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}

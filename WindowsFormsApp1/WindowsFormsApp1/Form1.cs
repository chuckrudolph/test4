using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FDLink;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        FDLink.Application _theApp;
        FDLink.IApplicationServer _server;
        FDLink.IDictationControl _dc;
        FDLink.RecordingControl _rc;

        public Form1() {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            _dc?.SetEnabled(false);
            _dc?.ClearTargetWindow();
            _dc = null;
            _server = null;
            _theApp = null;
            _rc = null;
        }

        void micStateChanged (IMicrophoneStateChangedEventArgs args) {
            System.Diagnostics.Trace.WriteLine($"microphone state is {args.CurrentState}");
            if (args.CurrentState != MicrophoneState.Stopped) {
                this.button1.BackColor = Color.LightGreen;
            }
            else {
                this.button1.BackColor = SystemColors.Control;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            _theApp = new FDLink.Application();

            try {

                if (_theApp.IsRunning()) {
                    _server = _theApp.Connect();
                }
                else {
                    _server = _theApp.Start();
                }

                _dc = _server.GetDictationControl();

                _dc.SetTargetWindow(rtbDictate.Handle.ToInt32());
                _dc.SetEnabled(true);

                _rc = (FDLink.RecordingControl)_server.GetRecordingControl();
                _rc.MicrophoneStateChanged += micStateChanged;


            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            int onOrOff = _rc.GetMicrophoneState();
            if (onOrOff == 1) {
                _rc.StartRecording("");
            }
            else {
                _rc.StopRecording();
            }

        }
    }
}

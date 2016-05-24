using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XingPrice
{
    public partial class frmMain : Form
    {
        private Session session;
        private BackgroundWorker bw;
        private bool working = false;
        
        public frmMain()
        {
            InitializeComponent();

            FormClosed += frmMain_FormClosed;

            session = new Session(Login_Handler);
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            working = false;
            MessageBox.Show("done");
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var codes = new String[] {
                "084990", // 바이로메드
                "214370", // 케어젠
                "102940", // 코오롱생명과학
                "095700", // 제넥신
                "034830", // 한국토지신탁
                "046890", // 서울반도체
            };

            foreach (var code in codes)
            {
                for (var month = 5; month <= 12; month++)
                {
                    var query = new Query(code, 2015, month);
                    if (query.Run() < 0) Console.WriteLine("Fail to query {0} {1}", 2015, month);
                    Thread.Sleep(1200);
                }
                for (var month = 1; month <= 5; month++)
                {
                    var query = new Query(code, 2016, month);
                    if (query.Run() < 0) Console.WriteLine("Fail to query {0} {1}", 2016, month);
                    Thread.Sleep(1200);
                }
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            session.Disconnect();
        }

        private void Login_Handler(string szCode, string szMsg)
        {
            if (szCode == "0000")
            {
                MessageBox.Show("Success to login");
            }
            else
            {
                MessageBox.Show(String.Format("Fail to login: [{0}] {1}", szCode, szMsg));
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!session.Connect())
            {
                MessageBox.Show("Fail to connect");
                return;
            }
            session.Login(txtId.Text, txtPass.Text, txtCertPass.Text);
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (working)
            {
                MessageBox.Show("working");
                return;
            }
            working = true;
            bw.RunWorkerAsync();
        }
    }
}

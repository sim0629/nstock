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
        
        public frmMain()
        {
            InitializeComponent();

            FormClosed += frmMain_FormClosed;

            session = new Session(Login_Handler);
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
            for (var month = 5; month <= 12; month++)
            {
                var query = new Query(2015, month);
                if (query.Run() < 0) Console.WriteLine("Fail to query {0} {1}", 2015, month);
                Thread.Sleep(1200);
            }
            for (var month = 1; month <= 4; month++)
            {
                var query = new Query(2016, month);
                if (query.Run() < 0) Console.WriteLine("Fail to query {0} {1}", 2016, month);
                Thread.Sleep(1200);
            }
        }
    }
}

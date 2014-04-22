using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// DU:  Added for networking
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;

using GR_Steelheaders_Member;

namespace GRSteelheaders_Client
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //TO DO:  retrieve connection values from an external source
            ServerHelper.client = new TcpClient("localhost", 5000);
            ServerHelper.reader = new StreamReader(ServerHelper.client.GetStream());
            ServerHelper.writer = new StreamWriter(ServerHelper.client.GetStream());
            ServerHelper.writer.AutoFlush = true;  // This is still the most important property to set!
        }



        private void btnViewMemberList_Click(object sender, EventArgs e)
        {
            frmActive fActive = new frmActive();

            fActive.Show();
        }

        private void btnViewInactiveList_Click(object sender, EventArgs e)
        {
            frmInactive fInactive = new frmInactive();

            fInactive.Show();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAdd fAdd = new frmAdd();

            fAdd.Show();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            frmEdit fEdit = new frmEdit();

            fEdit.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                ServerHelper.writer.WriteLine("<request><action>disconnect</action></request>");

                if (ServerHelper.client != null)
                    ServerHelper.client.Close();

                if (ServerHelper.client != null)
                    ServerHelper.reader.Close();

                if (ServerHelper.client != null)
                    ServerHelper.writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Close();
            }
        }
    }
}

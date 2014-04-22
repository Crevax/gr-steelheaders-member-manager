using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using GR_Steelheaders_Member;

namespace GRSteelheaders_Client
{
    public partial class frmSearch : Form
    {
        public string Member_ID;

        public frmSearch()
        {
            InitializeComponent();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Member_ID = String.Empty;
            Close();
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            Member mem = new Member();
            String request = String.Empty;
            String response = String.Empty;

            XmlDocument xmlDoc = null;
            XmlNodeList xmlNodes = null;

            mem.LastName = txtLastName.Text;

            request = mem.BuildRequest(Member.requestType.SEARCH);

            ServerHelper.writer.WriteLine(request);

            response = ServerHelper.reader.ReadLine();

            if (Member.getNodeText("//response/errorCode", response).Equals("0"))
            {

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                xmlNodes = xmlDoc.SelectNodes("//response/members/member");

                grdSearchResults.Rows.Clear();

                foreach (XmlNode node in xmlNodes)
                {
                    grdSearchResults.Rows.Add(node.ChildNodes[0].InnerText,
                                            node.ChildNodes[1].InnerText,
                                            node.ChildNodes[2].InnerText);
                }

            }
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            grdSearchResults.Columns.Add("Mem_ID", "ID");
            grdSearchResults.Columns.Add("Mem_First_Name", "First Name");
            grdSearchResults.Columns.Add("Mem_Last_Name", "Last Name");
        }

        private void grdSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = grdSearchResults.SelectedCells[0].RowIndex;
            Member_ID = grdSearchResults.Rows[index].Cells[0].Value.ToString();
            Close();
        }

    }
}

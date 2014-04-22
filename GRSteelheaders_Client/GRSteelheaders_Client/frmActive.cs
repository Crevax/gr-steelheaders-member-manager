﻿using System;
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
    public partial class frmActive : Form
    {
        public frmActive()
        {
            InitializeComponent();
        }

        private void frmActive_Load(object sender, EventArgs e)
        {
            grdActiveMembers.Columns.Add("Mem_ID", "ID");
            grdActiveMembers.Columns.Add("Mem_First_Name", "First Name");
            grdActiveMembers.Columns.Add("Mem_Last_Name", "Last Name");

            String request = String.Empty;
            String response = String.Empty;

            XmlDocument xmlDoc = null;
            XmlNodeList xmlNodes = null;

            request = "<request><action>list_active</action></request>";

            ServerHelper.writer.WriteLine(request);

            response = ServerHelper.reader.ReadLine();

            if (Member.getNodeText("//response/errorCode", response).Equals("0"))
            {

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                xmlNodes = xmlDoc.SelectNodes("//response/members/member");

                grdActiveMembers.Rows.Clear();

                foreach (XmlNode node in xmlNodes)
                {
                    grdActiveMembers.Rows.Add(node.ChildNodes[0].InnerText,
                                            node.ChildNodes[1].InnerText,
                                            node.ChildNodes[2].InnerText);
                }

            }
        }

    }
}

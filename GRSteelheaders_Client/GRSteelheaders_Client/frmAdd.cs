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
using System.Text.RegularExpressions;

using GR_Steelheaders_Member;

namespace GRSteelheaders_Client
{
    public partial class frmAdd : Form
    {
        public frmAdd()
        {
            InitializeComponent();
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            String request = String.Empty;
            String response = String.Empty;
            Member mem = new Member();

            if (Validate() == true)  // There are errors!
            {
                MessageBox.Show("Please fix the errors and try again!");
            }
            else
            {
                mem.FirstName = txtFirstName.Text;
                mem.LastName = txtLastName.Text;
                mem.Address = txtAddress.Text;
                mem.City = txtCity.Text;
                mem.State = cboState.SelectedItem.ToString();
                mem.ZIP = mtxtZipCode.Text;
                mem.Phone = mtxtPhoneNumber.Text;
                mem.Email = txtEmailAddress.Text;
                mem.StatusID = cboStatus.SelectedIndex + 1; //Program index starts at 0, database index starts at 1; so we add 1 to match the program with the database

                // Build the request
                request = mem.BuildRequest(Member.requestType.ADD);

                // Send the request
                ServerHelper.writer.WriteLine(request);

                // Await the response
                response = ServerHelper.reader.ReadLine();

                // Process the response

                String newID = Member.getNodeText("//response/member/mem_id", response);

                if (!newID.Equals(String.Empty))
                    txtMemberID.Text = newID;
            }
        }

        private void frmAdd_Load(object sender, EventArgs e)
        {
            String request = "<request><action>listStatuses</action></request>";
            String response = String.Empty;
            try
            {
                ServerHelper.writer.WriteLine(request);
                response = ServerHelper.reader.ReadLine();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                XmlNodeList nodeList = xmlDoc.SelectNodes("//response/statuses/status");

                String status_id = String.Empty;
                String status_desc = String.Empty;
                cboStatus.Items.Clear();  // remove the default items if we have a good list from the database.

                foreach (XmlNode node in nodeList)
                {
                    status_id = node.ChildNodes.Item(0).InnerText;  // Note:  This assumes the structure/ordering of the XML will not change.  Is this wise in this case? 
                    status_desc = node.ChildNodes.Item(1).InnerText;
                    cboStatus.Items.Add(status_desc);
                }

                request = "<request><action>listStateCodes</action></request>";

                ServerHelper.writer.WriteLine(request);
                response = ServerHelper.reader.ReadLine();

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                nodeList = xmlDoc.SelectNodes("//response/states/state");

                String state_code = String.Empty;
                String state_name = String.Empty;
                cboState.Items.Clear();  // remove the default items if we have a good list from the database.

                foreach (XmlNode node in nodeList)
                {
                    state_code = node.ChildNodes.Item(0).InnerText;  // Note:  This assumes the structure/ordering of the XML will not change.  Is this wise in this case? 
                    state_name = node.ChildNodes.Item(1).InnerText;
                    cboState.Items.Add(state_code);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Boolean Validate()
        {
            Boolean hasError = false;

            if (!Regex.IsMatch(txtMemberID.Text, @"^\d+$"))
            {
                txtMemberID.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtMemberID.BackColor = Color.White;
            }

            if (cboStatus.SelectedItem == null)
            {
                hasError = true;
            }

            if (!Regex.IsMatch(txtFirstName.Text, @"^\w+$"))
            {
                txtFirstName.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtFirstName.BackColor = Color.White;
            }

            if (!Regex.IsMatch(txtLastName.Text, @"^\w+$"))
            {
                txtLastName.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            if (String.IsNullOrEmpty(txtAddress.Text))
            {
                txtAddress.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            if (!Regex.IsMatch(txtCity.Text, @"^\w+$"))
            {
                txtCity.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            if (cboState.SelectedItem == null)
            {
                hasError = true;
            }

            if (!Regex.IsMatch(mtxtZipCode.Text, @"^\d{5}-\d{4}$"))
            {
                mtxtZipCode.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            if (String.IsNullOrEmpty(mtxtPhoneNumber.Text))
            {
                mtxtPhoneNumber.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            if (String.IsNullOrEmpty(txtEmailAddress.Text))
            {
                txtEmailAddress.BackColor = Color.Red;
                hasError = true;
            }
            else
            {
                txtLastName.BackColor = Color.White;
            }

            return hasError;

        }
    }
}

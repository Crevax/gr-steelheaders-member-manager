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
    public partial class frmEdit : Form
    {
        public frmEdit()
        {
            InitializeComponent();
        }

        private void frmEdit_Load(object sender, EventArgs e)
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

        private void cmdSubmit_Click(object sender, EventArgs e)
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
                mem.ID = Convert.ToInt32(txtMemberID.Text);
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
                request = mem.BuildRequest(Member.requestType.UPDATE);

                // Send the request
                ServerHelper.writer.WriteLine(request);

                // Await the response
                response = ServerHelper.reader.ReadLine();

                // Process the response
                String errorCode = Member.getNodeText("//response/errorCode", response);

                if (errorCode.Equals("0"))
                {
                    MessageBox.Show("Record Updated!");
                }
                else
                {
                    String error = Member.getNodeText("//response/errorMessage", response);
                    MessageBox.Show("Error:  " + error);
                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            String request = String.Empty;
            String response = String.Empty;
            Member mem = new Member();

            mem.ID = Convert.ToInt32(txtMemberID.Text);

            // Build the request
            request = mem.BuildRequest(Member.requestType.DELETE);

            // Send the request
            ServerHelper.writer.WriteLine(request);

            // Await the response
            response = ServerHelper.reader.ReadLine();

            // Process the response
            String errorCode = Member.getNodeText("//response/errorCode", response);

            if (errorCode.Equals("0"))
            {
                doInquire();
                MessageBox.Show("Record Deleted!");
            }
            else
            {
                String error = Member.getNodeText("//response/errorMessage", response);
                MessageBox.Show("Error:  " + error);
            }
        }

        private void doInquire()
        {
            String request = String.Empty;
            String response = String.Empty;
            Member mem = new Member();

            mem.ID = Convert.ToInt32(txtMemberID.Text);

            // Build the request
            request = mem.BuildRequest(Member.requestType.INQUIRE);

            // Send the request
            ServerHelper.writer.WriteLine(request);

            // Await the response
            response = ServerHelper.reader.ReadLine();

            // Process the response

            String errorCode = Member.getNodeText("//response/errorCode", response);

            if (!errorCode.Equals("0"))
                MessageBox.Show(Member.getNodeText("//response/errorMessage", response));
            else
            {
                txtFirstName.Text = Member.getNodeText("//response/member/mem_first_name", response);
                txtLastName.Text = Member.getNodeText("//response/member/mem_last_name", response);
                txtAddress.Text = Member.getNodeText("//response/member/mem_address", response);
                txtCity.Text = Member.getNodeText("//response/member/mem_city", response);
                cboState.SelectedIndex = cboState.FindStringExact(Member.getNodeText("//response/member/mem_state_code", response));
                mtxtZipCode.Text = Member.getNodeText("//response/member/mem_zip", response);
                mtxtPhoneNumber.Text = Member.getNodeText("//response/member/mem_phone", response);
                txtEmailAddress.Text = Member.getNodeText("//response/member/mem_email", response);
                cboStatus.SelectedIndex = Convert.ToInt32(Member.getNodeText("//response/member/mem_status_id", response)) - 1; //Subtract one to sync Computerized Index Start (0) to Databse Index Start (1)
            }

        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            frmSearch fSearch = new frmSearch();

            fSearch.ShowDialog();

            if (fSearch.Member_ID != String.Empty)
            {
                txtMemberID.Text = fSearch.Member_ID;
                doInquire();
            }

            fSearch = null;
        }

        private void cmdExit_Click(object sender, EventArgs e)
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

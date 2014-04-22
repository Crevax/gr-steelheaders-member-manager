using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data.OleDb;

namespace GR_Steelheaders_Member
{
    public class Member
    {
        private int _ID;
        private string _fname,
                       _lname,
                       _address,
                       _city,
                       _state,
                       _ZIP,
                       _phone,
                       _email;
        private int _statusID;

        public enum requestType
        {
            ADD,
            UPDATE,
            DELETE,
            UNDELETE,
            PURGE,
            INQUIRE,
            SEARCH,
            LIST_ACTIVE,
            LIST_INACTIVE
        }

        public Member()
        {
            _ID = 0;
            _fname = String.Empty;
            _lname = String.Empty;
            _address = String.Empty;
            _city = String.Empty;
            _state = String.Empty;
            _ZIP = String.Empty;
            _phone = String.Empty;
            _email = String.Empty;
            _statusID = 0;
        }

        public Member(OleDbDataReader dr)
        {
            _ID = Convert.ToInt32(dr["Mem_ID"].ToString());
            _fname = dr["Mem_First_Name"].ToString();
            _lname = dr["Mem_Last_Name"].ToString();
            _address = dr["Mem_Address"].ToString();
            _city = dr["Mem_City"].ToString();
            _ZIP = dr["Mem_Zip_Code"].ToString();
            _state = dr["Mem_State_Code"].ToString();
            _phone = dr["Mem_Phone_Number"].ToString();
            _email = dr["Mem_Email"].ToString();
            _statusID = Convert.ToInt32(dr["Mem_Status_ID"].ToString());
        }

        public Member(String xml)
        {
            String value = null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            value = getNodeText("//member/mem_id", xmlDoc);
            if (value == null || value == String.Empty)
                value = "0";
            _ID = 0;
            ID = Convert.ToInt32(value);

            value = getNodeText("//member/mem_first_name", xmlDoc);
            _fname = String.Empty;
            FirstName = value;

            value = getNodeText("//member/mem_last_name", xmlDoc);
            _lname = String.Empty;
            LastName = value;

            value = getNodeText("//member/mem_address", xmlDoc);
            _address = String.Empty;
            Address = value;

            value = getNodeText("//member/mem_city", xmlDoc);
            _city = String.Empty;
            City = value;

            value = getNodeText("//member/mem_state_code", xmlDoc);
            _state = String.Empty;
            State = value;

            value = getNodeText("//member/mem_zip", xmlDoc);
            _ZIP = String.Empty;
            ZIP = value;

            value = getNodeText("//member/mem_phone", xmlDoc);
            _phone = String.Empty;
            Phone = value;

            value = getNodeText("//member/mem_email", xmlDoc);
            _email = String.Empty;
            Email = value;

            value = getNodeText("//member/mem_status_id", xmlDoc);
            if (value == null || value == String.Empty)
                value = "0";
            _statusID = 0;
            StatusID = Convert.ToInt32(value);
            
        }

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (value > 0)
                    _ID = value;
            }
        }

        public string FirstName
        {
            get
            {
                return _fname;
            }
            set
            {
                if (value.Length > 0)
                    _fname = value;
            }
        }

        public string LastName
        {
            get
            {
                return _lname;
            }
            set
            {
                if (value.Length > 0)
                    _lname = value;
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (value.Length > 0)
                    _address = value;
            }
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (value.Length > 0)
                    _city = value;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value.Length > 0)
                    _state = value;
            }
        }

        public string ZIP
        {
            get
            {
                return _ZIP;
            }
            set
            {
                if (value.Length > 0)
                    _ZIP = value;
            }
        }

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (value.Length > 0)
                    _phone = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value.Length > 0)
                    _email = value;
            }
        }

        public int StatusID
        {
            get
            {
                return _statusID;
            }
            set
            {
                if (value > 0)
                    _statusID = value;
            }
        }

        public String BuildRequest(requestType reqType)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(sw);
            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages


            switch (reqType)
            {
                case requestType.ADD:
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("request");
                    xmlWriter.WriteElementString("action", "add");
                    xmlWriter.WriteStartElement("member");
                    xmlWriter.WriteElementString("mem_first_name", FirstName);
                    xmlWriter.WriteElementString("mem_last_name", LastName);
                    xmlWriter.WriteElementString("mem_address", Address);
                    xmlWriter.WriteElementString("mem_city", City);
                    xmlWriter.WriteElementString("mem_state_code", State);
                    xmlWriter.WriteElementString("mem_zip", ZIP);
                    xmlWriter.WriteElementString("mem_phone", Phone);
                    xmlWriter.WriteElementString("mem_email", Email);
                    xmlWriter.WriteElementString("mem_status_id", StatusID.ToString());
                    xmlWriter.WriteEndElement(); // end member
                    xmlWriter.WriteEndElement(); // end request
                    xmlWriter.WriteEndDocument();
                    break;
                case requestType.UPDATE:
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("request");
                    xmlWriter.WriteElementString("action", "update");
                    xmlWriter.WriteStartElement("member");
                    xmlWriter.WriteElementString("mem_id", ID.ToString());
                    xmlWriter.WriteElementString("mem_first_name", FirstName);
                    xmlWriter.WriteElementString("mem_last_name", LastName);
                    xmlWriter.WriteElementString("mem_address", Address);
                    xmlWriter.WriteElementString("mem_city", City);
                    xmlWriter.WriteElementString("mem_state_code", State);
                    xmlWriter.WriteElementString("mem_zip", ZIP);
                    xmlWriter.WriteElementString("mem_phone", Phone);
                    xmlWriter.WriteElementString("mem_email", Email);
                    xmlWriter.WriteElementString("mem_status_id", StatusID.ToString());
                    xmlWriter.WriteEndElement(); // end member
                    xmlWriter.WriteEndElement(); // end request
                    xmlWriter.WriteEndDocument();
                    break;
                case requestType.DELETE:
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("request");
                    xmlWriter.WriteElementString("action", "delete");
                    xmlWriter.WriteStartElement("member");
                    xmlWriter.WriteElementString("mem_id", ID.ToString());
                    xmlWriter.WriteEndElement(); // end member
                    xmlWriter.WriteEndElement(); // end request
                    xmlWriter.WriteEndDocument();
                    break;
                case requestType.INQUIRE:
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("request");
                    xmlWriter.WriteElementString("action", "inquire");
                    xmlWriter.WriteStartElement("member");
                    xmlWriter.WriteElementString("mem_id", ID.ToString());
                    xmlWriter.WriteEndElement(); // end member
                    xmlWriter.WriteEndElement(); // end request
                    xmlWriter.WriteEndDocument();
                    break;
                case requestType.SEARCH:
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("request");
                    xmlWriter.WriteElementString("action", "search");
                    xmlWriter.WriteStartElement("member");
                    xmlWriter.WriteElementString("mem_last_name", LastName);
                    xmlWriter.WriteEndElement(); // end member
                    xmlWriter.WriteEndElement(); // end request
                    xmlWriter.WriteEndDocument();
                    break;
            }


            return sw.ToString();
        }

        public String toXML()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(sw);

            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages

            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("member");

            xmlWriter.WriteElementString("mem_id", ID.ToString());
            xmlWriter.WriteElementString("mem_first_name", FirstName);
            xmlWriter.WriteElementString("mem_last_name", LastName);
            xmlWriter.WriteElementString("mem_address", Address);
            xmlWriter.WriteElementString("mem_city", City);
            xmlWriter.WriteElementString("mem_state_code", State);
            xmlWriter.WriteElementString("mem_zip", ZIP);
            xmlWriter.WriteElementString("mem_phone", Phone);
            xmlWriter.WriteElementString("mem_email", Email);
            xmlWriter.WriteElementString("mem_status_id", StatusID.ToString());

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            return sw.ToString();
        }

        public static String getNodeText(String xPath, String xml)
        {
            XmlDocument xmlDoc = new XmlDocument();

            String nodeValue = String.Empty;

            try
            {
                xmlDoc.LoadXml(xml);
                nodeValue = getNodeText(xPath, xmlDoc);
            }
            catch (Exception ex)
            {
                nodeValue = String.Empty;
            }

            return nodeValue;
        }

        public static String getNodeText(String xPath, XmlDocument xmlDoc)
        {
            String value = String.Empty;
            XmlNode xNode = null;

            xNode = xmlDoc.SelectSingleNode(xPath);		// extract a single node based upon the xpath string provided 

            if (xNode != null)
            {
                value = xNode.InnerText;	// save the text; otherwise return null
            }

            return value;
        }
    }
}

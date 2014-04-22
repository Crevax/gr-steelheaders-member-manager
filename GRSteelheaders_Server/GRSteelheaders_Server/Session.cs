using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

using GR_Steelheaders_Member;

namespace GRSteelheaders_Server
{
    class Session
    {
        private static long m_session_id = 0;
        private long m_sid;
        private TcpClient m_client;
        private StreamReader m_reader;
        private StreamWriter m_writer;
        private Boolean m_isListening;

        public enum ResponseType
        {
            ADD,
            UPDATE,
            DEACTIVATE,
            REACTIVATE,
            DELETE,
            INQUIRE,
            SEARCH,
            LIST_ACTIVE,
            LIST_INACTIVE
        }

        public Session(TcpClient client)
        {
            // Increment the session ID
            m_sid = ++m_session_id;

            m_client = client;
            m_reader = new StreamReader(client.GetStream());
            m_writer = new StreamWriter(client.GetStream());
            m_writer.AutoFlush = true;

            m_isListening = true;

            LogMessage("Client Session {0} Initialized from {1}.", m_sid, m_sid, client.Client.RemoteEndPoint.ToString());
        }

        // The session version of the LogMessage will display the session id for tracking purposes.  The server 
        // version does not automatically append the session id 
        public static void LogMessage(String message, long sid, params object[] args)
        {
            // Apply the formatting requested by the user
            String s = String.Format(message, args);

            // add the date/time to the message 
            Console.WriteLine("{0}\t{1}:{2}", DateTime.Now.ToString(), sid, s);

            //write a log file entry
            if (Server.logWriter == null)
                Server.logWriter = LogWriter.getInstance();

            Server.logWriter.WriteLogMessage(String.Format("{0}\t{1}", DateTime.Now.ToString(), s));
        }

        public void Run()
        {
            String request = String.Empty;
            String response = String.Empty;
            Member mem = null;
            Member[] mem_list;

            MemberDB.connString = Server.dbConnString;
            // Working Storage Fields

            while (m_isListening)
            {
                try
                {
                    // Wait for a request
                    request = m_reader.ReadLine();

                    // Log the request
                    LogMessage("Request: {0}", m_sid, request);

                    // Handle the request
                    String action = getNodeText("//request/action", request);

                    if (action != null)
                    {
                        if (action.Equals("add"))
                        {
                            try
                            {
                                // Construct a member using the request 
                                mem = new Member(request);

                                // Try to add the member record to the database
                                mem = MemberDB.Add(mem);

                                // Send the response to the client
                                response = BuildResponse(mem, ResponseType.ADD);
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.ADD, ex);
                            }
                        }

                        else if (action.Equals("update"))
                        {
                            try
                            {
                                mem = new Member(request);

                                mem = MemberDB.Update(mem);

                                response = BuildResponse(mem, ResponseType.UPDATE);
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.UPDATE, ex);
                            }
                        }

                        else if (action.Equals("deactivate"))
                        {
                            try
                            {
                                mem = new Member(request);

                                mem = MemberDB.Deactivate(mem);

                                response = BuildResponse(mem, ResponseType.DEACTIVATE);
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.DEACTIVATE, ex);
                            }
                        }

                        else if (action.Equals("reactivate"))
                        {
                            try
                            {
                                mem = new Member(request);

                                mem = MemberDB.Reactivate(mem);

                                response = BuildResponse(mem, ResponseType.REACTIVATE);
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.REACTIVATE, ex);
                            }
                        }

                        else if (action.Equals("delete"))
                        {
                            try
                            {
                                mem = new Member(request);

                                int rowsAffected = MemberDB.Delete(mem);

                                if (rowsAffected == 0)
                                {
                                    throw new System.Exception("Member's status must be 'Inactive' in order to delete!");
                                }
                                else
                                {
                                    response = BuildResponse(mem, ResponseType.DELETE);
                                }
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.DELETE, ex);
                            }
                        }

                        else if (action.Equals("inquire"))
                        {
                            try
                            {
                                mem = new Member(request);

                                mem = MemberDB.Inquire(mem);

                                response = BuildResponse(mem, ResponseType.INQUIRE);
                            }
                            catch (Exception ex)
                            {
                                response = BuildResponse(mem, ResponseType.INQUIRE, ex);
                            }
                        }

                        if (action.Equals("search"))
                        {
                            try
                            {
                                mem = new Member(request);

                                mem_list = MemberDB.Search(mem);

                                response = BuildResponse(mem_list, ResponseType.SEARCH);
                            }
                            catch (Exception ex)
                            {
                                mem_list = new Member[0];  // create an array to pass
                                response = BuildResponse(mem_list, ResponseType.SEARCH, ex);
                            }
                        }

                        if (action.Equals("list_active"))
                        {
                            try
                            {
                                mem_list = MemberDB.List_Active();

                                response = BuildResponse(mem_list, ResponseType.LIST_ACTIVE);
                            }
                            catch (Exception ex)
                            {
                                mem_list = new Member[0];  // create an array to pass
                                response = BuildResponse(mem_list, ResponseType.LIST_ACTIVE, ex);
                            }
                        }

                        if (action.Equals("list_inactive"))
                        {
                            try
                            {
                                mem_list = MemberDB.List_Inactive();

                                response = BuildResponse(mem_list, ResponseType.LIST_INACTIVE);
                            }
                            catch (Exception ex)
                            {
                                mem_list = new Member[0];  // create an array to pass
                                response = BuildResponse(mem_list, ResponseType.LIST_INACTIVE, ex);
                            }
                        }

                        else if (action.Equals("listStatuses"))
                        {
                            Status[] statuses = StatusDB.List();
                            StringWriter sw = new StringWriter();
                            XmlTextWriter xmlWriter = new XmlTextWriter(sw);
                            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages

                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("response");
                            xmlWriter.WriteElementString("errorCode", "0");
                            xmlWriter.WriteStartElement("statuses");

                            foreach (Status status in statuses)
                            {
                                xmlWriter.WriteStartElement("status");
                                xmlWriter.WriteElementString("status_id", status.ID.ToString());
                                xmlWriter.WriteElementString("status_desc", status.Description);
                                xmlWriter.WriteEndElement();                                // end member
                            }
                            xmlWriter.WriteEndElement();                                // end statuses
                            xmlWriter.WriteEndElement();                                // end response
                            xmlWriter.WriteEndDocument();

                            response = sw.ToString();
                        }

                        else if (action.Equals("listStateCodes"))
                        {
                            StateCode[] states = StateCodeDB.List();
                            StringWriter sw = new StringWriter();
                            XmlTextWriter xmlWriter = new XmlTextWriter(sw);
                            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages

                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("response");
                            xmlWriter.WriteElementString("errorCode", "0");
                            xmlWriter.WriteStartElement("states");

                            foreach (StateCode state in states)
                            {
                                xmlWriter.WriteStartElement("state");
                                xmlWriter.WriteElementString("code", state.Code);
                                xmlWriter.WriteElementString("name", state.Name);
                                xmlWriter.WriteEndElement();                                // end state
                            }

                            xmlWriter.WriteEndElement();                                // end states
                            xmlWriter.WriteEndElement();                                // end response
                            xmlWriter.WriteEndDocument();

                            response = sw.ToString();
                        }

                        // handle the disconnect request
                        // This request should be handled last
                        else if (action.Equals("disconnect"))
                        {
                            m_isListening = false;
                            response = "<response><errorCode>0</errorCode></response>";
                        }
                    }

                    LogMessage("Response: {0}", m_sid, response);
                    m_writer.WriteLine(response);

                    request = String.Empty;
                    response = String.Empty;
                }
                catch (IOException ex)
                {
                    LogMessage("Client disconnected without saying goodbye.  How rude!", m_sid);
                    m_isListening = false;
                }
                catch (OutOfMemoryException ex)
                {
                    // TO DO:  How do we handle this?
                }
            }

            m_reader.Close();
            m_writer.Close();
            m_client.Close();

            LogMessage("Session {0} disconnected.", m_sid, m_session_id.ToString());
        }

        // A utility method that returns the contents of a node.  This is a fairly reusable 
        // method.  Where should it be placed?
        private static String getNodeText(String xPath, String xml)
        {
            String value = String.Empty;
            XmlNode xNode = null;
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                // load the xml from a string
                xmlDoc.LoadXml(xml);

                xNode = xmlDoc.SelectSingleNode(xPath);		// extract a single node based upon the XPATH string provided 

                if (xNode != null)
                {
                    value = xNode.InnerText;	            // save the text; otherwise return null
                }
            }
            catch (XmlException ex)
            {
                value = null;
            }
            catch (Exception ex)
            {
                value = null;
            }


            return value;
        }

        private static String BuildResponse(Member[] mem_list, ResponseType responseType, Exception ex = null)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(sw);
            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages

            switch (responseType)
            {
                case ResponseType.SEARCH:
                    if (ex == null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("members");

                        foreach (Member mem in mem_list)
                        {

                            xmlWriter.WriteStartElement("member");
                            xmlWriter.WriteElementString("mem_id", mem.ID.ToString());
                            xmlWriter.WriteElementString("mem_first_name", mem.FirstName);
                            xmlWriter.WriteElementString("mem_last_name", mem.LastName);
                            xmlWriter.WriteElementString("mem_address", mem.Address);
                            xmlWriter.WriteElementString("mem_city", mem.City);
                            xmlWriter.WriteElementString("mem_state_code", mem.State);
                            xmlWriter.WriteElementString("mem_zip", mem.ZIP);
                            xmlWriter.WriteElementString("mem_phone", mem.Phone);
                            xmlWriter.WriteElementString("mem_email", mem.Email);
                            xmlWriter.WriteElementString("mem_status_id", mem.StatusID.ToString());
                            xmlWriter.WriteEndElement();                                // end member
                        }


                        xmlWriter.WriteEndElement();                                // end members
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                     // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;
                case ResponseType.LIST_ACTIVE:
                    if (ex == null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("members");

                        foreach (Member mem in mem_list)
                        {

                            xmlWriter.WriteStartElement("member");
                            xmlWriter.WriteElementString("mem_id", mem.ID.ToString());
                            xmlWriter.WriteElementString("mem_first_name", mem.FirstName);
                            xmlWriter.WriteElementString("mem_last_name", mem.LastName);
                            xmlWriter.WriteElementString("mem_address", mem.Address);
                            xmlWriter.WriteElementString("mem_city", mem.City);
                            xmlWriter.WriteElementString("mem_state_code", mem.State);
                            xmlWriter.WriteElementString("mem_zip", mem.ZIP);
                            xmlWriter.WriteElementString("mem_phone", mem.Phone);
                            xmlWriter.WriteElementString("mem_email", mem.Email);
                            xmlWriter.WriteElementString("mem_status_id", mem.StatusID.ToString());
                            xmlWriter.WriteEndElement();                                // end member
                        }


                        xmlWriter.WriteEndElement();                                // end members
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                     // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;
                case ResponseType.LIST_INACTIVE:
                    if (ex == null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("members");

                        foreach (Member mem in mem_list)
                        {

                            xmlWriter.WriteStartElement("member");
                            xmlWriter.WriteElementString("mem_id", mem.ID.ToString());
                            xmlWriter.WriteElementString("mem_first_name", mem.FirstName);
                            xmlWriter.WriteElementString("mem_last_name", mem.LastName);
                            xmlWriter.WriteElementString("mem_address", mem.Address);
                            xmlWriter.WriteElementString("mem_city", mem.City);
                            xmlWriter.WriteElementString("mem_state_code", mem.State);
                            xmlWriter.WriteElementString("mem_zip", mem.ZIP);
                            xmlWriter.WriteElementString("mem_phone", mem.Phone);
                            xmlWriter.WriteElementString("mem_email", mem.Email);
                            xmlWriter.WriteElementString("mem_status_id", mem.StatusID.ToString());
                            xmlWriter.WriteEndElement();                                // end member
                        }


                        xmlWriter.WriteEndElement();                                // end members
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                     // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;
            }

            return sw.ToString();

        }

        private static String BuildResponse(Member mem, ResponseType responseType, Exception ex = null)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(sw);
            xmlWriter.Formatting = Formatting.None;  // Set to none to enable the use of readline and writeline to send complete messages

            switch (responseType)
            {
                case ResponseType.INQUIRE:
                    if (ex == null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteElementString("mem_id", mem.ID.ToString());
                        xmlWriter.WriteElementString("mem_first_name", mem.FirstName);
                        xmlWriter.WriteElementString("mem_last_name", mem.LastName);
                        xmlWriter.WriteElementString("mem_address", mem.Address);
                        xmlWriter.WriteElementString("mem_city", mem.City);
                        xmlWriter.WriteElementString("mem_state_code", mem.State);
                        xmlWriter.WriteElementString("mem_zip", mem.ZIP);
                        xmlWriter.WriteElementString("mem_phone", mem.Phone);
                        xmlWriter.WriteElementString("mem_email", mem.Email);
                        xmlWriter.WriteElementString("mem_status_id", mem.StatusID.ToString());
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;

                case ResponseType.ADD:
                    if (ex == null) // This is a GOOD response!
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteElementString("mem_id", mem.ID.ToString());
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }

                    break;

                case ResponseType.UPDATE:
                    if (ex == null) // This is a GOOD response!
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;

                case ResponseType.DEACTIVATE:
                    if (ex == null) // This is a GOOD response!
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;

                case ResponseType.REACTIVATE:
                    if (ex == null) // This is a GOOD response!
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;

                case ResponseType.DELETE:
                    if (ex == null) // This is a GOOD response!
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "0");
                        xmlWriter.WriteStartElement("member");
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    else
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("response");
                        xmlWriter.WriteElementString("errorCode", "-1");            // there is an error!
                        xmlWriter.WriteElementString("errorMessage", ex.Message);
                        xmlWriter.WriteStartElement("member");                      // if we have an error we likely do not have an id.
                        xmlWriter.WriteEndElement();                                // end member
                        xmlWriter.WriteEndElement();                                // end response
                        xmlWriter.WriteEndDocument();
                    }
                    break;
            }

            return sw.ToString();

        }
    }
}
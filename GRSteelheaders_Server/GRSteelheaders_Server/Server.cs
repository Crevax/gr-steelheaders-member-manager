using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace GRSteelheaders_Server
{
    class Server
    {
        private static int m_port = 5000;
        private static Boolean m_isListening = true;
        private static TcpClient client = null;
        private static TcpListener listener = null;
        public static LogWriter logWriter = null;

        public static String workingDirectory = System.IO.Directory.GetCurrentDirectory();
        public static String dbProvider = "Microsoft.ACE.OLEDB.12.0";
        public static String dbName = "GRSteelheaders_Members.accdb";
        public static String dbConnString = String.Format(@"Provider={0};Data Source={1}\Data\{2}", dbProvider, workingDirectory, dbName);



        // Log a message to the screen and to a file (TO DO).  Provides an example of using 
        // a variable number of parameters like Console.WriteLine
        private static void LogMessage(String message, params object[] args)
        {
            // Apply the formatting requested by the user
            String s = String.Format(message, args);

            // add the date/time to the message 
            Console.WriteLine("{0}\t{1}", DateTime.Now.ToString(), s);

            //write a log file entry
            if (logWriter == null)
                logWriter = LogWriter.getInstance();

            logWriter.WriteLogMessage(String.Format("{0}\t{1}", DateTime.Now.ToString(), s));
        }

        static void Main(string[] args)
        {
            // if there are parameters, figure out what was passed... This code could be put into a method call to clean-up the main method...
            if (args.Length > 0)
            {
                if (args[0].StartsWith("-p:")) // setting a custom port...
                {
                    String integerPattern = "^\\d+$";       // matches one or more digit characters [0-9]
                    String value = args[0].Split(':')[1];   // extract the port from the string, e.g. "-p:6000"

                    int port = 5000;
                    // Check to see if the value is an integer
                    if (Regex.IsMatch(value, integerPattern))
                    {
                        port = Convert.ToInt32(value);
                        if (port > 0 && port <= 65535)  // verify that the port requested is within range
                        {
                            m_port = port;  // use the overridden port; otherwise use the default port of 5000.
                        }
                    }
                    else
                        m_port = port;
                }
            }


            // Listen to any IP address on the local machine
            listener = new TcpListener(System.Net.IPAddress.Any, m_port);

            // Start the listener
            listener.Start();
            LogMessage("Server Startup Initiated at {0}", DateTime.Now);

            LogMessage("Current working Directory: {0}", Server.workingDirectory);
            LogMessage("Database Provider: {0}", Server.dbProvider);
            LogMessage("Database Name: {0}", Server.dbName);
            LogMessage("Database Connection String: {0}", dbConnString);

            while (m_isListening)
            {
                LogMessage("Awaiting connection on port {0}.", m_port);
                client = listener.AcceptTcpClient();
                LogMessage("Accepted a Connection from: {0}.", client.Client.RemoteEndPoint.ToString());

                // Create a new session
                Session s = new Session(client);

                // Create a new thread and start it to it will execute asynchronously
                Thread newThread;
                newThread = new System.Threading.Thread(s.Run);
                newThread.Start();
            }

            LogMessage("Server Shutdown Initiated.");
            listener.Stop();
            LogMessage("Server Shutdown Completed at ", DateTime.Now);
        }
    }
}

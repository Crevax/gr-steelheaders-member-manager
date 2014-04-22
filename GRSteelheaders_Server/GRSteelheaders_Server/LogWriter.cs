using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GRSteelheaders_Server
{
    public class LogWriter
    {
        private static StreamWriter writer;
        private static LogWriter logWriter;

        private LogWriter()
        {
            writer = new StreamWriter(@".\AppServer.log", true);  // Append to the file
            writer.AutoFlush = true;                              // This can be turned off on a production system; having it on for debugging is important
        }

        public static LogWriter getInstance()
        {
            if (logWriter == null)
                logWriter = new LogWriter();

            return logWriter;
        }

        public void WriteLogMessage(String message)
        {
            writer.WriteLine(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace GRSteelheaders_Client
{
    class ServerHelper
    {
        public static TcpClient client;
        public static StreamReader reader;
        public static StreamWriter writer;
    }
}

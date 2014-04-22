using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace GRSteelheaders_Server
{
    class StatusDB
    {
        public static Status[] List()
        {
            String result = String.Empty;
            LinkedList<Status> list = new LinkedList<Status>();

            OleDbConnection cn = new OleDbConnection(Server.dbConnString);

            String strSQL = "SELECT Status_ID, Status_Desc FROM Statuses;";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                list.AddLast(new Status(Convert.ToInt32(dr["Status_ID"]), dr["Status_Desc"].ToString()));
            }

            cn.Close();

            return list.ToArray();
        }
    }
}

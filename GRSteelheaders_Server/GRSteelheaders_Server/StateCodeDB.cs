using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace GRSteelheaders_Server
{
    public class StateCodeDB
    {
        public static StateCode[] List()
        {
            String result = String.Empty;
            LinkedList<StateCode> list = new LinkedList<StateCode>();

            OleDbConnection cn = new OleDbConnection(Server.dbConnString);

            String strSQL = "SELECT State_Code, State_name FROM States;";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                list.AddLast(new StateCode(dr["State_Code"].ToString(), dr["State_Name"].ToString()));
            }

            cn.Close();

            return list.ToArray();
        }


    }
}

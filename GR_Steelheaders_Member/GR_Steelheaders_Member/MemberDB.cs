using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace GR_Steelheaders_Member
{
    public class MemberDB
    {
        public static String connString;

        public static Member[] Search(Member mem)
        {

            LinkedList<Member> list = new LinkedList<Member>();

            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "SELECT Mem_ID, Mem_First_Name, Mem_Last_Name, Mem_Address, Mem_City, Mem_State_Code, Mem_Zip_Code, Mem_Phone_Number, Mem_Email, Mem_Status_ID FROM Members WHERE Mem_Last_Name LIKE @Mem_Last_Name";
            
            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            OleDbParameter pm = new OleDbParameter("@Mem_Last_Name", OleDbType.Char, 40);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.LastName + "%";       // Append the wildcard!
            cm.Parameters.Add(pm);

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                mem = new Member(dr);
                list.AddLast(mem);
            }

            cn.Close();

            return list.ToArray();
        }

        public static Member[] List_Active()
        {

            LinkedList<Member> list = new LinkedList<Member>();

            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "SELECT Mem_ID, Mem_First_Name, Mem_Last_Name, Mem_Address, Mem_City, Mem_State_Code, Mem_Zip_Code, Mem_Phone_Number, Mem_Email, Mem_Status_ID FROM Members WHERE Mem_Status_ID <> 4";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Member mem = new Member(dr);
                list.AddLast(mem);
            }

            cn.Close();

            return list.ToArray();
        }

        public static Member[] List_Inactive()
        {

            LinkedList<Member> list = new LinkedList<Member>();

            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "SELECT Mem_ID, Mem_First_Name, Mem_Last_Name, Mem_Address, Mem_City, Mem_State_Code, Mem_Zip_Code, Mem_Phone_Number, Mem_Email, Mem_Status_ID FROM Members WHERE Mem_Status_ID = 4";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read())
            {
                Member mem = new Member(dr);
                list.AddLast(mem);
            }

            cn.Close();

            return list.ToArray();
        }

        public static Member Inquire(Member mem)
        {

            LinkedList<Member> list = new LinkedList<Member>();

            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "SELECT Mem_ID, Mem_First_Name, Mem_Last_Name, Mem_Address, Mem_City, Mem_State_Code, Mem_Zip_Code, Mem_Phone_Number, Mem_Email, Mem_Status_ID FROM Members WHERE Mem_ID = @Mem_ID;";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);
            OleDbDataReader dr;

            OleDbParameter pm = new OleDbParameter("@Mem_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ID;
            cm.Parameters.Add(pm);

            cn.Open();

            dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            if (dr.Read())
                mem = new Member(dr);

            cn.Close();

            return mem;
        }

        public static Member Add(Member mem)
        {
            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "INSERT INTO Members ";
            strSQL += "( ";
            strSQL += "Mem_First_Name,";
            strSQL += "Mem_Last_Name,";
            strSQL += "Mem_Address,";
            strSQL += "Mem_City,";
            strSQL += "Mem_State_Code,";
            strSQL += "Mem_Zip_Code,";
            strSQL += "Mem_Phone_Number,";
            strSQL += "Mem_Email,";
            strSQL += "Mem_Status_ID";
            strSQL += ") ";
            strSQL += "VALUES";
            strSQL += "(";
            strSQL += "@Mem_First_Name,";
            strSQL += "@Mem_Last_Name,";
            strSQL += "@Mem_Address,";
            strSQL += "@Mem_City,";
            strSQL += "@Mem_State_Code,";
            strSQL += "@Mem_Zip_Code,";
            strSQL += "@Mem_Phone_Number,";
            strSQL += "@Mem_Email,";
            strSQL += "@Mem_Status_ID";
            strSQL += ")";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);

            OleDbParameter pm = new OleDbParameter("@Mem_First_Name", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.FirstName;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Last_Name", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.LastName;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Address", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Address;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_City", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.City;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_State_Code", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.State;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Zip_Code", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ZIP;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Phone_Number", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Phone;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Email", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Email;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Status_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.StatusID;
            cm.Parameters.Add(pm);

            cn.Open();

            // execute the update
            cm.ExecuteNonQuery();

            // obtain the auto number
            cm = new OleDbCommand("SELECT @@IDENTITY;", cn);

            mem.ID = Convert.ToInt32(cm.ExecuteScalar());

            cn.Close();

            return mem;
        }

        public static Member Update(Member mem)
        {
            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "UPDATE Members ";
            strSQL += "SET Mem_First_Name = @Mem_First_Name,";
            strSQL += "Mem_Last_Name = @Mem_Last_Name,";
            strSQL += "Mem_Address = @Mem_Address,";
            strSQL += "Mem_City = @Mem_City,";
            strSQL += "Mem_State_Code = @Mem_State_Code,";
            strSQL += "Mem_Zip_Code = @Mem_Zip_Code,";
            strSQL += "Mem_Phone_Number = @Mem_Phone_Number,";
            strSQL += "Mem_Email = @Mem_Email,";
            strSQL += "Mem_Status_ID = @Mem_Status_ID ";
            strSQL += "WHERE Mem_ID = @Mem_ID";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);

            OleDbParameter pm = new OleDbParameter("@Mem_First_Name", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.FirstName;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Last_Name", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.LastName;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Address", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Address;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_City", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.City;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_State_Code", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.State;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Zip_Code", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ZIP;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Phone_Number", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Phone;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Email", OleDbType.Char, 255);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.Email;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_Status_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.StatusID;
            cm.Parameters.Add(pm);

            pm = new OleDbParameter("@Mem_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ID;
            cm.Parameters.Add(pm);

            cn.Open();

            // execute the update
            cm.ExecuteNonQuery();

            cn.Close();

            return mem;
        }

        public static Member Deactivate(Member mem)
        {
            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "UPDATE Members ";
            strSQL += "Mem_Status_ID = 4";
            strSQL += "WHERE MEM_ID = @MEM_ID";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);

            OleDbParameter pm = new OleDbParameter("@Mem_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ID;
            cm.Parameters.Add(pm);

            cn.Open();

            // execute the update
            cm.ExecuteNonQuery();

            cn.Close();

            return mem;
        }

        public static Member Reactivate(Member mem)
        {
            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "UPDATE Members ";
            strSQL += "Mem_Status_ID = 2";
            strSQL += "WHERE Mem_ID = @Mem_ID;";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);

            OleDbParameter pm = new OleDbParameter("@Mem_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ID;
            cm.Parameters.Add(pm);

            cn.Open();

            // execute the update
            cm.ExecuteNonQuery();

            cn.Close();

            return mem;
        }

        public static int Delete(Member mem)
        {
            OleDbConnection cn = new OleDbConnection(connString);

            String strSQL = "DELETE FROM Members ";
            strSQL += "WHERE Mem_ID = @Mem_ID AND Mem_Status_ID = 4;";

            OleDbCommand cm = new OleDbCommand(strSQL, cn);

            OleDbParameter pm = new OleDbParameter("@Mem_ID", OleDbType.Integer);
            pm.Direction = ParameterDirection.Input;
            pm.Value = mem.ID;
            cm.Parameters.Add(pm);

            cn.Open();

            // execute the update
            int rowsAffected = cm.ExecuteNonQuery();

            cn.Close();

            return rowsAffected;
        }
    }
}

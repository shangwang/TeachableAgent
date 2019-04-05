using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace Nico.csharp.functions
{
    public class SQLAgent
    {

        public bool agent { get; set; }
        public string agentname { get; set; }

        public static bool GetAgentSetting(string userid)
        {
            string queryString = "Select agent From NicoDB.dbo.USERS Where NicoDB.dbo.USERS.UserID = @UserID";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            bool agent = true;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@UserID", userid);
                    agent = Convert.ToBoolean(cmd.ExecuteScalar());
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLUpdateAgent GetAgent", 0, userid);
            }

            return agent; 
        }

        public static void SetAgent(string agent, string userid)
        {

            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "UPDATE NicoDB.dbo.USERS SET agent = @agent WHERE NicoDB.dbo.USERS.UserID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@agent", agent);
                cmd.ExecuteNonQuery();

                connection.Close();

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLUpdateCondition UpdateAgent", 0, userid);
            }
        }

        public static string GetHubID(string userid)
        {
            string queryString = "Select hubid From NicoDB.dbo.USERS Where NicoDB.dbo.USERS.UserID = @UserID";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            string hubid = "";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@UserID", userid);
                    hubid = Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLUpdateAgent GetHubID", 0, userid);
            }

            return hubid;
        }

        public static void SetHubID(string hubid, string userid)
        {

            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "UPDATE NicoDB.dbo.USERS SET HUBID = @hubid WHERE NicoDB.dbo.USERS.UserID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@hubid", hubid);
                cmd.ExecuteNonQuery();

                connection.Close();

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLUpdateCondition UpdateHubID", 0, userid);
            }
        }
    }
}
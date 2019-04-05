using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace Nico.csharp.functions
{
    public class SQLLog
    {
        public DateTime LogTime { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string UserID { get; set; }
        public int ProblemID { get; set; }
        public int StepID { get; set; }
        public int SessionID { get; set; }
        public string FormName { get; set; }
        public string StackTrace { get; set; }

        public static void InsertLog(DateTime logtime, string message, string stacktrace, string formname, int errorcode, string userid = "nlubold", int problemid = 1, int stepid = 1, int sessionid = 1)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = null;
            connetionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            connection = new SqlConnection(connetionString);
            sql = "Insert into NicoDB.dbo.Error_Log Values(@DateTime, @UserID, @ProblemID, @StepID, @SessionID, @ErrorCode, @ErrorMessage, @FormName, @StackTrace)";
            SqlCommand cmd = new SqlCommand(sql, connection);

            connection.Open();

            cmd.Parameters.AddWithValue("@DateTime", logtime);
            cmd.Parameters.AddWithValue("@UserID", userid);
            cmd.Parameters.AddWithValue("@ProblemID", problemid);
            cmd.Parameters.AddWithValue("@StepID", stepid);
            cmd.Parameters.AddWithValue("@SessionID", sessionid);
            cmd.Parameters.AddWithValue("@ErrorCode", errorcode);
            cmd.Parameters.AddWithValue("@ErrorMessage", message);
            cmd.Parameters.AddWithValue("@FormName", formname);
            cmd.Parameters.AddWithValue("@StackTrace", stacktrace);



            cmd.ExecuteNonQuery();

            connection.Close();
        }

    }
}
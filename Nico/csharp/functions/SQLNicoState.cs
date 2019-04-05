using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;


namespace Nico.csharp.functions
{
    public class SQLNicoState
    {
        public DateTime LogTime { get; set; }
        public string UserID { get; set; }
        public Tuple<string, int, string> NicoResponse { get; set; }
        public List<int> problemStep { get; set; }

        
        // in Nico's response, string is path to the file that contains his response - we need to read this to log it in the DB
        // int is the movement code, second int is whether we update the problem step
        public static void UpdateNicoState(string userid, Tuple<string, int, string> nicoResponse, List<int> problemStep, DateTime time)
        {
            int sessionid = 1;                                                                  // ****** FIX ********

            string transcript = nicoResponse.Item1;
            int problem = problemStep[0];
            int step = problemStep[1];
            int answerKey = problemStep[3];
            int movementCode = nicoResponse.Item2;
            string answeredStep = nicoResponse.Item3;

            
            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "Insert into NicoDB.dbo.Nico_State Values(@UserID, @DateTime, @SessionID, @ProblemID, @StepID, @NicoMovement, @AnswerKey, @AnsweredStep, @NicoResponse, @FilePath)";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@DateTime", time);
                cmd.Parameters.AddWithValue("@SessionID", sessionid);
                cmd.Parameters.AddWithValue("@ProblemID", problem);
                cmd.Parameters.AddWithValue("@StepID", step);
                cmd.Parameters.AddWithValue("@NicoResponse", transcript);
                cmd.Parameters.AddWithValue("@NicoMovement", movementCode);
                cmd.Parameters.AddWithValue("@AnswerKey", answerKey);
                cmd.Parameters.AddWithValue("@AnsweredStep", answeredStep);
                cmd.Parameters.AddWithValue("@FilePath", nicoResponse.Item1);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch(Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLNicoState UpdateState", 0, userid);
            }

        }


        private static string readResponse(string path)
        {
            if (path == "")
            {
                return "empty transcript";
            }
            else
            {
                return File.ReadAllText(path);
            }
        }

        public static string ReadNicoState_Answer(string userid)
        {
            string NicoStateAnswer = "";

            string queryString = "SELECT TOP 1 AnsweredStep FROM NicoDB.dbo.Nico_State ORDER BY NicoStateKey DESC";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();

                    NicoStateAnswer = (string)cmd.ExecuteScalar();

                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLNicoState ReadAnswerVal", 0, userid);
            }
            return NicoStateAnswer;
        }

    }
}
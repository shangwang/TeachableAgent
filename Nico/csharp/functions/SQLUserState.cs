using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace Nico.csharp.functions
{
    public class SQLUserState
    {
        public DateTime StartTime { get; set; }
        public string UserID { get; set; }
        public string DialogueAct { get; set; }
        public string Transcript { get; set; }
        public int SpeakerSpoke { get; set; }
        public List<int> ProblemStep { get; set; }
        int numautoresponses { get; set; }

        public static void UpdateSpeakerState(string userid, string dialogueAct, string transcript, int speakerSpoke, List<int> problemStep, DateTime starttime, string clickstep, int numautoresponses)
        {
            int sessionid = 1;                                                                  // ****** FIX ********

            int problem = problemStep[0];
            int step = problemStep[1];
            int answerKey = problemStep[3];
            int confidence = 50;
            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "Insert into NicoDB.dbo.User_State Values(@UserID, @DateTime, @SessionID, @ProblemID, @StepID, @DialogueAct, @DialogueActConfidence, @Spoke, @StepAnswerKey, @ClickStep, @NumAutoResponses, @transcript)";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@DateTime", starttime);
                cmd.Parameters.AddWithValue("@SessionID", sessionid);
                cmd.Parameters.AddWithValue("@ProblemID", problem);
                cmd.Parameters.AddWithValue("@StepID", step);
                cmd.Parameters.AddWithValue("@DialogueAct", dialogueAct);
                cmd.Parameters.AddWithValue("@DialogueActConfidence", confidence);
                cmd.Parameters.AddWithValue("@Spoke", speakerSpoke);
                cmd.Parameters.AddWithValue("@StepAnswerKey", answerKey);
                cmd.Parameters.AddWithValue("@ClickStep", clickstep);
                cmd.Parameters.AddWithValue("@NumAutoResponses", numautoresponses);
                cmd.Parameters.AddWithValue("@transcript", transcript);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLUserState UpdateSpeakerState", 0, userid);
            }
        }



    }
}
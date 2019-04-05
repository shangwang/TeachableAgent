using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace Nico.csharp.functions
{
    public class SQLProblemStepTracker
    {
        public string UserID { get; set; }
        public int ProblemID { get; set; }
        public int StepID { get; set; }
        public int SessionID { get; set; }
        public int NumTimesProb { get; set; }
        public int NicoAnswered { get; set; }
        public int ProblemImgKey { get; set; }
        public int NumAutoResponses { get; set; }
        public int NumTurns { get; set; }

        public static void UpdateProbStep(string userid, int sessionid, int problemid, int stepid, int problemimgkey, int stepanswerkey, int newanswer, int numautoresponses, int numturns)
        {
            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "UPDATE NicoDB.dbo.Problem_Step_Tracker SET ProblemID=@ProblemID, StepID = @StepID, ProblemImgKey = @ProblemImgKey, StepAnswerKey = @StepAnswerKey, NewAnswer = @NewAnswer, NumAutoResponses = @NumAutoResponses, NumTurns = @NumTurns WHERE NicoDB.dbo.Problem_Step_Tracker.UserID = @UserID and NicoDB.dbo.Problem_Step_Tracker.SessionID = @SessionID";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@SessionID", sessionid);
                cmd.Parameters.AddWithValue("@ProblemID", problemid);
                cmd.Parameters.AddWithValue("@StepID", stepid);
                cmd.Parameters.AddWithValue("@ProblemImgKey", problemimgkey);
                cmd.Parameters.AddWithValue("@StepAnswerKey", stepanswerkey);
                cmd.Parameters.AddWithValue("@NewAnswer", newanswer);
                cmd.Parameters.AddWithValue("@NumAutoResponses", numautoresponses);
                cmd.Parameters.AddWithValue("@NumTurns", numturns);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemStepTracker UpdateProbStep", 0, userid);
            }
        }

        public static void UpdateNumTurns(string userid, int numturns)
        {
            try
            {
                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "UPDATE NicoDB.dbo.Problem_Step_Tracker SET NumTurns = @NumTurns WHERE NicoDB.dbo.Problem_Step_Tracker.UserID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@NumTurns", numturns);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemStepTracker UpdateProbStep", 0, userid);
            }
        }


        // This is to INSERT new records into the problem step tracker - if ever enable registration, call this and create records for user for each of the problems
        public static void WriteNewProbStep(string userid, int sessionid, int problemid, int stepid, int problemimgkey, int stepanswerkey, int newanswer, int numautoresponses, int numturns)
        {
            try
            {

                string connectionString = null;
                SqlConnection connection;
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;
                connectionString = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                connection = new SqlConnection(connectionString);
                sql = "Insert into NicoDB.dbo.Problem_Step_Tracker Values(UserID = @UserID, SessionID = @SessionID, ProblemID = @ProblemID, StepID = @StepID, ProblemImgKey = @ProblemImgKey, StepAnswerKey = @StepAnswerKey, NewAnswer = @NewAnswer, NumAutoResponses = @NumAutoResponses, NumTurns = @NumTurns";
                SqlCommand cmd = new SqlCommand(sql, connection);

                connection.Open();

                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@SessionID", sessionid);
                cmd.Parameters.AddWithValue("@ProblemID", problemid);
                cmd.Parameters.AddWithValue("@StepID", stepid);
                cmd.Parameters.AddWithValue("@ProblemImgKey", problemimgkey);
                cmd.Parameters.AddWithValue("@StepAnswerKey", stepanswerkey);
                cmd.Parameters.AddWithValue("@NewAnswer", newanswer);
                cmd.Parameters.AddWithValue("@NumAutoResponses", numautoresponses);
                cmd.Parameters.AddWithValue("@NumTurns", numturns);
                cmd.ExecuteNonQuery();

                connection.Close();

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemStepTracker WriteNewProbStep", 0, userid);
            }
        }

        /* 
            * Returns an int array where index 0 = problem, 1 = step, 3 = problemimagekey that was loaded for this step, 4 = the step answer pattern for this step
        */
        public static List<int> ReadProbStep(string userid)
        {
            List<int> problemStep = new List<int>();                            

            string queryString = "Select * From NicoDB.dbo.Problem_Step_Tracker Where NicoDB.dbo.Problem_Step_Tracker.UserID = @UserID";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@UserID", userid);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        problemStep = ReadSingleRow((IDataRecord)reader);
                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemStepTracker ReadProbStep", 0, userid);
            }
            return problemStep;
        }

        private static List<int> ReadSingleRow(IDataRecord record)
        {
            List<int> problemStep = new List<int>();
            for (int i = 3; i < record.FieldCount; i++)
            {
                problemStep.Add(Convert.ToInt32(record[i]));                      
            }
            return problemStep;
        }


        // This function takes whether this step has been answered (current answer) and the current answer key
        // Retrieves the 'answer pattern' associated with that answer key
        // and calculates the new pattern based on whether the step was answered. Returns the key associated with this pattern
        public static int CalculateNewAnswerKey(int currentanswer, int answerkey, int step, string userid)
        {
            string currentpattern = SQLAnswerPattern.GetAnswerPattern(answerkey, userid)[1];
            int newAnswerKey = 0;
            StringBuilder sb = new StringBuilder(currentpattern);
            char[] ca = currentanswer.ToString().ToCharArray();
            sb[step] = ca[0];
            currentpattern = sb.ToString();
            newAnswerKey = SQLAnswerPattern.GetAnswerKey(currentpattern, userid);
            return newAnswerKey;
        }

    }
}
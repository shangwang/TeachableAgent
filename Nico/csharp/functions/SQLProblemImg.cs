using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Nico.csharp.functions
{
    public class SQLProblemImg
    {
        public int ProblemID { get; set; }
        public int StepID { get; set; }
        public string FilePath { get; set; }
        public int CorrectnessCode { get; set; }
        public int Answer { get; set; }
        public int NicoAnswered { get; set; }

        /* 
         * Returns the filepath of a particular image for a given problem / step by searching against the problem id, step id, whether the answer is correct, 
         * and the order of answers seen. 
         * Currently there is a column for whether this image contains the answer to that step. 
        */
        public static Tuple<string, int> FetchImgPath(int problemid, int stepid, int correctnesscode = 0, int stepanswerkey = 1)
        {
            Tuple<string, int> result = new Tuple<string, int>("",2);             // key to the filepath for this image, to be stored in the problem step tracker. Start at 2 because that's the first key in the DB
            try { 
                string queryString = "Select * From NicoDB.dbo.Problem_Step_Img Where NicoDB.dbo.Problem_Step_Img.ProblemID = @ProblemID and NicoDB.dbo.Problem_Step_Img.StepID = @StepID " +
                    "and NicoDB.dbo.Problem_Step_Img.CorrectnessCode = @CorrectnessCode and NicoDB.dbo.Problem_Step_Img.StepAnswerKey = @StepAnswerKey";
                string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@ProblemID", problemid);
                    cmd.Parameters.AddWithValue("@StepID", stepid);
                    cmd.Parameters.AddWithValue("@CorrectnessCode", correctnesscode);
                    cmd.Parameters.AddWithValue("@StepAnswerKey", stepanswerkey);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result = ReadSingleRow((IDataRecord)reader);
                    
                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "SQLProblemImg FetchImgPath", 1);
            }
            return result;
        }
        /* 
         * Returns the filepath of a particular image for a given problem / step given the key to the image. 
        */
        public static Tuple<string, int> FetchImgPathByKey(int problemimgkey, string userid)
        {
            Tuple<string, int> result = new Tuple<string, int>("", 2);             // key to the filepath for this image, to be stored in the problem step tracker. Start at 2 because that's the first key in the DB
            string queryString = "Select * From NicoDB.dbo.Problem_Step_Img Where NicoDB.dbo.Problem_Step_Img.ProblemImgKey = @ProblemImgKey";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@ProblemImgKey", problemimgkey);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result = ReadSingleRow((IDataRecord)reader);

                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemImg FetchImgPathByKey", 0, userid);
            }
            return result;
        }

        private static Tuple<string,int> ReadSingleRow(IDataRecord record)
        {
            return new Tuple<string, int>(record[3].ToString(), Convert.ToInt32(record[0]));
        }
    }
}
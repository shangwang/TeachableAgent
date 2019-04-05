using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Nico.csharp.functions
{
    public class SQLAnswerPattern
    {

        public int AnswerKey { get; set; }
        public int AnswerPattern { get; set; }
        public static List<string> GetAnswerPattern(int answerKey, string userid)
        {
            List<string> answerInfo = new List<string>();

            string queryString = "Select * From NicoDB.dbo.AnswerPatterns Where NicoDB.dbo.AnswerPatterns.AnswerPatternKey = @AnswerKey";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@AnswerKey", answerKey);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        answerInfo = ReadSingleRow((IDataRecord)reader);
                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLAnswerPattern GetAnswerPattern", 0, userid);
            }
            return answerInfo;
        }

        public static int GetAnswerKey(string answerpattern, string userid)
        {
            int answerkey = 0;

            string queryString = "Select AnswerPatternKey From NicoDB.dbo.AnswerPatterns Where NicoDB.dbo.AnswerPatterns.AnswerPattern = @AnswerPattern";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            try
            {

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@AnswerPattern", answerpattern);
                    answerkey = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLAnswerPattern GetAnswerKey", 0, userid);
            }
            return answerkey;
        }

        private static List<string> ReadSingleRow(IDataRecord record)
        {
            List<string> answerInfo = new List<string>();
            //problemStep[0] = Convert.ToInt32(record[0]);
            for (int i = 0; i < record.FieldCount; i++)
            {
                answerInfo.Add(record[i].ToString());
            }
            return answerInfo;
        }
    }
}
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
    public class SQLProblemDesign
    {
        public int ProblemID { get; set; }
        public int StepID { get; set; }
        public int NumSteps { get; set; }
        public int NumCells { get; set; }
        public int Answer { get; set; }

        public static List<string> GetProbDesign(int problemid, string problemset, int answer, string userid)
        {
            List<string> problemDesign = new List<string>();

            string queryString = "Select * From NicoDB.dbo.Problem_Design Where NicoDB.dbo.Problem_Design.ProblemID = @ProblemID and NicoDB.dbo.Problem_Design.Answer = @Answer AND NicoDB.dbo.Problem_Design.ProblemSet = @ProblemSet";
            string constr = ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand(queryString, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@ProblemID", problemid);
                    cmd.Parameters.AddWithValue("@Answer", answer);
                    cmd.Parameters.AddWithValue("@ProblemSet", problemset);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        problemDesign = ReadSingleRow((IDataRecord)reader);
                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "SQLProblemDesign GetProbDesign", 0, userid);
            }
            return problemDesign;
        }

        private static List<string> ReadSingleRow(IDataRecord record)
        {
            List<string> problemInfo = new List<string>();
            //problemStep[0] = Convert.ToInt32(record[0]);
            for (int i = 4; i < record.FieldCount; i++)
            {
                problemInfo.Add(record[i].ToString());
            }
            return problemInfo;
        }

    }
}
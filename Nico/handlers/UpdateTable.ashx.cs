using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Nico.csharp.functions;

namespace Nico.handlers
{
    /// <summary>
    /// Summary description for UpdateTable
    /// </summary>
    public class UpdateTable : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            /* ----------Retrieve current user & session ID ----------------
             * Should be dynamic - need to fix this
            */
            string userid = HttpContext.Current.User.Identity.Name;
            int sessionid = 1;


            /* ----------Retrieve current problem and step information----------------
             * Problem Step Info: 1st value is the problem, second value contains the current step, third value contains the
             * problem image key (may remove this, currently unused), fourth value contains the key to the current answer pattern,
             * fifth value indicates whether Nico just answered this step. 
            */
            string problemset = SQLConditionGenderInfo.GetProblemSet(userid);
            List<int> problemStep = SQLProblemStepTracker.ReadProbStep(userid);
            int problem = problemStep[0];
            int step = problemStep[1];
            int problemimgkey = problemStep[2];
            int answerKey = problemStep[3];
            int newanswer = problemStep[4];

            /* ----------Retrieve the problem content----------------
             * Problem Text - contains description of problem plus headers for columns
             * Problem Design Answers - contains the full content, including answers
             * Problem Design Blanks - contains the content of the problem with question marks where Nico must provide the answer
             * Answer Pattern - contains the current pattern of answered steps because technically they don't have to go in order
             * ChAnswerPattern - the character array of the current answer pattern
             * Max step - maximum number of steps for the problem
             * Num Rows - number of rows for the problem
             * Num Cells - number of columns for the problem
             * Step Answer Pattern - indicates which cells need to be answered
            */
            List<string> problemText = SQLProblemText.ReadProbText(problem, problemset, userid);
            List<string> problemDesign_Answers = SQLProblemDesign.GetProbDesign(problem, problemset, 1, userid);
            List<string> problemDesign_Blanks = SQLProblemDesign.GetProbDesign(problem, problemset, 0, userid);

            if (answerKey == 0)
            {
                // BIG PROBLEM! How did this happen?
                answerKey = 1;
            }

            string answerPattern = SQLAnswerPattern.GetAnswerPattern(answerKey, userid)[1];
            char[] chAnswerPattern = answerPattern.ToCharArray();
            int maxsteps = Convert.ToInt32(problemDesign_Blanks[0]);
            int numrows = Convert.ToInt32(problemDesign_Blanks[1]);
            int numcells = Convert.ToInt32(problemDesign_Blanks[2]);
            char[] stepAnswerPattern = problemDesign_Blanks[3].ToCharArray();


            
            /* ----------Answer display variables----------------
             * Calculate the cell which is a 'new' answer, if there is one. 
             * Will only be a 'new' answer when new answer does not equal zero
             * If new answer is not zero, then it is equal to the step which was answered
             * Can get the cell that is 'new' using step answer pattern
            */
            int cellAnswer = 0;
            if (newanswer != 0)
            {
                cellAnswer = (int)Char.GetNumericValue(stepAnswerPattern[newanswer - 1]);
                newanswer = 1;

            }


            /* ----------Set variables for Nico and User----------------
             * Tuple:
             *      string: Nico's response
             *      int: the movement code
             *      bool: indicates whether Nico answered this step
             * clickstep: string to populate in user log how we got to this page
            */
            Tuple<string, int, bool> nicoResponse;                                                                      
            string path = HttpRuntime.AppDomainAppPath;
            string clickstep = "none";
            

            /* ---------- Create Table Data----------------
             * Create a list of objects which can then be serialized into a single Json object and passed back to client.
             * The list of objects - each object is a row in the table
             * Steps:
             *      (1) Store how many columns, step that currently on, max steps, and if a cell on this step was newly answered
             *      (2) Create header row
             *      (3) Create the rest of the rows
             * Also pass info regarding buttons to display and problem description text (ProblemDescription.Text = problemText[0];)
            */

            List<TableInfo> rowsList = new List<TableInfo>();

            // Step (1): Populate important variables including if new answer and what cell that answer is
            rowsList.Add(new TableInfo(Convert.ToString(numcells), Convert.ToString(step), Convert.ToString(maxsteps), Convert.ToString(newanswer), Convert.ToString(cellAnswer), Convert.ToString(problemText[0]), Convert.ToString(numrows)));            
            
            // Step (2): Create header row
            rowsList.Add(new TableInfo(problemText.GetRange(1, numcells).ToArray()));

            // Step (3): Create the rest of the rows
            int index = 0;
            for (int j = 0; j < numrows; j++)
            {
                if (chAnswerPattern[j] == '1')
                {
                    if (maxsteps == 1 && newanswer == 0)
                    {
                        rowsList.Add(new TableInfo(problemDesign_Blanks.GetRange(4 + index, numcells).ToArray()));
                    }
                    else
                    {
                        rowsList.Add(new TableInfo(problemDesign_Answers.GetRange(4 + index, numcells).ToArray()));
                    }
                }
                else
                {
                    rowsList.Add(new TableInfo(problemDesign_Blanks.GetRange(4+index, numcells).ToArray()));
                }
                index += numcells;
            }

            var json = new JavaScriptSerializer().Serialize(rowsList);
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    
    public class TableInfo
    {
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }

        public TableInfo(string[] columns)
        {
            switch (columns.Length)
            {
                case 1:
                    Col1 = columns[0];
                    break;
                case 2:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    break;
                case 3:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    Col3 = columns[2];
                    break;
                case 4:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    Col3 = columns[2];
                    Col4 = columns[3];
                    break;
                case 5:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    Col3 = columns[2];
                    Col4 = columns[3];
                    Col5 = columns[4];
                    break;
                case 6:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    Col3 = columns[2];
                    Col4 = columns[3];
                    Col5 = columns[4];
                    Col6 = columns[5];
                    break;
                case 7:
                    Col1 = columns[0];
                    Col2 = columns[1];
                    Col3 = columns[2];
                    Col4 = columns[3];
                    Col5 = columns[4];
                    Col6 = columns[5];
                    Col7 = columns[6];
                    break;
                default:
                    break;
            }
        }

        public TableInfo(string numCells, string step, string maxSteps, string answer, string answercell, string probtext, string numRows)
        {
            Col1 = numCells;
            Col2 = step;
            Col3 = maxSteps;
            Col4 = answer;
            Col5 = answercell;
            Col6 = probtext;
            Col7 = numRows;
        }
        

        
    }
}
 
 
 
 
 
 
 
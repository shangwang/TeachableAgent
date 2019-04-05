using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Security;
using System.Diagnostics;
using Nico.csharp.functions;

/* 
* Removing page refresh:
*       - Remove Nico's autopopulating the answer on every refresh - done
*       - Add in 'new answer' - done
*       - Fix bug calling DialogueEngine twice in beginning 
*       - Make everything prettier, buttons and table
*       - Add transitions (transition for step change? using CSS maybe??)
*       
* Other Important:
*       Add a 'first step' tracker - what 'row' should we be starting on? Blue/Green/Yellow paint problem
*       Make the table smaller
*       Make the buttons prettier
*       Change it so not striped but looks kind of like that
*       Bold headers and bold text of highlighted cell
*       Multiple users
*       Fix username - may need to create membership class 
*               string username = context.Request.LogonUserIdentity.Name;
*               MembershipUser currentUser = Membership.GetUser();
*               string username = currentUser.UserName;
*       
* Dialogue:
*       - Draft dialogue such that Nico doesn't answer until 'hears' answer
*       - Need a confirm after answer to move on to the next step
*       - Draft dialogue around timer - trigger 3 - 4 times
*        
* Logging:
*       Make sure we are logging errors for every SQL call possible
*       Write program to sync up logs
* 
* Later:
*       Add acoustic-prosodic info capture
*       Update dialogue act estimation
*       Change problem step info to be stored inside of an object
*       Fix the Response Generation return variables to be objects
*       Fix all other Tuple returns to return objects instead of Tuples
*       Change the talk_with_nico.py to read the text from SQL database - same for chatBot.py
* 
* TESTING:
*           Test indicator that final transcript wasn't posted fully
*       Test timer fully
* 
* Dialogue thoughts
        - need to get 'have we been here before'
        - history - for dialogue act, probaly going to need to pull prior dialogue acts - how are we going to get that?
        - history - for Nico's state, need if student spoke before - how are we going to get that?
* 
* Speaker spoke - #1 means they spoke, #0 means they didn't speak but the file wasn't triggered by a timer, #2 means they did not speak and it was triggered by timer
 */


namespace Nico.handlers
{
    /// <summary>
    /// Summary description for DialogueEngine
    /// </summary>
    public class DialogueEngine : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            // General variables
            DateTime timeStart = DateTime.Now;
            string path = context.Request.PhysicalApplicationPath;
            string userid = HttpContext.Current.User.Identity.Name;
            Tuple<string, int> imageInfo = new Tuple<string, int>("",2);
            string page = "";
            
            // response to transmit back to caller
            string transResponse = "";


            // Variables important to the speaker's state
            string audioFile = path + "data\\userAudio\\blob.wav"; 
            string transcript = "";
            string dialogueAct = "";
            int speakerSpoke = 0;
            List<int> problemStep = new List<int>();                                                                        // [0] is problem, [1] is the step, and [2] is the image key loaded for this step, [3] is the answer pattern
            int sessionid = 1;                                                                                              // ************************* FIX THIS ********
            int nextstepanswerkey = 0;

            // Variables important to Nico's state
            Tuple<string, int, string> nicoResponse;                                                                           // string => Nico's response, int is the movement code, string contains whether Nico is 'answering', 'confirming', or 'not answering'
            
            try
            {
                problemStep = SQLProblemStepTracker.ReadProbStep(userid);                                                   // Get current step 
                int problem = problemStep[0];
                int step = problemStep[1];
                int probImg = problemStep[2];
                int answerKey = problemStep[3];
                int numAutoResponses = problemStep[5];
                int numturns = problemStep[6];
                string clickstep = "none";
                int newanswer = 0;
/*
                if (step == 0)
                {
                    step = 1;
                }
*/
                transcript = context.Request.Params["transcript"];    // Get transcript (if there is one)
                page = context.Request.Params["page_loc"];

                if (context.Request.Files.Count > 0)                                                                       // Write out audio file (if it's there)
                {
                    HttpFileCollection files = context.Request.Files;
                    //audioFile = writeFile(files, path, userid, timeStart);
                }
                if (transcript == "problem start")
                {
                    speakerSpoke = 0;
                    clickstep = "problem start";
                    step = 1;
                }
                else if (transcript == "no response")
                {
                    speakerSpoke = 0;
                    clickstep = "null";
                    numAutoResponses += 1;
                    if (numAutoResponses > 3)
                    {
                        transcript = "Triggered Speech";
                    }
                    else
                    {
                        transcript = "Triggered Speech " + numAutoResponses.ToString();
                    }

                }
                else if (transcript == "HELLO FIRST TIME")
                {
                    speakerSpoke = 0;
                    clickstep = "hello nico start";
                }
                else if ((transcript != "") && (transcript != null))                                                             // Is there a transcript? If there is get the speaker's dialogue act
                {
                    dialogueAct = estimateDialogueAct(transcript);
                    speakerSpoke = 1;                                                                                           // Set that the speaker actually spoke this turn
                }
                else
                {
                    speakerSpoke = 2;
                    transcript = "transcript empty or null";
                }


                // TODO:  Simplify this - NicoResponseText should be MUCH simpler - no need for audio file or any of the audio processing
                if (SQLConditionGenderInfo.GetVoiceText(userid) == "text")
                {
                    nicoResponse = ResponseGeneration.NicoResponseText(path, problemStep, speakerSpoke, transcript, timeStart, page, userid, audioFile);                 // Generate and initiate Nico's response
                    path = nicoResponse.Item1;
                    if (path == "")
                    {
                        transResponse = "I'm sorry!  Can you please try again?";
                    }
                    else
                    {
                        transResponse = File.ReadAllText(path);
                    }
                }
                else
                {

                    nicoResponse = ResponseGeneration.NicoResponse(path, problemStep, speakerSpoke, transcript, timeStart, page, userid, audioFile);                 // Generate and initiate Nico's response
                    //transResponse = "Saved User Wav File!";
                    transResponse = transcript;
                }


                SQLUserState.UpdateSpeakerState(userid, dialogueAct, transcript, speakerSpoke, problemStep, timeStart, clickstep, numAutoResponses);    // Write out speaker state info
                SQLNicoState.UpdateNicoState(userid, nicoResponse, problemStep, timeStart);                                           // Write out Nico's state info & update problem/step


                // Nico response: string => Nico's response, int is the movement code, the boolean indicates whether Nico answered the step
                // To update the step, we check if Nico answered the question. 

                numturns += 1;
                if (nicoResponse.Item3 == "answering")
                {
                    nextstepanswerkey = SQLProblemStepTracker.CalculateNewAnswerKey(1, answerKey, step, userid);                // Passing 1 as first argument because Nico DID answer this step
                    newanswer = step;
                    SQLProblemStepTracker.UpdateProbStep(userid, sessionid, problem, step, probImg, nextstepanswerkey, newanswer, numAutoResponses, numturns);
                }
                else
                {
                    nextstepanswerkey = answerKey;                                                                                 // current answer key
                    SQLProblemStepTracker.UpdateProbStep(userid, sessionid, problem, step, probImg, nextstepanswerkey, newanswer, numAutoResponses, numturns);
                }

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "DialogueEngine.ashx.cs", 0, userid);
            }

            
            context.Response.ContentType = "text/plain";
            //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //context.Response.AppendHeader("Access-Control-Allow-Headers", "x-requested-with");
            context.Response.Write(transResponse);

        }

        // Write out audio file 
        private string writeFile(HttpFileCollection files, string path, string userid, DateTime timeStart)
        {
            string fullPath = "";
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile file = files[i];
                string formatFileName = string.Format("{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}", file.FileName, timeStart);
                fullPath = path + "data\\userAudio\\" + userid + "_2" + formatFileName + ".wav";
                file.SaveAs(fullPath);
                fullPath = path + "data\\userAudio\\" + file.FileName + ".wav";
                file.SaveAs(fullPath);
            }
            return fullPath;
        }

        // using the transcript, estimate the speakers dialogue act
        private string estimateDialogueAct(string transcript)
        {
            return "statement";
        }

        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
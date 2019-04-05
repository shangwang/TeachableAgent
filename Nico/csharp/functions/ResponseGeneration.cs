using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Nico.csharp.functions;
using System.Threading;
using System.Speech;
using System.Diagnostics;
using System.Data;
using Nico.Hubs;

namespace Nico.csharp.functions
{
    
    static class Globals
    {
        public static string pythonDirectory()
        {
            return "C:\\Python27\\";
        }
    }
    
    
    public class ResponseGeneration
    {
   

        // Text-based function - needs to be modified
        public static Tuple<string, int, string> NicoResponseText(string path, List<int> problemStep, int speakerSpoke, string transcript, DateTime time, string page, string userID, string useraudio)
        {

            Tuple<string, int, string> response = new Tuple<string, int, string>("", 1, "no answer");
            bool checkIfAnswered = false;
            bool agent = SQLAgent.GetAgentSetting(userID);

            try
            {
                // ****** IMPORTANT ******
                // These variables might change based on which NaoNRIProgram calling
                string verbalManagerFile = (agent) ? Globals.pythonDirectory() + "NaoNRIPrograms\\VerbalManager\\DialogueManager_Agent.py " : Globals.pythonDirectory() + "NaoNRIPrograms\\VerbalManager\\DialogueManager.py ";
                string condition = SQLConditionGenderInfo.GetCondition(userID);
                string robotIP = SQLConditionGenderInfo.GetRobotIP(userID);
                string enttype = SQLConditionGenderInfo.GetEntrainment(userID);

                // Get whether the current step has been answered and pass that along 
                int currentstep = problemStep[1];
                int answerKey = problemStep[3];
                int numturns = problemStep[6];
                string answerPattern = SQLAnswerPattern.GetAnswerPattern(answerKey, userID)[1];
                char[] chAnswerPattern = answerPattern.ToCharArray();

                if (page == "ProblemPage")
                {

                    if (transcript == "" || transcript == null)                                                                                                          // Need to handle when there is no transcript - will depend on if this is the first time we've been here or not
                    {
                        //transcript = problemStep[0].ToString() + "_" + problemStep[1].ToString();
                        transcript = "no response";
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);  // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);

                    }
                    else if (transcript == "next step")
                    {
                        if (chAnswerPattern[currentstep] == '1')
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString() + " a";
                        }
                        else
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString();
                        }
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else if (transcript == "prior step")
                    {
                        if (chAnswerPattern[currentstep] == '1')
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString() + " a";
                        }
                        else
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString();
                        }

                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else if (transcript == "problem start")
                    {
                        transcript = transcript + " " + problemStep[0].ToString();

                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                       //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else
                    {
                        // make sure topic set to the correct problem and step
                        // detect problem and step we're on, set to that problem and step
                        // don't play response

                        checkIfAnswered = true;

                        // now generate response
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                      //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);
                    }
                }
                else if (page == "HelloNico")
                {
                    response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                   //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);
                }
                else
                {
                    SQLLog.InsertLog(DateTime.Now, "problem with page detection", "page string not being filled in", "ResponseGeneration.NicoResponse", 1);
                }

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "ResponseGeneration.NicoResponse", 1);
            }

            return response;

        }

        
        /* Voice option
         * Using speaker info, get Nico's response and instigate movement
         * Returns a tuple containing the path to the file of Nico's response, Nico's movement code, and whether Nico provided the answer to this step
        */
        public static Tuple<string, int, string> NicoResponse(string path, List<int> problemStep, int speakerSpoke, string transcript, DateTime time, string page, string userID, string useraudio)
        {

            Tuple<string, int, string> response = new Tuple<string, int, string>("",1,"no answer");
            bool checkIfAnswered = false;
            bool agent = SQLAgent.GetAgentSetting(userID);

            try
            {
                // ****** IMPORTANT ******
                // These variables might change based on which NaoNRIProgram calling
                string verbalManagerFile = (agent) ? Globals.pythonDirectory() + "NaoNRIPrograms\\VerbalManager\\DialogueManager_Agent.py " : Globals.pythonDirectory() + "NaoNRIPrograms\\VerbalManager\\DialogueManager.py ";
                string condition = SQLConditionGenderInfo.GetCondition(userID);
                string robotIP = SQLConditionGenderInfo.GetRobotIP(userID);
                string enttype = SQLConditionGenderInfo.GetEntrainment(userID);

                // Get whether the current step has been answered and pass that along 
                int currentstep = problemStep[1];
                int answerKey = problemStep[3];
                int numturns = problemStep[6];
                string answerPattern = SQLAnswerPattern.GetAnswerPattern(answerKey, userID)[1];
                char[] chAnswerPattern = answerPattern.ToCharArray();

                if (page == "ProblemPage")
                {

                    if (transcript == "" || transcript == null)                                                                                                          // Need to handle when there is no transcript - will depend on if this is the first time we've been here or not
                    {
                        //transcript = problemStep[0].ToString() + "_" + problemStep[1].ToString();
                        transcript = "no response";
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);  // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);
                        
                    }
                    else if (transcript == "next step")
                    {
                        if (chAnswerPattern[currentstep] == '1')
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString() + " a";
                        }
                        else
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString();
                        }
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else if (transcript == "prior step")
                    {
                        if (chAnswerPattern[currentstep] == '1')
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString() + " a";
                        }
                        else
                        {
                            transcript = transcript + " " + problemStep[0].ToString() + " " + problemStep[1].ToString();
                        }

                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else if (transcript == "problem start")
                    {
                        transcript = transcript + " " + problemStep[0].ToString();

                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, 0, robotIP, enttype, agent);
                    }
                    else
                    {
                        // make sure topic set to the correct problem and step
                        // detect problem and step we're on, set to that problem and step
                        // don't play response

                        checkIfAnswered = true;

                        // now generate response
                        response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                        //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);
                    }
               }
               else if (page == "HelloNico")
               {
                    response = dialogueManager(userID, path, problemStep, speakerSpoke, transcript, time, checkIfAnswered, condition);                                                                                      // Generate Nico's response (currently just pandorbots)
                    //moveSpeak(path, response.Item1, response.Item2, verbalManagerFile, useraudio, condition, userID, time, numturns, robotIP, enttype, agent);
                }
               else
                {
                    SQLLog.InsertLog(DateTime.Now, "problem with page detection", "page string not being filled in", "ResponseGeneration.NicoResponse", 1);
                }

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "ResponseGeneration.NicoResponse", 1);
            }
            MyHub.Start(userID, response.Item1);
            return response;

        }

        // This function takes in the speakers transcript and generates Nico's response
        // Output is:
        //              String - Transcript - path of the response file (what Nico said)
        //              Int - Movement - movement code (overwritten later by python movement code)
        //              Int - AnswerStep - did Nico answer this step
        private static Tuple<string, int, string> dialogueManager(string userid, string path, List<int> problemStep, int speakerSpoke, string transcript, DateTime time, bool checkIfAnswered, string condition)
        {
            // Return variables
            string answerStep = "no answer";
            int nicoMovementCode = 1;

            string pathTranscriptFile = "";
            string pathResponseFile = "";
            string BOT = "";           
            string problemset = (SQLConditionGenderInfo.GetProblemSet(userid)).ToLower();
            string responseText = "";

           /*  CREATE BOT NAME
            * 
                 * Pandorabots available:
                 *      asocialemma        Corresponds to Emma Single Session Problem Set and includes social dialogue 
                 *      anonsocialemma     Corresponds to Emma Single Session Problem Set
                 *      emmab          Corresponds to Emma Multi Session 1
                 *      emmac               Corresponds to Emma Multi Session 1
                 *      emmad               Corresponds to Emma Multi Session 1
                 *      emmae               Corresponds to Emma Multi Session 1
                 *      emmaf               Corresponds to Emma Multi Session 1
                 *      gsocialnico        Corresponds to Nico Single Session Problem Set and includes social dialogue
                 *      gnonsocialnico     Corresponds to Nico Single Session Problem Set, no social dialogue
            * */

            if ( problemset == "a")
            {
                if (condition.Contains("nonsocial")) {BOT = problemset + "nonsocialemma";}
                else {BOT = problemset + "socialemma";}
            }
            else if (problemset == "g")
            {
                if (condition.Contains("nonsocial")) { BOT = problemset + "nonsocialnico"; }
                else { BOT = problemset + "socialnico"; }
            }
            else
            {
                BOT = "emma" + problemset;
            }

            try
            {
                // Save transcript to a file so pandora python api program can read it in; save response in a file as well
                pathResponseFile = string.Format("{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}", path + "data\\transcripts\\" + userid + "_nicoresponse", time) + ".txt";                
                pathTranscriptFile = string.Format("{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}", path + "data\\transcripts\\" + userid + "_transcript", time) + ".txt";
                StreamWriter transcriptFile = new StreamWriter(pathTranscriptFile);
                transcriptFile.Write(transcript);
                transcriptFile.Close();

                string pythonexe = Globals.pythonDirectory() + "python.exe";
                string pythonargs = Globals.pythonDirectory() + "NaoNRIPrograms\\VerbalManager\\chatPandoraBot.py " + pathTranscriptFile + " " + pathResponseFile + " " + BOT + " " + userid;

                ExternalMethodsCaller.PythonProcess(pythonexe, pythonargs);

                responseText = readResponse(pathResponseFile);
                string lastAnswer = SQLNicoState.ReadNicoState_Answer(userid);
                if (checkIfAnswered && (responseText.Contains("put the answer") ) )
                {
                    answerStep = "answering";
                }
            }
            catch (Exception error)
            {
                // ** WRITE OUT TO DB
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "ResponseGeneration.generateNicoResponse", 1);
            }

            Tuple<string, int, string> result = new Tuple<string, int, string>(responseText, nicoMovementCode, answerStep);
            return result;
        }

        // Call to Python code for agent/robot to move or speak
        private static void moveSpeak(string path, string pathResponseFile, int movementCode, string verbalManagerFile, string useraudio, string condition, string userid, DateTime time, int numturns, string robotIP, string enttype, bool agent)
        {
            try
            {
                if (agent)
                {
                    string agentAudioPath = "../data/agentAudio/" + generateSpeech(userid, condition, pathResponseFile, path);
                    if (enttype == "None")
                    {

                        ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
                        Thread.Sleep(2000);                    
                    }
                    else
                    {
                        string pythonexe = Globals.pythonDirectory() + "python.exe";
                        string formatdate = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", time);
                        if (useraudio == "")
                        {
                            useraudio = Globals.pythonDirectory() + "NaoNRIPrograms\\NicoAudio\\response.wav";
                        }
                        string agentAudio = agentAudioPath;
                        string gender = SQLConditionGenderInfo.GetGender(userid);
                        string pythonargs = verbalManagerFile + userid + " " + formatdate + " " + useraudio + " " + agentAudio + " " + condition + " " + numturns.ToString() + " " + enttype + " " + gender;
                        ExternalMethodsCaller.PythonProcess(pythonexe, pythonargs);

                        Thread.Sleep(1000);

                    }

                   
                    
                }
                else
                {
                    string pythonexe = Globals.pythonDirectory() + "python.exe";
                    string formatdate = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", time);
                    if (useraudio == "")
                    {
                        useraudio = Globals.pythonDirectory() + "NaoNRIPrograms\\NicoAudio\\response.wav";
                    }
                    string pythonargs = verbalManagerFile + userid + " " + formatdate + " " + useraudio + " " + pathResponseFile + " " + condition + " " + numturns.ToString() + " " + robotIP + " " + enttype;
                    ExternalMethodsCaller.PythonProcess(pythonexe, pythonargs);
                }
            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "ResponseGeneration.moveSpeak", 1);
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

        private static string generateSpeech(string userid, string condition, string pathResponseFile, string path)
        {
            // *********  generate wav file voicing the response *****************
            // Using Microsoft voices

            // initiate new instance of speech synthesizer
            // needs own separate dedicated thread...

            string response = readResponse(pathResponseFile);
            string audioFilePath = "";

            SQLLog.InsertLog(DateTime.Now, "Beginning Thread", "Beginning Thread", "ResponseGeneration.moveSpeak", 1);
            Thread t = new Thread(() =>
            {

                try
                {

                    System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer();
                    string gender = SQLConditionGenderInfo.GetGender(userid);
                    bool makecopy = false;

                    if (synth != null)
                    {

                        if (gender.Contains("female"))
                        {
                            synth.SelectVoice("Microsoft Zira Desktop");

                        }
                        else
                        {
                            synth.SelectVoice("Microsoft David Desktop");
                        }

                        if (condition == "nonsocial" || condition == "social")
                        {
                            synth.SetOutputToWaveFile(path + "data\\agentAudio\\transformed.wav");
                            audioFilePath = "transformed.wav";
                            makecopy = true;
                        }
                        else
                        {
                            synth.SetOutputToWaveFile(path + "data\\agentAudio\\response.wav");
                            audioFilePath = "response.wav";
                        }

                        synth.Speak(response);

                        if (makecopy)
                        {
                            string formatFileName = string.Format("data\\agentAudio\\transformed_{0:yyyy-MM-dd_hh-mm-ss-tt}.wav", DateTime.Now);
                            audioFilePath = string.Format("transformed_{0:yyyy-MM-dd_hh-mm-ss-tt}.wav", DateTime.Now);                            synth.SetOutputToWaveFile(path + formatFileName);
                            synth.Speak(response);
                        }

                        synth.SetOutputToNull();

                    }
                }
                catch (Exception e)
                {
                    SQLLog.InsertLog(DateTime.Now, "Something went wrong in generating speech", "", "ResponseGeneration.generateSpeech", 1);
                }


            });
            t.Start();
            t.Join();

            Thread.Sleep(1000);

            return audioFilePath;

        }


    }
}
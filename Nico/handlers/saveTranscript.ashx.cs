using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Configuration;

/*
 * Need to move everything into functions
 * Check for if context is empty
 * set final_transcript to something that indicates when it was on a page load
 * Figure out how to rename to 'step analyzer'
 * determine what the slowest part of this is...
*/



namespace Nico.handlers
{
    /// <summary>
    /// Summary description for saveTranscript
    /// </summary>
    public class saveTranscript : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string path1 = context.Request.PhysicalApplicationPath;
            string userid = HttpContext.Current.User.Identity.Name;
            string line = "";
            string image = "";
            string imagePath = "";
            using (StreamWriter ws = new StreamWriter(path1 + "data\\logs\\log_StepAnalyzer.txt"))
            {

                try
                {
                    if (context.Request.ContentLength > 0)
                    {
                        line = new StreamReader(context.Request.InputStream).ReadToEnd();
                    }

                    string formatFileName = string.Format("{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}", "transcript", DateTime.Now);
                    string fullPath = path1 + "data\\transcripts\\" + userid + "_" + formatFileName + ".txt";
                    StreamWriter transcriptFile = new StreamWriter(fullPath);
                    transcriptFile.Write(line);
                    transcriptFile.Close();

                    string pythonexe = "C:\\Python27\\python.exe";
                    string pythonargs = "C:\\Python27\\talk_with_nico.py " + fullPath;

                    //string pythonexe = "E:\\Python27\\python.exe";
                    //string pythonargs = "E:\\Python27\\talk_with_nico.py " + fullPath;
                    

                    var pythonProcessInfo = new ProcessStartInfo(pythonexe, pythonargs)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };

                    Process pythonProc;

                    if ((pythonProc = Process.Start(pythonProcessInfo)) == null)
                    {
                        ws.WriteLine("Issue with python call.");
                        throw new InvalidOperationException("??");
                    }
                    
                    StreamReader myStreamReader = pythonProc.StandardError;
                    StreamReader myStreamReader2 = pythonProc.StandardOutput;


                    ws.WriteLine("Initiated praat process call; waiting for it to exit \n");

                    pythonProc.WaitForExit();
                    pythonProc.Close();

                    string test1 = myStreamReader.ReadToEnd();
                    string test2 = myStreamReader2.ReadToEnd();

                    ws.WriteLine(test1);
                    ws.WriteLine(test2);

                }
                catch (Exception error)
                {
                    ws.Write(error.Message);
                    ws.Write(error.StackTrace);

                }

                int count = 1;
                try
                {
                    using (StreamReader readNumTimes = new StreamReader(path1 + @"data\logs\numberOfTimesSpaceBar.txt"))
                    {
                        string s = readNumTimes.ReadLine();
                        count += Int32.Parse(s);
                    }
                }
                catch (Exception ex1)
                {
                    ws.WriteLine(ex1.Message);
                }


                try
                {
                    using (StreamWriter numTimes = new StreamWriter(path1 + @"data\logs\numberOfTimesSpaceBar.txt"))
                    {
                        numTimes.WriteLine(count);
                    }
                }
                catch (Exception ex1)
                {
                    ws.WriteLine(ex1.Message);
                }



                try
                {

                    using (StreamReader images = new StreamReader(path1 + @"content\img\imagedirectory\problem_one_images.txt"))
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            if (i < 9)
                            {
                                image = images.ReadLine();
                            }
                        }
                        imagePath = "../content/img/problem_one/" + image;
                    }
                }
                catch (Exception ex1)
                {
                    ws.WriteLine(ex1.Message);
                }

            }

            


            context.Response.ContentType = "text/HTML";
            context.Response.Write(imagePath);
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
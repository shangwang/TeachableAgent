using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Security;
using System.Diagnostics;
using Nico.csharp.functions;

namespace Nico.csharp.functions
{
    public class ExternalMethodsCaller
    {

        // Generic function, calls any external program process, labeled as Python because only being used to call Python code currently
        public static void PythonProcess(string pythonexe, string pythonargs)
        {
            try
            {
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
                    throw new InvalidOperationException("??");
                }

                StreamReader myStreamReader = pythonProc.StandardError;
                StreamReader myStreamReader2 = pythonProc.StandardOutput;

                pythonProc.WaitForExit();
                pythonProc.Close();

                string test1 = myStreamReader.ReadToEnd();
                string test2 = myStreamReader2.ReadToEnd();

            }
            catch (Exception error)
            {
                SQLLog.InsertLog(DateTime.Now, error.Message, error.StackTrace, "ExternalMethodCaller", 1);
            }
        }

    }
}
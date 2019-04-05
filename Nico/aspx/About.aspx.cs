using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Security;
using Nico.csharp.functions;


namespace Nico
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 
        }

        protected void Submit(object sender, EventArgs e)
        {
            string gender = Request.Form["selectGender"];
            string agent = Request.Form["selectAgent"];
            string condition = Request.Form["selectCondition"];
            string enttype = Request.Form["selectEntrainment"];
            string problemSet = Request.Form["selectProblemSet"];
            string voiceText = Request.Form["selectVoiceText"];
            string path = Request.PhysicalApplicationPath;
            string userid = HttpContext.Current.User.Identity.Name;
            string robotAddress = robotIP.Value;
            
            if (voiceText == "text" && condition.Contains("entrain") )
            {
                string script = "alert('You cannot select both text output and entrainment. Please select social or non-social if you want to use a text-based agent.');";
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(submitbutton, this.GetType(), "Test", script, true);
            }
            else if (agent == "false" && voiceText == "text")
            {
                string script = "alert('You cannot select both text output and a robot version. Please select agent if you want to use a text.');";
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(submitbutton, this.GetType(), "Test", script, true);
            }
            else
            {
                try
                {
                    SQLConditionGenderInfo.UpdateConditionGender(userid, condition, gender, robotAddress, enttype, problemSet, voiceText);
                    SQLAgent.SetAgent(agent, userid);
                    Response.Redirect("default.aspx", false);
                }
                catch (Exception error)
                {
                    SQLLog.InsertLog(DateTime.Now, error.Message, error.ToString(), "UpdateStep.ashx.cs", 0, userid);
                }
            }
        }


    }
}
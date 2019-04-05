using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nico.csharp.functions;

namespace Nico.aspx
{
    public partial class RedirectPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userid = HttpContext.Current.User.Identity.Name;
            string voiceText = SQLConditionGenderInfo.GetVoiceText(userid);
            if (SQLAgent.GetAgentSetting(userid) && voiceText == "text")
            {
                Response.Redirect("ProblemPageAgentText.aspx", false);
            }
            else if (SQLAgent.GetAgentSetting(userid))
            {
                Response.Redirect("ProblemPageAgent.aspx", false);
            }
            else
            {
                Response.Redirect("Intro.aspx", false);
            }
        }
    }
}
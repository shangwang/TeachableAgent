using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nico.aspx
{
    public partial class Intro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Hello_Nico(object sender, EventArgs e)
        {
            //int eventid = 21;
            //string eventtype = "problemone_to_nicoone";
            //string result = logged(eventtype, eventid);

            Response.Redirect("HelloNico.aspx");
        }
    }
}
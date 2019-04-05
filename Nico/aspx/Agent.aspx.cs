using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nico.Hubs;

namespace Nico.aspx
{
    public partial class Agent_Billy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userid = HttpContext.Current.User.Identity.Name;
           // MyHub.AddUser(userid);
        }
    }
}
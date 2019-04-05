using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Routing;

namespace Nico
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            
            try
            {
                SqlDependency.Start(ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString);
            }
            catch (Exception error)
            {
                // trouble!
            }

        }
        protected void Application_End()
        {
            try
            {
                SqlDependency.Stop(ConfigurationManager.ConnectionStrings["NicoDB"].ConnectionString);
            }
            catch(Exception error)
            {
                // more trouble!
            }
             
        }
    }
}
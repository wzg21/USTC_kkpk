using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;

namespace TimelyEvaluation.aspx
{
    public partial class Activate : System.Web.UI.Page
    {
        public string type = "error";

        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string activecode = Request.QueryString["activecode"];
            if (id == null || activecode == null) 
            {
                type = "error";
                Response.Redirect("/html/failed.html");
                return;
            }
            UserInfo user = new UserInfo();
            user.SearchUserID = Convert.ToInt32(id);
            if (1 == user.Activate(activecode)) 
            {
                type = "succeed";
                Response.Redirect("/html/success.html");
            }
            else
            {
                type = "invalid";
                Response.Redirect("/html/failed.html");
            }
        }
    }
}
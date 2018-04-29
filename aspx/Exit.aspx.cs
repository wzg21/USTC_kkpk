using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TimelyEvaluation.aspx
{
    public partial class Exit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["UserInfo"] = null; //清除用户会话 退出系统
                Response.Redirect("/aspx/Login.aspx");
            }
        }
    }
}
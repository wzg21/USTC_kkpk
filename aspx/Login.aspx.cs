using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TimelyEvaluation.DataBase;

namespace TimelyEvaluation.aspx
{
    public partial class Login : System.Web.UI.Page
    {
        string url = HttpContext.Current.Request.RawUrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
                Session["UserInfo"] = null;
        }

        protected void Login_Button_Click(object sender, EventArgs e)
        {
            string username = UserName_TextBox.Text.Trim(); //获取用户名
            string Email = Email_select.Value.ToString();   //获取邮箱后缀
            string pwd = Password_TextBox.Text.Trim(); //获取用户输入的密码
            //判断用户名和密码是否为空可以尝试写到javascript中，避免访问服务器
            if (username == "")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "nouser", "alert('请输入邮箱');", true);
                //HttpContext.Current.Response.Write("<script>alert('请输入邮箱');location.href='" + url + "'</script>");
            }
            else if (pwd == "")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "nopassword", "alert('请输入密码');", true);
                //HttpContext.Current.Response.Write("<script>alert('请输入密码');location.href='" + url + "'</script>");
            }
            else
            {
                UserInfo user = new UserInfo();
                user.Password = pwd;
                user.Email = username + Email;

                switch (user.Login_Email())
                {
                    case 0:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "loginfailed", "alert('登录失败');", true);
                        //HttpContext.Current.Response.Write("<script>alert('登录失败');location.href='" + url + "'</script>");
                        break;
                    case 1:
                        Session["UserInfo"] = user; //将用户详细信息放入会话
                        Response.Redirect("/aspx/CourseList.aspx"); //跳转页面至首页
                        break;
                    default:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "notactivate", "alert('账号未激活');", true);
                        //HttpContext.Current.Response.Write("<script>alert('账号未激活');location.href='" + url + "'</script>");
                        break;
                }
            }
            
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpRequestValidationException)
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('输入的内容包含非法字符');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                Server.ClearError();
            }
        }

    }
}
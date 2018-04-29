using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TimelyEvaluation.DataBase;

namespace TimelyEvaluation.aspx
{
    public partial class RetrievePassword : System.Web.UI.Page
    {
        string url = HttpContext.Current.Request.RawUrl;
        UserInfo newuser = new UserInfo(); //创建用户对象

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected static Boolean IsChineseLetter(int code)
        {
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);

            if (code >= chfrom && code <= chend)
            {
                return true;     //当code在中文范围内返回true
            }
            else
            {
                return false;    //当code不在中文范围内返回false
            }
        }

        public static Boolean isLegalNumber_MailName(string str)
        {
            //(Beta)本函数用于判断string（邮箱名）的非法字符，若string由大小写、字母组成，则返回true，否则返回false
            char[] charStr = str.ToLower().ToCharArray();
            for (int i = 0; i < charStr.Length; i++)
            {
                int num = Convert.ToInt32(charStr[i]);
                if (!((num >= 48 && num <= 57) || (num >= 97 && num <= 123) || (num >= 65 && num <= 90)))
                {
                    return false;
                }
            }
            return true;
        }

        protected void GetVerificationCode_Button_Click(object sender, EventArgs e)
        {
            string Mail_name = Email_TextBox.Text.Trim(); //获取邮箱
            string Mail_suffix = Email_select.Value.ToString();   //获取邮箱后缀
            string email = Mail_name + Mail_suffix;
            if (Mail_name == "" )
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "notcomplete", "alert('有缺失信息');", true);
                //HttpContext.Current.Response.Write("<script>alert('有缺失信息');location.href='" + url + "'</script>");
            }
            else if (Mail_suffix != "@mail.ustc.edu.cn" && Mail_suffix != "@ustc.edu.cn")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "invalidemailex", "alert('非法的邮箱后缀');", true);
                //HttpContext.Current.Response.Write("<script>alert('非法的邮箱后缀');location.href='" + url + "'</script>");
            }
            else if (Mail_name.Length > 20 || Mail_name.Length < 2 || !isLegalNumber_MailName(Mail_name)) //(Beta)将邮箱用户名长度限制在2-20位，邮箱中不能含有非法字符
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "invalidemail", "alert('非法的邮箱（邮箱请限制在2-20位）');", true);
                //HttpContext.Current.Response.Write("<script>alert('非法的邮箱用户名（邮箱名请限制在2-20位）');location.href='" + url + "'</script>");
            }
            else
            {
                newuser.Email = email;
       
                switch(newuser.SendVerificationCode())
                {
                    case -1:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "emailnotfound", "alert('邮箱不存在');", true);
                        //HttpContext.Current.Response.Write("<script>alert('邮箱不存在');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                        break;
                    case 1:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "successsend", "alert('发送成功，请前去邮箱查看');", true);
                        //HttpContext.Current.Response.Write("<script>alert('发送成功，请前去邮箱查看');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                        break;
                    case 2:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "gotoactivate", "alert('账号未激活，已发送激活链接');", true);
                        //HttpContext.Current.Response.Write("<script>alert('账号未激活，已发送激活链接');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                        break;
                    default:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "sendfailed", "alert('发送失败');", true);
                        //HttpContext.Current.Response.Write("<script>alert('发送失败');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                        break;
                }
            }
            
        }

        protected void RetrievePassword_Button_Click(object sender, EventArgs e)
        {
            string Mail_name = Email_TextBox.Text.Trim(); //获取邮箱
            string Mail_suffix = Email_select.Value.ToString();   //获取邮箱后缀
            string pwd = Password_TextBox.Text.Trim(); //获取密码
            string pwd2 = PasswordConfirm_TextBox.Text.Trim(); //获取重复密码
            string email = Mail_name + Mail_suffix;
            string verification_code = VerificationCode_TextBox.Text.Trim();//获取验证码
            //判断是否为空可以尝试写到javascript中，避免访问服务器
            if (Mail_name == "" || pwd == "" || pwd2 == "")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "notcomplete", "alert('有缺失信息');", true);
                //HttpContext.Current.Response.Write("<script>alert('有缺失信息');location.href='" + url + "'</script>");
            }
            else if (pwd != pwd2)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "notsame", "alert('两次输入密码不一致');", true);
                //HttpContext.Current.Response.Write("<script>alert('两次输入密码不一致');location.href='" + url + "'</script>");
            }
            else if (Mail_suffix != "@mail.ustc.edu.cn" && Mail_suffix != "@ustc.edu.cn") 
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "invalidemailex", "alert('非法的邮箱后缀');", true);
                //HttpContext.Current.Response.Write("<script>alert('非法的邮箱后缀');location.href='" + url + "'</script>");
            }
            else if (Mail_name.Length > 20 || Mail_name.Length < 2 || !isLegalNumber_MailName(Mail_name)) //(Beta)将邮箱用户名长度限制在2-20位，邮箱中不能含有非法字符
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "invalidemail", "alert('非法的邮箱（邮箱请限制在2-20位）');", true);
                //HttpContext.Current.Response.Write("<script>alert('非法的邮箱用户名（邮箱名请限制在2-20位）');location.href='" + url + "'</script>");
            }
            else if (pwd.Length > 18 || pwd.Length < 6)//(Beta)密码限制在6-18位字符
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "invalidpassword", "alert('非法的密码（密码请限制在6-18位）');", true);
                //HttpContext.Current.Response.Write("<script>alert('非法的密码（密码请限制在6-18位）');location.href='" + url + "'</script>");
            }
            else
            {
                
                newuser.Email = email; //设置邮箱
                newuser.Password = pwd; //设置密码

                switch (newuser.ChangePasswordByEmail(verification_code))
                {
                    case -1:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "emailconnotfound", "alert('邮箱不存在或未激活');", true);
                        //HttpContext.Current.Response.Write("<script>alert('邮箱不存在或未激活');location.href='" + url + "'</script>");
                        break;
                    case 1:
                        Response.Redirect("/aspx/Wait.aspx");
                        break;
                    case -2:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "errorcode", "alert('验证码错误');", true);
                        //HttpContext.Current.Response.Write("<script>alert('验证码错误');location.href='" + url + "'</script>"); 
                        break;
                    default:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "changefailed", "alert('修改失败');", true);
                        //HttpContext.Current.Response.Write("<script>alert('修改失败');location.href='" + url + "'</script>");
                        break;
                }
            }
            return;
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
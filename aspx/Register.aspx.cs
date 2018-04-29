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
    public partial class Register : System.Web.UI.Page
    {
        string url = HttpContext.Current.Request.RawUrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "tsdiv", "Notice();", true);
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
        public static Boolean isLegalNumber_UserName(string str)
        {
            //(Beta)本函数用于判断string(昵称)的非法字符，若string由大小写、字母、下划线、汉字组成，则返回true，否则返回false
            char[] charStr = str.ToLower().ToCharArray();
            for (int i = 0; i < charStr.Length; i++)
            {
                int num = Convert.ToInt32(charStr[i]);
                if (!( (num >= 48 && num <= 57) || (num >= 97 && num <= 123) || (num >= 65 && num <= 90) || num == 95||(IsChineseLetter(num))))
                {
                    return false;
                }
            }
            return true;
        }
        protected void Register_Button_Click(object sender, EventArgs e)
        {
            string Mail_name = Email_TextBox.Text.Trim(); //获取邮箱
            string Mail_suffix = Email_select.Value.ToString();   //获取邮箱后缀
            string pwd = Password_TextBox.Text.Trim(); //获取密码
            string pwd2 = PasswordConfirm_TextBox.Text.Trim(); //获取重复密码
            string email = Mail_name + Mail_suffix;
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
                UserInfo newuser = new UserInfo(); //创建用户对象
                newuser.Email = email; //设置邮箱
                newuser.Password = pwd; //设置密码
                
                if (Mail_suffix == "@ustc.edu.cn")  //老师注册
                {
                    newuser.isStudent = 0;
                    newuser.UserName = Teacher_UserName_textbox.Text.Trim();
                    if(newuser.UserName == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "noteachername", "alert('老师姓名不能为空');", true);
                        //HttpContext.Current.Response.Write("<script>alert('老师姓名不能为空');location.href='" + url + "'</script>");
                        return;
                    }
                }
                else if(Mail_suffix == "@mail.ustc.edu.cn") //学生注册
                {
                    newuser.isStudent = 1;
                    newuser.UserName = Student_UserName_textbox.Text.Trim();
                    if (!isLegalNumber_UserName(newuser.UserName))//(Beta)学生昵称由字母、数字、汉字、下划线组成
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "invalidstudentname", "alert('学生昵称中含有非法字符');", true);
                        //HttpContext.Current.Response.Write("<script>alert('学生昵称中含有非法字符');location.href='" + url + "'</script>");
                        return;
                    }
                    if(newuser.UserName.Length >12|| newuser.UserName.Length<2)//(Beta)学生昵称限制在2-12位字符
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "invalidnamelength", "alert('学生昵称长度非法（请限制在2-12位）');", true);
                        //HttpContext.Current.Response.Write("<script>alert('学生昵称长度非法（请限制在2-12位）');location.href='" + url + "'</script>");
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "invalidemailex", "alert('非法的邮箱后缀');", true);
                    //HttpContext.Current.Response.Write("<script>alert('邮箱后缀非法');location.href='" + url + "'</script>");
                    return;
                }
                switch (newuser.Register())
                {
                    case -1:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "registeremail", "alert('该邮箱已被注册');", true);
                        //HttpContext.Current.Response.Write("<script>alert('该邮箱已被注册');location.href='" + url + "'</script>");
                        break;
                    case 1:
                        Response.Redirect("/aspx/Wait.aspx");
                        //Response.Redirect("/aspx/Login.aspx");//注册成功 跳转至登录页面
                        break;
                    case -2:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "invalidteachername", "alert('教师姓名不存在，请重新输入！');", true);
                        //HttpContext.Current.Response.Write("<script>alert('教师姓名不存在，请重新输入！');location.href='" + url + "'</script>"); 
                        break;
                    default:
                        ScriptManager.RegisterStartupScript(Page, GetType(), "registererror", "alert('注册异常');", true);
                        //HttpContext.Current.Response.Write("<script>alert('注册异常');location.href='" + url + "'</script>");
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
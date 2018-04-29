using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Data;

namespace TimelyEvaluation.aspx
{
    public partial class Information : System.Web.UI.Page
    {
        string url = HttpContext.Current.Request.RawUrl;
        public UserInfo user = new UserInfo();
        public UserInfo owner = new UserInfo();
        public DataSet MyInfo = new DataSet();
        public DataSet OwnerInfo = new DataSet();
        public int uid;
        public string default_MyUsername = "待设置昵称";
        public string default_MyPersonalProfile = "待设置个人签名";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserInfo"] == null)
            {
                Response.Redirect("/aspx/Login.aspx");
            }
            else
            {
                user = Session["UserInfo"] as TimelyEvaluation.DataBase.UserInfo;
            }

            string uid_string = Request.QueryString["UID"];
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (uid_string == null || !rex.IsMatch(uid_string))
            {
                Response.Redirect("/aspx/Information.aspx?UID=0");
            }

            uid = int.Parse(uid_string);    //使用UID传参

            if (user.UserID == uid)//user查看自己信息
            {         
                if (!Page.IsPostBack)
                {
                    //从数据库通过ID获得本人信息，赋给MyInfo
                    user.SearchUserID = uid;
                    MyInfo = user.SearchUserInfo();

                    user.UserName = MyInfo.Tables[0].Rows[0]["UserName"].ToString();
                    user.Email = MyInfo.Tables[0].Rows[0]["Email"].ToString();
                    user.PersonalProfile = MyInfo.Tables[0].Rows[0]["PersonalProfile"].ToString();

                    user.PhotoUrl = MyInfo.Tables[0].Rows[0]["PhotoUrl"].ToString();
                    
                    user.isStudent = int.Parse(MyInfo.Tables[0].Rows[0]["isStudent"].ToString());

                    if (user.UserName == default_MyUsername)
                    {

                    }
                    else
                    {
                        MyUsername_TextBox.Text = user.UserName;
                    }

                    if (user.PersonalProfile == default_MyPersonalProfile)
                    {

                    }
                    else
                    {
                        MyPersonalProfile_TextBox.Text = user.PersonalProfile;
                    }
                }

               
            }//end if
            else               //user查看他人(owner)信息
            {
               //从数据库通过ID获得他人信息，赋给OwnerInfo
               owner.SearchUserID = uid;
               OwnerInfo = owner.SearchUserInfo();
               if (0 == OwnerInfo.Tables[0].Rows.Count)
               {
                   owner.UserName = "未知用户";
                   owner.Email = "未知";
                   owner.PersonalProfile = "未知";
                   owner.isStudent = 1;
               }
               else
               {
                   owner.UserName = OwnerInfo.Tables[0].Rows[0]["UserName"].ToString();
                   owner.Email = OwnerInfo.Tables[0].Rows[0]["Email"].ToString();
                   owner.PersonalProfile = OwnerInfo.Tables[0].Rows[0]["PersonalProfile"].ToString();
                   owner.isStudent = int.Parse(OwnerInfo.Tables[0].Rows[0]["isStudent"].ToString());
               }               
               
               //owner.PhotoUrl = OwnerInfo.Tables[0].Rows[0]["PhotoUrl"].ToString();
            }
        }//end Page_Load
           

        protected void ModifyMyInfomation_Button_Click(object sender, EventArgs e)
        {
            string NewUsername;
            string NewPersonProfile;
            if(user.isStudent == 1)
            {
                if (user.UserName == default_MyUsername)
                {
                    NewUsername = MyUsername0_TextBox.Text.Trim();   //获取新的用户昵称                    
                }
                else
                {
                    NewUsername = MyUsername_TextBox.Text.Trim();   //获取新的用户昵称
                }

                if (NewUsername == "点击编辑自己的昵称")
                {
                    NewUsername = default_MyUsername;
                }
                user.UserName = NewUsername;
            }
            

            if (user.PersonalProfile == default_MyPersonalProfile)
            {
                NewPersonProfile = MyPersonalProfile0_TextBox.Text.Trim(); //获取新的用户个人签名               
            }
            else
            {
                NewPersonProfile = MyPersonalProfile_TextBox.Text.Trim(); //获取新的用户个人签名
            }
            if (NewPersonProfile == "点击编辑自己的个人签名")
            {
                NewPersonProfile = default_MyPersonalProfile;
            }

            user.PersonalProfile = NewPersonProfile;
            /*对用户头像的修改*/
            //if (PhotoFileUpload.HasFile)
            //{
            //    string fullFileName = PhotoFileUpload.PostedFile.FileName; //文件路径名
            //    string fileName = fullFileName.Substring(fullFileName.LastIndexOf("\\") + 1); //文件名称
            //    string type = fullFileName.Substring(fullFileName.LastIndexOf(".") + 1); //文件格式
            //    //type = PhotoFileUpload.PostedFile.ContentType;

            //    if (type == "jpg" || type == "jpeg" || type == "bmp" || type == "gif" || type == "png") //判断文件是否为指定类型
            //    {                       //上传文件格式正确(未对文件大小做出限定)(*未对文件名冲突做出解决)
            //        string upPath = "/userPhoto/";  //上传文件路径
            //        // string upFilename = DateTime.Now.ToString("yyyyMMddhhmmssfff") + user.UserID + type;
            //        string webfileName = user.UserID + "." + type;
            //        string webFilePath = Server.MapPath(upPath) + webfileName;        // 服务器端文件路径
            //        PhotoFileUpload.SaveAs(webFilePath);
                    
            //        user.PhotoUrl = webfileName;   //##数据库接口
                
            //    }
            //    else                    //上传文件格式不正确
            //    {
            //        Response.Write("<script>alert('头像上传文件格式不正确(限于.jpg .jpeg .bmp .gif .png)')</script>");
            //        return;
            //    }
            //}

            /*昵称、个人签名的修改*/
            
            

            if(0 == user.ChangeUserInfo())    //修改失败
            {
                HttpContext.Current.Response.Write("<script>alert('修改失败');location.href='" + url + "'</script>");
            }
            else                //修改成功
            {
                HttpContext.Current.Response.Write("<script>alert('修改成功');location.href='" + url + "'</script>");                              
            }
            
        }
        protected void ModifyMyPassword_Button_Click(object sender, EventArgs e)
        {

            //获取用户输入
            string OldPassword = ModifyMyPassword_OldPassword_Textbox.Text.Trim();
            string NewPassword = ModifyMyPassword_NewPassword_Textbox.Text.Trim();
            string ConfirmPassword = ModifyMyPassword_ComfirmPassword_Textbox.Text.Trim();
            
            if(NewPassword != ConfirmPassword)
            {
                //确认密码与新密码不一致
                HttpContext.Current.Response.Write("<script>alert('两次输入密码不一致');location.href='" + url + "'</script>");
            }
            else 
            {
                //对原密码进行验证
                UserInfo temp = new UserInfo();
                temp.Email = user.Email;
                temp.Password = OldPassword;

                if (1 == temp.Login_Email())
                {
                    //原密码正确，进行新密码修改
                    user.Password = NewPassword;
                    if(1==user.ChangeUserInfo())
                    {
                        HttpContext.Current.Response.Write("<script>alert('密码修改成功');location.href='" + url + "'</script>");
                    }
                    else 
                    {
                        HttpContext.Current.Response.Write("<script>alert('密码修改失败，请重试');location.href='" + url + "'</script>");
                    }
                }
                else 
                {
                    //原密码错误
                    HttpContext.Current.Response.Write("<script>alert('原密码错误');location.href='" + url + "'</script>");
                }
            }

        }//end ModifyMyPassword_Button_Click

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
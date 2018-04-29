using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Web.UI.HtmlControls;

namespace TimelyEvaluation.aspx
{
    public partial class Course : System.Web.UI.Page
    {
        public string courseid;
        public UserInfo user = new UserInfo();
        public CourseInfo course = new CourseInfo();
        public DataSet Teacher_TA;
        public DataSet mycomment;
        public DataSet allcomment;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                user = Session["UserInfo"] as UserInfo;
            }

            courseid = Request.QueryString["CourseID"];
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (courseid == null || !rex.IsMatch(courseid))
            {
                Response.Redirect("/aspx/CourseList.aspx");
            }

            course.CourseID = int.Parse(courseid);
            course.UserID = user.UserID;
            course.GetCourseInfo();
            Teacher_TA = course.GetCourseTeacher();
            mycomment = course.GetMyCourseComment();

            if (!Page.IsPostBack)
            {
                AllDataBind();
            }
        }

        private void AllDataBind()
        {
            course.CourseID = int.Parse(courseid);
            course.UserID = user.UserID;
            course.GetCourseInfo();
            Teacher_TA = course.GetCourseTeacher();
            mycomment = course.GetMyCourseComment();

            Modify_Intro.Text = course.Introduction;
            T_DataList.DataSource = course.GetCourseTeacher().Tables["TeacherName"];
            T_DataList.DataBind();
            A_DataList.DataSource = course.GetCourseTeacher().Tables["AssistantName"];
            A_DataList.DataBind();
            if (user.isStudent == 1 && course.IsCourseTeacher() == 0)
            {
                if (mycomment.Tables["CourseComment"].Rows.Count != 0)
                {
                    ModifyMyComment_TextBox.Text = mycomment.Tables["CourseComment"].Rows[0]["Content"].ToString();

                    Byte[] tags = new Byte[20];
                    Byte a = 1, b = 0;
                    for (int i = 1; i <= 6; i++)
                    {
                        if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[i - 1] == a)
                        {
                            ((CheckBox)FindControl("MyCommentTagModify" + i.ToString() + "_CheckBox")).Checked = true;
                        }
                        else
                        {
                            ((CheckBox)FindControl("MyCommentTagModify" + i.ToString() + "_CheckBox")).Checked = false;
                        }
                    }
                }
            }

            MessageInfo mc = new MessageInfo();
            if (mycomment.Tables["CourseComment"].Rows.Count != 0)
            {
                mc.MessageID = int.Parse(mycomment.Tables["CourseComment"].Rows[0]["CommentID"].ToString());
                DataSet mycommentreply = mc.GetCommentReply();
                MyCommentReplyComment_DataList.DataSource = mycommentreply;
                MyCommentReplyComment_DataList.DataBind();
                MyReplyNum_div.InnerHtml = mycommentreply.Tables["Reply"].Rows.Count.ToString();
            }

            allcomment = course.GetCourseComment();
            AspNetPager.RecordCount = allcomment.Tables["CourseComment"].Rows.Count;
            AllComment_DataList.DataSource = FilterTable(allcomment.Tables["CourseComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllComment_DataList.DataBind();
        }

        protected void AspNetPager_PageChanged(object src, EventArgs e)
        {
            int index;// = AspNetPager.CurrentPageIndex;
            if (Request.QueryString["Page"] == null)
            {
                index = 1;
            }
            else
            {
                index = Convert.ToInt32(Request.QueryString["Page"]);
                AspNetPager.CurrentPageIndex = index;//一直都是1，所以用这种法子给它改
            }
            AllComment_DataList.DataSource = FilterTable(allcomment.Tables["CourseComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllComment_DataList.DataBind();
        }

        public DataTable FilterTable(DataTable dt, int startIndex, int endIndex)
        {
            DataTable filterTable = dt.Clone();
            if (endIndex <= dt.Rows.Count)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    filterTable.Rows.Add(dt.Rows[i].ItemArray);
                }
            }
            return filterTable;
        }

        protected void AllComment_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                MessageInfo comment = new MessageInfo();
                comment.MessageID = int.Parse(DataBinder.Eval(e.Item.DataItem, "CommentID").ToString());

                DataList dataList = e.Item.FindControl("ReplyComment_DataList") as DataList;
                DataSet commentreply = comment.GetCommentReply();
                dataList.DataSource = commentreply;
                dataList.DataBind();

                ((HtmlGenericControl)e.Item.FindControl("ReplyNum_div")).InnerHtml = commentreply.Tables["Reply"].Rows.Count.ToString();
                ((HtmlGenericControl)e.Item.FindControl("ReplyNum_div")).TagName = "ReplyNum_div_Name" + DataBinder.Eval(e.Item.DataItem, "CommentID").ToString();
            }
        }

        protected void MyComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyComment_TextBox.Text.Trim() == "" || MyComment_TextBox.Text == "你还没有对此课程发表任何评论哦")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='/aspx/Course.aspx?CourseID=" + courseid + "'</script>");
                }
                else
                {
                    Byte[] tags = new Byte[20];
                    Byte a = 1, b = 0; ;
                    for (int i = 1; i <= 6; i++)
                    {
                        tags[i - 1] = ((CheckBox)FindControl("MyCommentTag" + i.ToString() + "_CheckBox")).Checked == true ? a : b;
                    }

                    MessageInfo mycomment = new MessageInfo();
                    mycomment.Anonymous = 0;
                    mycomment.UserID = user.UserID;
                    mycomment.CourseID = course.CourseID;
                    mycomment.Content = MyComment_TextBox.Text;
                    mycomment.Tags = tags;
                    switch(mycomment.AddComment())
                    {
                        case 1:
                            AllDataBind();
                            break;
                        case -1:
                            AllDataBind();
                            ScriptManager.RegisterStartupScript(Page, GetType(), "havecomment", "alert('评论已存在');", true);
                            break;
                        default:
                            ScriptManager.RegisterStartupScript(Page, GetType(), "commentfailed", "alert('评论失败');", true);
                            break;
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void MyAnonymousComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyComment_TextBox.Text.Trim() == "" || MyComment_TextBox.Text == "你还没有对此课程发表任何评论哦")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='/aspx/Course.aspx?CourseID=" + courseid + "'</script>");
                }
                else
                {
                    Byte[] tags = new Byte[20];
                    Byte a = 1, b = 0; ;
                    for (int i = 1; i <= 6; i++)
                    {
                        tags[i - 1] = ((CheckBox)FindControl("MyCommentTag" + i.ToString() + "_CheckBox")).Checked == true ? a : b;
                    }

                    MessageInfo mycomment = new MessageInfo();
                    mycomment.Anonymous = 1;
                    mycomment.UserID = user.UserID;
                    mycomment.CourseID = course.CourseID;
                    mycomment.Content = MyComment_TextBox.Text;
                    mycomment.Tags = tags;
                    switch (mycomment.AddComment())
                    {
                        case 1:
                            AllDataBind();
                            break;
                        case -1:
                            AllDataBind();
                            ScriptManager.RegisterStartupScript(Page, GetType(), "havecomment", "alert('评论已存在');", true);
                            break;
                        default:
                            ScriptManager.RegisterStartupScript(Page, GetType(), "commentfailed", "alert('评论失败');", true);
                            break;
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void DeleteMyComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo mycomment_ = new MessageInfo();
                mycomment_.MessageID = int.Parse(mycomment.Tables["CourseComment"].Rows[0]["CommentID"].ToString());
                if (1 == mycomment_.DeleteComment())
                {
                    AllDataBind();
                }
                else
                {
                    AllDataBind();
                    ScriptManager.RegisterStartupScript(Page, GetType(), "deletefailed", "alert('删除失败或评论不存在');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ModifyMyComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (ModifyMyComment_TextBox.Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='/aspx/Course.aspx?CourseID=" + courseid + "'</script>");
                }
                else
                {
                    Byte[] tags = new Byte[20];
                    Byte a = 1, b = 0; ;
                    for (int i = 1; i <= 6; i++)
                    {
                        tags[i - 1] = ((CheckBox)FindControl("MyCommentTagModify" + i.ToString() + "_CheckBox")).Checked == true ? a : b;
                    }

                    MessageInfo mycomment = new MessageInfo();
                    mycomment.UserID = user.UserID;
                    mycomment.CourseID = course.CourseID;
                    mycomment.Content = ModifyMyComment_TextBox.Text;
                    mycomment.Tags = tags;
                    if (1 == mycomment.ChangeComment())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        AllDataBind();
                        ScriptManager.RegisterStartupScript(Page, GetType(), "changefailed", "alert('修改失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void LikeMyComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo comment = new MessageInfo();
                comment.MessageID = int.Parse(mycomment.Tables["CourseComment"].Rows[0]["CommentID"].ToString());
                comment.UserID = user.UserID;
                if (1 == comment.AddLike())
                {
                    AllDataBind();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "likefailed", "alert('你已赞过');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void LikeComment_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo comment = new MessageInfo();
                comment.MessageID = int.Parse(e.CommandArgument.ToString());
                comment.UserID = user.UserID;
                if (1 == comment.AddLike())
                {
                    AllDataBind();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "likefailed", "alert('你已赞过');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ReplyComment_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllComment_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replycomment = new MessageInfo();
                    replycomment.Anonymous = 0;
                    replycomment.MessageID = int.Parse(e.CommandArgument.ToString());
                    replycomment.UserID = user.UserID;
                    replycomment.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replycomment.TargetReplyID = int.Parse(TargetReplyID_TextBox.Text);
                    replycomment.Content = ((TextBox)AllComment_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    if (1 == replycomment.AddCommentReply())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "replyfailed", "alert('回复失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ReplyCommentAnonymous_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllComment_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replycomment = new MessageInfo();
                    replycomment.Anonymous = 1;
                    replycomment.MessageID = int.Parse(e.CommandArgument.ToString());
                    replycomment.UserID = user.UserID;
                    replycomment.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replycomment.TargetReplyID = int.Parse(TargetReplyID_TextBox.Text);
                    replycomment.Content = ((TextBox)AllComment_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    if (1 == replycomment.AddCommentReply())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "replyfailed", "alert('回复失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ReplyMyComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyReplyComment_TextBox.Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replycomment = new MessageInfo();
                    replycomment.Anonymous = 0;
                    replycomment.MessageID = int.Parse(mycomment.Tables["CourseComment"].Rows[0]["CommentID"].ToString());
                    replycomment.UserID = user.UserID;
                    replycomment.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replycomment.TargetReplyID = int.Parse(TargetReplyID_TextBox.Text);
                    replycomment.Content = MyReplyComment_TextBox.Text;
                    if (1 == replycomment.AddCommentReply())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "replyfailed", "alert('回复失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ReplyMyCommentAnonymous_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyReplyComment_TextBox.Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replycomment = new MessageInfo();
                    replycomment.Anonymous = 1;
                    replycomment.MessageID = int.Parse(mycomment.Tables["CourseComment"].Rows[0]["CommentID"].ToString());
                    replycomment.UserID = user.UserID;
                    replycomment.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replycomment.TargetReplyID = int.Parse(TargetReplyID_TextBox.Text);
                    replycomment.Content = MyReplyComment_TextBox.Text;
                    if (1 == replycomment.AddCommentReply())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "replyfailed", "alert('回复失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void Delete_CommentReply_LinkButton_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo replycomment = new MessageInfo();
                replycomment.MessageID = int.Parse(e.CommandArgument.ToString());
                if (1 == replycomment.DeleteCommentReply())
                {
                    AllDataBind();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "deletefailed", "alert('删除失败');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void DeleteTA_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                CourseInfo c = new CourseInfo();
                c.CourseID = course.CourseID;
                c.UserID = int.Parse(e.CommandArgument.ToString());
                if (1 == c.DropCourseAssistant())
                {
                    AllDataBind();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "deletefailed", "alert('删除失败');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void AddTA_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
                if (!rex.IsMatch(TAID.Text.Trim()) || int.Parse(TAID.Text.Trim()) <= 9999)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "errortype", "alert('输入ID格式错误！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('输入ID格式错误！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    CourseInfo c = new CourseInfo();
                    c.CourseID = course.CourseID;
                    c.UserID = int.Parse(TAID.Text.Trim());
                    if (1 == c.SelectCourseAssistant())
                    {
                        AllDataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "addfailed", "alert('添加失败');", true);
                    }
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void ModifyIntro_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                course.Introduction = Modify_Intro.Text;
                if (1 == course.ChangeCourseInfo())
                {
                    AllDataBind();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "changefailed", "alert('修改失败');", true);
                }
            }
            else
            {
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('闲置时间过长，请重新登录');location.href='" + Request.Url.AbsoluteUri + "'</script>");
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpRequestValidationException)
            {
                //ClientScript.RegisterStartupScript(GetType(), "invalidletter", "alert('输入的内容包含非法字符');", true);
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('输入的内容包含非法字符');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                Server.ClearError();
            }
        }
    }
}
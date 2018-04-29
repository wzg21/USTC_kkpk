using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Web.UI.HtmlControls;

namespace TimelyEvaluation.aspx
{
    public partial class MyAdvice : System.Web.UI.Page
    {
        public string courseid;
        public UserInfo user = new UserInfo();
        public CourseInfo course = new CourseInfo();
        public DataSet Teacher_TA;
        public DataSet myadvice;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                user = Session["UserInfo"] as UserInfo;
            }
            else
            {
                Response.Redirect("/aspx/Login.aspx");
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

            Teacher_DataList.DataSource = course.GetCourseTeacher().Tables["TeacherName"];
            Teacher_DataList.DataBind();
            TA_DataList.DataSource = course.GetCourseTeacher().Tables["AssistantName"];
            TA_DataList.DataBind();
            if (Teacher_DataList.Items.Count == 0 && TA_DataList.Items.Count == 0)
            {
                ((HtmlGenericControl)Page.FindControl("No_t")).InnerHtml = "由于这门课程的老师或助教未注册，暂时无法发表建议o(╥﹏╥)o";
                Page.FindControl("MyAdvice_TextBox").Visible = false;
                Page.FindControl("MyAdvice_Button").Visible = false;
                Page.FindControl("MyAnonymousAdvice_Button").Visible = false;
            }

            myadvice = course.GetMyCourseAdvice();
            AspNetPager.RecordCount = myadvice.Tables["CourseAdvice"].Rows.Count;
            AllMyAdvice_DataList.DataSource = FilterTable(myadvice.Tables["CourseAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllMyAdvice_DataList.DataBind();
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
            AllMyAdvice_DataList.DataSource = FilterTable(myadvice.Tables["CourseAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllMyAdvice_DataList.DataBind();
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

        protected void AllMyAdvice_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                MessageInfo advice = new MessageInfo();
                advice.MessageID = int.Parse(DataBinder.Eval(e.Item.DataItem, "AdviceID").ToString());

                DataList dataList = e.Item.FindControl("ReplyAdvice_DataList") as DataList;
                DataSet advicereply = advice.GetAdviceReply();
                dataList.DataSource = advicereply;
                dataList.DataBind();

                ((HtmlGenericControl)e.Item.FindControl("ReplyNum_div")).InnerHtml = advicereply.Tables["Reply"].Rows.Count.ToString();
                ((HtmlGenericControl)e.Item.FindControl("ReplyNum_div")).TagName = "ReplyNum_div_Name" + DataBinder.Eval(e.Item.DataItem, "AdviceID").ToString();
            }
        }

        protected void Teacher_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ((HtmlGenericControl)e.Item.FindControl("name")).InnerHtml = DataBinder.Eval(e.Item.DataItem, "UserName").ToString();
                TextBox userid = e.Item.FindControl("UserID_TextBox") as TextBox;
                userid.Text = DataBinder.Eval(e.Item.DataItem, "UserID").ToString();
            }
        }

        protected void TA_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ((HtmlGenericControl)e.Item.FindControl("name")).InnerHtml = DataBinder.Eval(e.Item.DataItem, "UserName").ToString();
                TextBox userid = e.Item.FindControl("UserID_TextBox") as TextBox;
                userid.Text = DataBinder.Eval(e.Item.DataItem, "UserID").ToString();
            }
        }

        protected void MyAdvice_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyAdvice_TextBox.Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='/aspx/MyAdvice.aspx?CourseID=" + courseid + "'</script>");
                }
                else
                {
                    ArrayList shieldid = new ArrayList();
                    ArrayList unshieldid = new ArrayList();
                    foreach (DataListItem item in Teacher_DataList.Items)
                    {
                        if (((HtmlInputCheckBox)item.FindControl("TemporaryID")).Checked)
                        {
                            unshieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                        else
                        {
                            shieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                    }
                    foreach (DataListItem item in TA_DataList.Items)
                    {
                        if (((HtmlInputCheckBox)item.FindControl("TemporaryID")).Checked)
                        {
                            unshieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                        else
                        {
                            shieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                    }
                    unshieldid.Add(0);
                    shieldid.Add(0);

                    MessageInfo myadvice = new MessageInfo();
                    myadvice.Anonymous = 0;
                    myadvice.UnShieldID = (Int32[])unshieldid.ToArray(typeof(Int32));
                    myadvice.ShieldID = (Int32[])shieldid.ToArray(typeof(Int32));
                    myadvice.UserID = user.UserID;
                    myadvice.CourseID = course.CourseID;
                    myadvice.Content = MyAdvice_TextBox.Text;
                    myadvice.AddAdvice();

                    AllDataBind();
                    //Response.Redirect("/aspx/MyAdvice.aspx?CourseID=" + courseid + "");
                }
            }
        }

        protected void MyAnonymousAdvice_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                if (MyAdvice_TextBox.Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='/aspx/MyAdvice.aspx?CourseID=" + courseid + "'</script>");
                }
                else
                {
                    ArrayList shieldid = new ArrayList();
                    ArrayList unshieldid = new ArrayList();
                    foreach (DataListItem item in Teacher_DataList.Items)
                    {
                        if (((HtmlInputCheckBox)item.FindControl("TemporaryID")).Checked)
                        {
                            unshieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                        else
                        {
                            shieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                    }
                    foreach (DataListItem item in TA_DataList.Items)
                    {
                        if (((HtmlInputCheckBox)item.FindControl("TemporaryID")).Checked)
                        {
                            unshieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                        else
                        {
                            shieldid.Add(int.Parse(((TextBox)item.FindControl("UserID_TextBox")).Text));
                        }
                    }
                    unshieldid.Add(0);
                    shieldid.Add(0);

                    MessageInfo myadvice = new MessageInfo();
                    myadvice.Anonymous = 1;
                    myadvice.UnShieldID = (Int32[])unshieldid.ToArray(typeof(Int32));
                    myadvice.ShieldID = (Int32[])shieldid.ToArray(typeof(Int32));
                    myadvice.UserID = user.UserID;
                    myadvice.CourseID = course.CourseID;
                    myadvice.Content = MyAdvice_TextBox.Text;
                    myadvice.AddAdvice();

                    AllDataBind();
                    //Response.Redirect("/aspx/MyAdvice.aspx?CourseID=" + courseid + "");
                }
            }
        }

        protected void Delete_Advice_LinkButton_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo myadvice = new MessageInfo();
                myadvice.MessageID = int.Parse(e.CommandArgument.ToString());
                myadvice.DeleteAdvice();

                AllDataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ReplyAdvice_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replyadvice = new MessageInfo();
                    replyadvice.Anonymous = 0;
                    replyadvice.MessageID = int.Parse(e.CommandArgument.ToString());
                    replyadvice.UserID = user.UserID;
                    replyadvice.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replyadvice.Content = ((TextBox)AllMyAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    replyadvice.AddAdviceReply();

                    AllDataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void ReplyAdviceAnonymous_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    MessageInfo replyadvice = new MessageInfo();
                    replyadvice.Anonymous = 1;
                    replyadvice.MessageID = int.Parse(e.CommandArgument.ToString());
                    replyadvice.UserID = user.UserID;
                    replyadvice.TargetUserID = int.Parse(TargetUserID_TextBox.Text);
                    replyadvice.Content = ((TextBox)AllMyAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    replyadvice.AddAdviceReply();

                    AllDataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void Delete_AdviceReply_LinkButton_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo replyadvice = new MessageInfo();
                replyadvice.MessageID = int.Parse(e.CommandArgument.ToString());
                replyadvice.DeleteAdviceReply();

                AllDataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
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
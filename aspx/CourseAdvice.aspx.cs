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
    public partial class CourseAdvice : System.Web.UI.Page
    {
        public string courseid;
        public UserInfo user = new UserInfo();
        public CourseInfo course = new CourseInfo();
        public DataSet Teacher_TA;
        public DataSet alladvice;

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

            int flag = 0;
            for (int i = 0; i < Teacher_TA.Tables["TeacherName"].Rows.Count;i++ )
            {
                if(user.UserID==int.Parse(Teacher_TA.Tables["TeacherName"].Rows[i][0].ToString()))
                {
                    flag = 1;
                    break;
                }
            }
            for (int i = 0; i < Teacher_TA.Tables["AssistantName"].Rows.Count; i++)
            {
                if (user.UserID == int.Parse(Teacher_TA.Tables["AssistantName"].Rows[i][0].ToString()))
                {
                    flag = 1;
                    break;
                }
            }
            if(flag==0)
            {
                Response.Redirect("/aspx/Login.aspx");
            }

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

            Modify_Intro.Text = course.Introduction;
            T_DataList.DataSource = course.GetCourseTeacher().Tables["TeacherName"];
            T_DataList.DataBind();
            A_DataList.DataSource = course.GetCourseTeacher().Tables["AssistantName"];
            A_DataList.DataBind();
            alladvice = course.GetCourseAdvice();
            AspNetPager.RecordCount = alladvice.Tables["CourseAdvice"].Rows.Count;
            AllAdvice_DataList.DataSource = FilterTable(alladvice.Tables["CourseAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllAdvice_DataList.DataBind();
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
            AllAdvice_DataList.DataSource = FilterTable(alladvice.Tables["CourseAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllAdvice_DataList.DataBind();
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

        protected void AllAdvice_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
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

        protected void ReplyAdvice_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
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
                    replyadvice.Content = ((TextBox)AllAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text;
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

        protected void DeleteTA_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                CourseInfo c = new CourseInfo();
                c.CourseID = course.CourseID;
                c.UserID = int.Parse(e.CommandArgument.ToString());
                c.DropCourseAssistant();

                AllDataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void AddTA_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                CourseInfo c = new CourseInfo();
                c.CourseID = course.CourseID;
                c.UserID = int.Parse(TAID.Text.Trim());
                c.SelectCourseAssistant();

                AllDataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ModifyIntro_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                course.Introduction = Modify_Intro.Text;
                course.ChangeCourseInfo();

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Web.UI.HtmlControls;

namespace TimelyEvaluation.aspx
{
    public partial class MyCourseList : System.Web.UI.Page
    {
        public string opertype;
        public UserInfo user = new UserInfo();

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

            opertype = Request.QueryString["OperType"];
            if (opertype == null)
            {
                Response.Redirect("/aspx/CourseList.aspx");
            }

            if (!Page.IsPostBack)
            {
                AllDataBind();
            }       
        }

        private void AllDataBind()
        {
            if (opertype == "AsStudent")
            {
                MyStudyingCourseList_DataList.DataSource = user.GetStudyingCourse();
                MyStudyingCourseList_DataList.DataBind();
                MyStudiedCourseList_DataList.DataSource = user.GetStudiedCourse();
                MyStudiedCourseList_DataList.DataBind();
            }
            else if (opertype == "AsTA")
            {
                MyTeachingCourseList_DataList.DataSource = user.GetTeachingCourse();
                MyTeachingCourseList_DataList.DataBind();
                MyTaughtCourseList_DataList.DataSource = user.GetTaughtCourse();
                MyTaughtCourseList_DataList.DataBind();
            }
        }

        protected void CourseList_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CourseInfo course = new CourseInfo();
                course.CourseID = int.Parse(DataBinder.Eval(e.Item.DataItem, "CourseID").ToString());

                DataList dataList = e.Item.FindControl("Teacher_DataList") as DataList;
                dataList.DataSource = course.GetCourseTeacher().Tables["TeacherName"];
                dataList.DataBind();
                if (dataList.Items.Count == 0)
                {
                    ((HtmlGenericControl)e.Item.FindControl("No_t")).InnerHtml = "暂未注册";
                }
                DataList dataList2 = e.Item.FindControl("TA_DataList") as DataList;
                dataList2.DataSource = course.GetCourseTeacher().Tables["AssistantName"];
                dataList2.DataBind();
                if (dataList2.Items.Count == 0)
                {
                    ((HtmlGenericControl)e.Item.FindControl("No_ta")).InnerHtml = "暂未注册";
                }
            }
        }

        protected void DropCourse_Button_Click(object sender, CommandEventArgs e)
        {
            CourseInfo course = new CourseInfo();
            course.UserID = user.UserID;
            course.CourseID = int.Parse(e.CommandArgument.ToString());
            course.DropCourse();

            AllDataBind();
            //Response.Redirect(Request.Url.AbsoluteUri);
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
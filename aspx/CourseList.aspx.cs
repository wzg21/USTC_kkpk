using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Data;
using System.Web.UI.HtmlControls;

namespace TimelyEvaluation.aspx
{
    public partial class CourseList : System.Web.UI.Page
    {
        public UserInfo user = new UserInfo();
        public CourseInfo course = new CourseInfo();
        public DataSet allcourse;
        string coursename, coursenum, teacher;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                user = Session["UserInfo"] as UserInfo;
                course.UserID = user.UserID;
            }

            coursename = Request.QueryString["CNA"];
            if (coursename == "") { coursename = null;}
            coursenum = Request.QueryString["CNU"];
            if (coursenum == "") { coursenum = null;}
            teacher = Request.QueryString["T"];
            if (teacher == "") { teacher = null;}

            if (!Page.IsPostBack)
            {
                allcourse = course.GetAllCourse(coursenum, coursename, teacher);
                AspNetPager.RecordCount = allcourse.Tables["AllCourse"].Rows.Count;
                CourseList_DataList.DataSource = FilterTable(allcourse.Tables["AllCourse"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                CourseList_DataList.DataBind();
            }
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
            CourseList_DataList.DataSource = FilterTable(allcourse.Tables["AllCourse"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            CourseList_DataList.DataBind();
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

        protected void SearchCourse_Button_Click(object sender, EventArgs e)
        {
            string coursenum = CourseNum_TextBox.Text.Trim();//获取课程号
            string teacher = Teacher_TextBox.Text.Trim(); //获取教师姓名
            string coursename = CourseName_TextBox.Text.Trim(); //获取课程名

            //这个不用改
            Response.Redirect("/aspx/CourseList.aspx?CNA=" + coursename + "&CNU=" + coursenum + "&T=" + teacher);
        }

        protected void AddDropCourse_Button_Click(object sender, CommandEventArgs e)
        {
            if(Session["UserInfo"] != null)
            {
                CourseInfo course = new CourseInfo();
                course.UserID = user.UserID;

                string[] arg = e.CommandArgument.ToString().Split(',');
                string courseid = arg[0];
                string ischosen = arg[1];
                course.CourseID = int.Parse(courseid);
                if (int.Parse(ischosen) == 0)
                {
                    course.SelectCourse();
                }
                if (int.Parse(ischosen) == 1)
                {
                    course.DropCourse();
                }

                allcourse = course.GetAllCourse(coursenum, coursename, teacher);
                AspNetPager.RecordCount = allcourse.Tables["AllCourse"].Rows.Count;
                CourseList_DataList.DataSource = FilterTable(allcourse.Tables["AllCourse"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                CourseList_DataList.DataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
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
                HttpContext.Current.Response.Write("<script type='text/javascript'>alert('输入的内容包含非法字符');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                Server.ClearError();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using TimelyEvaluation.DataBase;
using System.Web.UI.HtmlControls;

namespace TimelyEvaluation.aspx
{
    public partial class MyNewAdvice : System.Web.UI.Page
    {
        public UserInfo user = new UserInfo();
        public DataSet mynewadvice;

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

            if (!Page.IsPostBack)
            {
                mynewadvice = user.GetNewAdvice();
                AspNetPager.RecordCount = mynewadvice.Tables["NewAdvice"].Rows.Count;
                AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdvice_DataList.DataBind();
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
            AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllMyNewAdvice_DataList.DataBind();
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

        protected void AllMyNewAdvice_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
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
                if (((TextBox)AllMyNewAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
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
                    replyadvice.Content = ((TextBox)AllMyNewAdvice_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    replyadvice.AddAdviceReply();

                    mynewadvice = user.GetNewAdvice();
                    AspNetPager.RecordCount = mynewadvice.Tables["NewAdvice"].Rows.Count;
                    AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewAdvice_DataList.DataBind();
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

                mynewadvice = user.GetNewAdvice();
                AspNetPager.RecordCount = mynewadvice.Tables["NewAdvice"].Rows.Count;
                AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdvice_DataList.DataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ReadAllNewAdvice_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo advice = new MessageInfo();
                advice.UserID = user.UserID;
                advice.Type = 2;
                advice.ReadType();

                mynewadvice = user.GetNewAdvice();
                AspNetPager.RecordCount = mynewadvice.Tables["NewAdvice"].Rows.Count;
                AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdvice_DataList.DataBind();
                //Response.Redirect("/aspx/MyNewAdvice.aspx");
            }
        }

        protected void ReadNewAdvice_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo advice = new MessageInfo();
                advice.UserID = user.UserID;
                advice.MessageID = int.Parse(e.CommandArgument.ToString());
                advice.Type = 2;
                advice.Read();

                mynewadvice = user.GetNewAdvice();
                AspNetPager.RecordCount = mynewadvice.Tables["NewAdvice"].Rows.Count;
                AllMyNewAdvice_DataList.DataSource = FilterTable(mynewadvice.Tables["NewAdvice"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdvice_DataList.DataBind();
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
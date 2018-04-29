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
    public partial class MyNewAdviceReply : System.Web.UI.Page
    {
        public UserInfo user = new UserInfo();
        public DataSet mynewadvicereply;

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
                mynewadvicereply = user.GetNewAdviceReply();
                AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdviceReply_DataList.DataBind();
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
            AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllMyNewAdviceReply_DataList.DataBind();
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

        protected void AllMyNewAdviceReply_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
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
            }
        }

        protected void Reply_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    string[] arg = e.CommandArgument.ToString().Split(',');
                    string adviceid = arg[0];
                    string targetuserid = arg[1];

                    MessageInfo reply = new MessageInfo();
                    reply.Anonymous = 0;
                    reply.MessageID = int.Parse(adviceid);
                    reply.UserID = user.UserID;
                    reply.TargetUserID = int.Parse(targetuserid);
                    reply.Content = ((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    reply.AddAdviceReply();

                    mynewadvicereply = user.GetNewAdviceReply();
                    AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                    AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewAdviceReply_DataList.DataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void ReplyAnonymous_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "nocontent", "alert('请输入内容！');", true);
                    //HttpContext.Current.Response.Write("<script type='text/javascript'>alert('请输入内容！');location.href='" + Request.Url.AbsoluteUri + "'</script>");
                }
                else
                {
                    string[] arg = e.CommandArgument.ToString().Split(',');
                    string adviceid = arg[0];
                    string targetuserid = arg[1];

                    MessageInfo reply = new MessageInfo();
                    reply.Anonymous = 1;
                    reply.MessageID = int.Parse(adviceid);
                    reply.UserID = user.UserID;
                    reply.TargetUserID = int.Parse(targetuserid);
                    reply.Content = ((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    reply.AddAdviceReply();

                    mynewadvicereply = user.GetNewAdviceReply();
                    AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                    AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewAdviceReply_DataList.DataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void ReplyAdvice_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("ReplyAdvice_TextBox")).Text.Trim() == "")
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
                    replyadvice.Content = ((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("ReplyAdvice_TextBox")).Text;
                    replyadvice.AddAdviceReply();

                    mynewadvicereply = user.GetNewAdviceReply();
                    AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                    AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewAdviceReply_DataList.DataBind();
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
                if (((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("ReplyAdvice_TextBox")).Text.Trim() == "")
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
                    replyadvice.Content = ((TextBox)AllMyNewAdviceReply_DataList.Items[index].FindControl("ReplyAdvice_TextBox")).Text;
                    replyadvice.AddAdviceReply();

                    mynewadvicereply = user.GetNewAdviceReply();
                    AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                    AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewAdviceReply_DataList.DataBind();
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

                mynewadvicereply = user.GetNewAdviceReply();
                AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdviceReply_DataList.DataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ReadAllNewAdviceReply_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo replyadvice = new MessageInfo();
                replyadvice.UserID = user.UserID;
                replyadvice.Type = 4;
                replyadvice.ReadType();

                mynewadvicereply = user.GetNewAdviceReply();
                AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdviceReply_DataList.DataBind();
                //Response.Redirect("/aspx/MyNewAdviceReply.aspx");
            }
        }

        protected void ReadNewAdviceReply_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo replyadvice = new MessageInfo();
                replyadvice.UserID = user.UserID;
                replyadvice.MessageID = int.Parse(e.CommandArgument.ToString());
                replyadvice.Type = 4;
                replyadvice.Read();

                mynewadvicereply = user.GetNewAdviceReply();
                AspNetPager.RecordCount = mynewadvicereply.Tables["NewAdviceReply"].Rows.Count;
                AllMyNewAdviceReply_DataList.DataSource = FilterTable(mynewadvicereply.Tables["NewAdviceReply"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewAdviceReply_DataList.DataBind();
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
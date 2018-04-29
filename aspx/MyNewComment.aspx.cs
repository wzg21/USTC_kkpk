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
    public partial class MyNewComment : System.Web.UI.Page
    {
        public UserInfo user = new UserInfo();
        public DataSet mynewcomment;

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
                mynewcomment = user.GetNewComment();
                AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewComment_DataList.DataBind();
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
            AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
            AllMyNewComment_DataList.DataBind();
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

        protected void AllMyNewComment_DataList_ItemDataBound(object sender, DataListItemEventArgs e)
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

        protected void LikeComment_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo comment = new MessageInfo();
                comment.MessageID = int.Parse(e.CommandArgument.ToString());
                comment.UserID = user.UserID;
                comment.AddLike();

                mynewcomment = user.GetNewComment();
                AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewComment_DataList.DataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ReplyComment_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyNewComment_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
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
                    replycomment.Content = ((TextBox)AllMyNewComment_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    replycomment.AddCommentReply();

                    mynewcomment = user.GetNewComment();
                    AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                    AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewComment_DataList.DataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void ReplyCommentAnonymous_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                DataListItem Item = (DataListItem)(((Control)sender).NamingContainer);
                int index = Item.ItemIndex;
                if (((TextBox)AllMyNewComment_DataList.Items[index].FindControl("Reply_TextBox")).Text.Trim() == "")
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
                    replycomment.Content = ((TextBox)AllMyNewComment_DataList.Items[index].FindControl("Reply_TextBox")).Text;
                    replycomment.AddCommentReply();

                    mynewcomment = user.GetNewComment();
                    AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                    AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                    AllMyNewComment_DataList.DataBind();
                    //Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        protected void Delete_CommentReply_LinkButton_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo replycomment = new MessageInfo();
                replycomment.MessageID = int.Parse(e.CommandArgument.ToString());
                replycomment.DeleteCommentReply();

                mynewcomment = user.GetNewComment();
                AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewComment_DataList.DataBind();
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void ReadAllNewComment_Button_Click(object sender, EventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo comment = new MessageInfo();
                comment.UserID = user.UserID;
                comment.Type = 1;
                comment.ReadType();

                mynewcomment = user.GetNewComment();
                AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewComment_DataList.DataBind();
                //Response.Redirect("/aspx/MyNewComment.aspx");
            }
        }

        protected void ReadNewComment_Button_Click(object sender, CommandEventArgs e)
        {
            if (Session["UserInfo"] != null)
            {
                MessageInfo comment = new MessageInfo();
                comment.UserID = user.UserID;
                comment.MessageID = int.Parse(e.CommandArgument.ToString());
                comment.Type = 1;
                comment.Read();

                mynewcomment = user.GetNewComment();
                AspNetPager.RecordCount = mynewcomment.Tables["NewComment"].Rows.Count;
                AllMyNewComment_DataList.DataSource = FilterTable(mynewcomment.Tables["NewComment"], AspNetPager.StartRecordIndex - 1, AspNetPager.EndRecordIndex - 1);
                AllMyNewComment_DataList.DataBind();
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
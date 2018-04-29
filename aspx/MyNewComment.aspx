﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyNewComment.aspx.cs" Inherits="TimelyEvaluation.aspx.MyNewComment" %>

<form id="form1" runat="server">

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/Public.css" >
<link rel="stylesheet" type="text/css" href="/css/MyNewMassage.css" >
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_我的新消息</title>

    <script src="/scripts/jquery-2.1.1/jquery.min.js"></script>
    <script type="text/javascript">
        function Exit() {
            if (confirm("确定要退出吗？")) {
                window.location = "/aspx/Exit.aspx";
            }
        }

        function ShowReplyShowRegion(commentid) {
            $("div[id ='ReplyRegion_div" + commentid + "']").hide();
            if ($("div[id ='ReplyShowRegion_div" + commentid + "']").is(":hidden")) {
                $("div[id ='ReplyShowRegion_div" + commentid + "']").show();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("收起回复");
            }
            else {
                $("div[id ='ReplyShowRegion_div" + commentid + "']").hide();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("回复");
            }
        }
        function ShowReplyRegion(targetreply, targetuser, commentid) {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);

            $("div[id ='ReplyRegion_div" + commentid + "']").show();
        }

        function ShowReplyRegionZero(targetreply, targetuser, commentid) {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);
            if ($("div[id ='ReplyRegion_div" + commentid + "']").is(":hidden")) {
                $("div[id ='ReplyRegion_div" + commentid + "']").show();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("收起回复");
            }
            else {
                $("div[id ='ReplyRegion_div" + commentid + "']").hide();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("回复");
            }
        }

        function JudgeZeroReply(commentid, targetuser) {
            var divList = document.getElementsByTagName('ReplyNum_div_Name' + commentid);
            if (divList[0].innerHTML.toString() == "0") {
                ShowReplyRegionZero('0', targetuser, commentid);
            }
            else {
                ShowReplyShowRegion(commentid);
            }
        }
        function ShowGuide(FatherTag, SonTag) {
            if (SonTag.style.display == "block") {
                SonTag.style.display = "none";
                FatherTag.className = "folder";
            } else {
                SonTag.style.display = "block";
                FatherTag.className = "unfolder";
            }
        }

        function MaxLength(field, maxlimit) {
            var j = field.value.replace(/[^\x00-\xff]/g, "**").length;
            //alert(j); 
            var tempString = field.value;
            var tt = "";
            if (j > maxlimit) {
                for (var i = 0; i < maxlimit; i++) {
                    if (tt.replace(/[^\x00-\xff]/g, "**").length < maxlimit)
                        tt = tempString.substr(0, i + 1);
                    else
                        break;
                }
                if (tt.replace(/[^\x00-\xff]/g, "**").length > maxlimit)
                    tt = tt.substr(0, tt.length - 1);
                field.value = tt;
            }
            else {
                ;
            }
        }
    </script>

</head>

<body>
    <asp:TextBox ID="TargetUserID_TextBox" runat="server" style="display:none"></asp:TextBox>
    <asp:TextBox ID="TargetReplyID_TextBox" runat="server" style="display:none"></asp:TextBox>
    <h1 >科科评科</h1>
    <%--以下这个section是侧边的导航栏，注意用户的头像和用户名写成了超链接a，可以做成鼠标移到或点击这个超链接显示导航栏--%>
    <%--如果用户没有登录，导航栏的内容是首页的超链接和登录注册按钮--%>
    <section id="Guide">
        <br />
        <%if (Session["UserInfo"] == null)
          { %>
              <div><a href="/aspx/CourseList.aspx">首页</a></div>
              <button class="Button0" type="button" onclick="window.location='/aspx/Login.aspx'">登录</button>
              <button class="Button0" type="button" onclick="window.location='/aspx/Register.aspx'">注册</button>
        <%} %>
        <%else
          { %>
              <ul>
                  <li>
                          <div class="GuideUser">
                              <%=user.UserName %> 
                          </div>
                          <li><a href="/aspx/CourseList.aspx">首页</a></li>
                          <%if(user.isStudent==1){ %>
                          <li><a id="StudentTag" <%--href=""--%> class="folder" onclick="ShowGuide(StudentTag,HideStuTag)">学生▿</li>
                              <ul id="HideStuTag">
                                  <li><a href="/aspx/MyCourseList.aspx?OperType=AsStudent">我修的课程</a></li>
                              </ul>
                          <li><a id="TATag" <%--href=""--%> class="folder" onclick="ShowGuide(TATag,HideTATag)">助教▿</li>
                              <ul id="HideTATag">
                                  <li><a href="/aspx/MyCourseList.aspx?OperType=AsTA">我带的课程</a></li>
                              </ul>
                          <%} %>
                          <%if(user.isStudent==0){ %>
                          <li><a id="TeacherTag" <%--href=""--%> class="folder" onclick="ShowGuide(TeacherTag,HideTeacherTag)">教师▿</li>
                              <ul id="HideTeacherTag">
                                  <li><a href="/aspx/MyCourseList.aspx?OperType=AsTA">我带的课程</a></li>
                              </ul>
                          <%} %>
                          <li><a id="MyNewInfo" class="folder" onclick="ShowGuide(MyNewInfo,HideNewInfo)">我的消息▿</li>
                              <ul id="HideNewInfo">
                                  <%System.Data.DataSet msgnum = user.GetNewMessageNum(); %>
                                  <li><a href="/aspx/MyNewAdviceReply.aspx">新的建议回复</a>(<%=msgnum.Tables["NewMessageNum"].Rows[3][1].ToString() %>)</li>
                                  <li><a href="/aspx/MyNewCommentReply.aspx">新的评论回复</a>(<%=msgnum.Tables["NewMessageNum"].Rows[2][1].ToString() %>)</li>
                                  <li><a href="/aspx/MyNewComment.aspx">新的课程评论</a>(<%=msgnum.Tables["NewMessageNum"].Rows[0][1].ToString() %>)</li>
                                  <li><a href="/aspx/MyNewAdvice.aspx">新的课程建议</a>(<%=msgnum.Tables["NewMessageNum"].Rows[1][1].ToString() %>)</li>
                             </ul>
                          <li><a href="/aspx/Information.aspx?UID=<%=user.UserID.ToString()%>">我的个人信息</a></li>
                          <li><a href="javascript:void(0);" onclick="Exit()">退出</a></li>

                  </li>
              </ul>
        <%} %>
    </section>
    
    <section class="Content">
        
        <%--<br />-------------------------------------------------------------------------<br />--%>
        <h2>新收到的评论：</h2>
        <%--新收到的评论：<br />--%>
        <asp:Button class="Button0" runat="server" Text="全部设为已读" OnClick="ReadAllNewComment_Button_Click"/>
        <asp:ScriptManager ID="MyNewComment_ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="MyNewComment_UpdatePanel" runat="server" UpdateMode="Always"> 
        <ContentTemplate> 
        <asp:DataList ID="AllMyNewComment_DataList" runat="server" OnItemDataBound="AllMyNewComment_DataList_ItemDataBound">

            <ItemTemplate>
                <div class="OneItem">
                (<%#Eval("CourseName") %>)<a class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a>:
                <pre><%#Eval("Content") %></pre> 
                    <br />
                <%#((Byte[])Eval("Tags"))[0] == (Byte)1? "<span class='CommentTag'>课程简单</span>":"" %>
                <%#((Byte[])Eval("Tags"))[1] == (Byte)1? "<span class='CommentTag'>讲课清晰</span>":"" %>
                <%#((Byte[])Eval("Tags"))[2] == (Byte)1? "<span class='CommentTag'>解惑及时</span>":"" %>
                <%#((Byte[])Eval("Tags"))[3] == (Byte)1? "<span class='CommentTag'>作业量少</span>":"" %>
                <%#((Byte[])Eval("Tags"))[4] == (Byte)1? "<span class='CommentTag'>给分较好</span>":"" %>
                <%#((Byte[])Eval("Tags"))[5] == (Byte)1? "<span class='CommentTag'>收获较大</span>":"" %>
                <p class="CommentTime">
                发表时间<%#Eval("CreateDate") %>
                 <a id="ReplyShowLink_a<%#Eval("CommentID") %>" href="javascript:void(0);" onclick="JudgeZeroReply('<%#Eval("CommentID") %>','<%#Eval("UserID") %>')">回复</a>(<div id="ReplyNum_div" tagname="ReplyNum_div_Name" runat="server" style="display:inline" ></div>)
                <asp:Button class="Like_Button" runat="server" CommandArgument='<%#Eval("CommentID") %>' OnCommand="LikeComment_Button_Click"/>
                (<%#Eval("LikeCounts") %>)
                </p>        
                <asp:Button class="Button0" runat="server" Text="设为已读" CommandArgument='<%#Eval("CommentID") %>' OnCommand="ReadNewComment_Button_Click"/>
                <div id="ReplyShowRegion_div<%#Eval("CommentID") %>" style="display:none">

                    <asp:DataList ID="ReplyComment_DataList" runat="server">
                        <ItemTemplate>
                        &nbsp&nbsp&nbsp&nbsp<a class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a></span> 回复 <span class="CommentName"><a class="Username" href='<%#int.Parse(Eval("TargetUserID").ToString())>9999? "/aspx/Information.aspx?UID=" + Eval("TargetUserID").ToString():"javascript:void(0);"%>'><%#Eval("TargetUserName") %></a>:<%#Eval("Content") %>
                        <asp:LinkButton runat="server" Text="删除这条回复" CommandArgument='<%#Eval("ReplyID") %>' OnCommand="Delete_CommentReply_LinkButton_Click" Visible='<%#int.Parse(Eval("RealUserID").ToString()) == user.UserID%>'/>
                        <button class="Button0" type="button" onclick="ShowReplyRegion('<%#Eval("ReplyID") %>','<%#Eval("UserID") %>','<%#Eval("CommentID") %>')" >回复</button>
                        </ItemTemplate>
                    </asp:DataList>
                    <div><button class="Button0" type="button" onclick="ShowReplyRegion('0','<%#Eval("UserID") %>','<%#Eval("CommentID") %>')">我也说一句</button></div>
                </div>

                <div id="ReplyRegion_div<%#Eval("CommentID") %>" style="display:none">
                    <asp:TextBox class="TextBox0" ID="Reply_TextBox" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                    <p  class="SubmitComment">
                    <asp:Button class="Button0" runat="server" Text="发表" CommandArgument='<%#Eval("CommentID") %>' OnCommand="ReplyComment_Button_Click"/>
                    &nbsp<asp:Button class="Button0" runat="server" Text="匿名发表" CommandArgument='<%#Eval("CommentID") %>' OnCommand="ReplyCommentAnonymous_Button_Click"/>
                    &nbsp&nbsp&nbsp</p>
                </div>
                </div>
                <%--<br />-------------------------------------------------------------------------<br />--%>
            </ItemTemplate>
        </asp:DataList>
        </ContentTemplate>
        </asp:UpdatePanel>

        <%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
            <webdiyer:AspNetPager CssClass="pagination" PagingButtonSpacing="0" CurrentPageButtonClass="active"
                 ID="AspNetPager" runat="server" AlwaysShow="true"  
                PageSize="6" onpagechanging="AspNetPager_PageChanged" FirstPageText="<<"  
                LastPageText=">>" NextPageText=">" PrevPageText="<" UrlPaging="True" 
                ShowFirstLast="true" ShowPageIndexBox="Never"  EnableUrlRewriting="True" UrlRewritePattern="/aspx/MyNewComment.aspx?Page={0}"></webdiyer:AspNetPager>
    </section>
</body>
    
</html>
</form>



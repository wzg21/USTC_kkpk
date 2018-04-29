<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Course.aspx.cs" Inherits="TimelyEvaluation.aspx.Course" %>

<form id="form1" runat="server">

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/Public.css" >
<link rel="stylesheet" type="text/css" href="/css/Course.css" >
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_课程评论</title>

    <script src="/scripts/jquery-2.1.1/jquery.min.js"></script>
    <script type="text/javascript">
        function Exit()
        {
            if (confirm("确定要退出吗？"))
            {
                window.location = "/aspx/Exit.aspx";
            }
        }

        function AddDefaultText(obj) {
            if (obj.value == "") {
                obj.value = "你还没有对此课程发表任何评论哦";
                if (document.getElementById("MyComment_TextBox") != null)
                    document.getElementById("MyComment_TextBox").style.cssText = "color:#BEBEBE";//修改字体颜色 
                else
                    document.getElementById("MyCommentDisable_TextBox").style.cssText = "color:#BEBEBE";//修改字体颜色 
            }
        }
        function ClearDafaultText(obj) {
            if (obj.value == "你还没有对此课程发表任何评论哦") {
                obj.value = "";
                if (document.getElementById("MyComment_TextBox") != null)
                    document.getElementById("MyComment_TextBox").style.cssText = "color:#000000";
                else
                    document.getElementById("MyCommentDisable_TextBox").style.cssText = "color:#000000";
            }
        }

        function ShowMyCommentRegion()
        {
            document.getElementById("ShowMyComment_div").style.display = 'block';
            document.getElementById("ShowMyCommentModify_div").style.display = 'none';
        }
        function ShowMyCommentModifyRegion()
        {
            for (var i = 1; i <= 6; i++)
            {
                if(document.getElementById("MyCommentTagModify"+i.toString()+"_CheckBox").checked)
                {
                    document.getElementById("MyCommentTagModify" + i.toString() + "_Label").className = "labelchecked";
                }
            }
            document.getElementById("ShowMyCommentModify_div").style.display = 'block';
            document.getElementById("ShowMyComment_div").style.display = 'none';
        }

        function ShowMyCommentReplyShowRegion()
        {
            $("div[id ='MyCommentReplyRegion_div']").hide();
            if ($("div[id ='MyCommentReplyShowRegion_div']").is(":hidden")) {
                $("div[id ='MyCommentReplyShowRegion_div']").show();
                $("a[id ='MyReplyShowLink_a']").text("收起回复");
            }
            else {
                $("div[id ='MyCommentReplyShowRegion_div']").hide();
                $("a[id ='MyReplyShowLink_a']").text("回复");
            }
        }

        function ShowMyCommentReplyRegion(targetreply, targetuser, commentid)
        {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);

            $("div[id ='MyCommentReplyRegion_div']").show();
        }

        function ShowMyCommentReplyRegionZero(targetreply, targetuser, commentid)
        {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);

            if ($("div[id ='MyCommentReplyRegion_div']").is(":hidden"))
            {
                $("div[id ='MyCommentReplyRegion_div']").show();
                $("a[id ='MyReplyShowLink_a']").text("收起回复");
            }
            else
            {
                $("div[id ='MyCommentReplyRegion_div']").hide();
                $("a[id ='MyReplyShowLink_a']").text("回复");
            }
        }

        function JudgeMyZeroReply(commentid, targetuser)
        {
            var divList = document.getElementsByTagName('MyReplyNum_div_Name');
            if (divList[0].innerHTML.toString() == "0") {
                ShowMyCommentReplyRegionZero('0', targetuser, commentid);
            }
            else {
                ShowMyCommentReplyShowRegion(commentid);
            }
        }

        function ShowReplyShowRegion(commentid)
        {
            $("div[id ='ReplyRegion_div" + commentid + "']").hide();
            if ($("div[id ='ReplyShowRegion_div" + commentid + "']").is(":hidden"))
            {
                $("div[id ='ReplyShowRegion_div" + commentid + "']").show();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("收起回复");
            }
            else
            {
                $("div[id ='ReplyShowRegion_div" + commentid + "']").hide();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("回复");
            }
        }
        function ShowReplyRegion(targetreply, targetuser, commentid)
        {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);

            $("div[id ='ReplyRegion_div" + commentid + "']").show();
        }

        function ShowReplyRegionZero(targetreply, targetuser, commentid)
        {
            $("#TargetUserID_TextBox").val(targetuser);
            $("#TargetReplyID_TextBox").val(targetreply);
            if ($("div[id ='ReplyRegion_div" + commentid + "']").is(":hidden"))
            {
                $("div[id ='ReplyRegion_div" + commentid + "']").show();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("收起回复");
            }
            else
            {
                $("div[id ='ReplyRegion_div" + commentid + "']").hide();
                $("a[id ='ReplyShowLink_a" + commentid + "']").text("回复");
            }
        }

        function JudgeZeroReply(commentid, targetuser)
        {
            var divList = document.getElementsByTagName('ReplyNum_div_Name' + commentid);
            if (divList[0].innerHTML.toString() == "0")
            {
                ShowReplyRegionZero('0', targetuser, commentid);
            }
            else
            {
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
        function ClickTag(index) {
            if (document.getElementById("MyCommentTag" + index.toString() + "_CheckBox") != null) {
                //可能执行JS后才改变Check的状态
                if (!document.getElementById("MyCommentTag" + index.toString() + "_CheckBox").checked) {
                    document.getElementById("MyCommentTag" + index.toString() + "_Label").className = "labelchecked";
                }
                else {
                    document.getElementById("MyCommentTag" + index.toString() + "_Label").className = "labeluncheck";
                }
            }
            else if (document.getElementById("MyCommentTagModify" + index.toString() + "_CheckBox") != null) {
                if (!document.getElementById("MyCommentTagModify" + index.toString() + "_CheckBox").checked) {
                    document.getElementById("MyCommentTagModify" + index.toString() + "_Label").className = "labelchecked";
                }
                else {
                    document.getElementById("MyCommentTagModify" + index.toString() + "_Label").className = "labeluncheck";
                }
            }
            else {
                if (!document.getElementById("Tag" + index.toString() + "_CheckBox").checked) {
                    document.getElementById("Tag" + index.toString() + "_Label").className = "labelchecked";
                }
                else {
                    document.getElementById("Tag" + index.toString() + "_Label").className = "labeluncheck";
                }
            }
        }

        function ShowAddTARegion() {
            if ($("div[id ='AddTA_div']").is(":hidden")) {
                $("div[id ='AddTA_div']").show();
            }
            else {
                $("div[id ='AddTA_div']").hide();
            }
        }

        function ShowIntroModifyRegion() {
            $("div[id ='ShowIntroModify_div']").show();
            $("div[id ='Intro']").hide();
        }

        function HideIntroModifyRegion() {
            $("div[id ='Intro']").show();
            $("div[id ='ShowIntroModify_div']").hide();
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
    <asp:TextBox class="TextBox0" ID="TargetUserID_TextBox" runat="server" style="display:none"></asp:TextBox><%--无视这一句,其他页面也是--%>
    <asp:TextBox class="TextBox0" ID="TargetReplyID_TextBox" runat="server" style="display:none"></asp:TextBox><%--无视这一句,其他页面也是--%>
    <%--<img id="Img1" src="/Imgs/ustctest.jpg" alt="USTC" />--%>
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
    
    <section id="CourseInfo">
        <%--下面这个div是课程信息区域--%>
        <br />
        <asp:ScriptManager ID="Course_ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="Course_UpdatePanel" runat="server" UpdateMode="Always"> 
        <ContentTemplate>
        <div id="CourseIntroduct">
            <h2><%=course.CourseName %></h2>
             
            教师：
             <asp:DataList ID="T_DataList" runat="server">
             <ItemTemplate>
                <a class="Username" href="/aspx/Information.aspx?UID=<%#Eval("UserID") %>"><%#Eval("UserName") %></a>
             </ItemTemplate>
             </asp:DataList>
             
             
            助教：
             <asp:DataList ID="A_DataList" runat="server">
             <ItemTemplate>
                 <a class="Username" href="/aspx/Information.aspx?UID=<%#Eval("UserID") %>"><%#Eval("UserName") %></a>
                 &nbsp
                  <%if (Session["UserInfo"] != null && user.isStudent == 0 && course.IsCourseTeacher() == 1)
                    { %>
                        <asp:Button  class="Button0" runat="server" Text="删除" CommandArgument='<%#Eval("UserID") %>' OnCommand="DeleteTA_Button_Click"/>
                  <%} %>
             </ItemTemplate>
             </asp:DataList>
            <%if (Session["UserInfo"] != null && user.isStudent == 0 && course.IsCourseTeacher() == 1)
                 { %>
                     <br />
                     <button class="Button0" type="button" onclick="ShowAddTARegion()">添加助教</button>
                     <div id="AddTA_div" style="display:none">
                         输入助教ID：<asp:TextBox ID="TAID" runat="server"></asp:TextBox>
                         <asp:Button class="Button0" runat="server" Text="添加" OnCommand="AddTA_Button_Click"/>
                     </div>
               <%} %>
            <br />
            <div id="Intro">课程简介：<pre><%=course.Introduction %></pre>
            <%if (Session["UserInfo"] != null && user.isStudent == 0 && course.IsCourseTeacher() == 1)
                 { %>
                     <button class="Button0" type="button" onclick="ShowIntroModifyRegion()">修改课程简介</button>
               <%} %>
            </div>
            <div id="ShowIntroModify_div" style="display:none">
                课程简介：<br />
               <asp:TextBox class="TextBox0" ID="Modify_Intro" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,200);" oninput=" MaxLength(this,200);"></asp:TextBox>
               <p  id="SubmitIntroModify">
                   <asp:Button  class="Button0" ID="Button1" runat="server" Text="确定修改" OnClick="ModifyIntro_Button_Click"/>
                   &nbsp<button class="Button0" type="button" onclick="HideIntroModifyRegion()">取消</button>
               &nbsp&nbsp&nbsp&nbsp
               </p>
            </div>
        </div>
        <%--<br />-------------------------------------------------------------------------<br />--%>
        <br />
        <%--学生页面--%>
        <%if (user.isStudent == 1 && course.IsCourseTeacher() == 0) 
          { %>
        <div id="MyComment">
              
              <%--<br />-------------------------------------------------------------------------<br />--%>
              <%--下面这个div是与我的评论相关的区域--%>
                  <h2>我的评论：</h2>
                  <%--如果用户没登录，会显示发表评论区域，但无法点击发表按钮--%>
                  <%if (Session["UserInfo"] == null)
                    { %>
                        <asp:TextBox class="TextBox0" ID="MyCommentDisable_TextBox" runat="server" onfocus="ClearDafaultText(this)" onblur="AddDefaultText(this)" style="color:#BEBEBE" Text="你还没有对此课程发表任何评论哦" TextMode="MultiLine"></asp:TextBox>
                        <br />
                        选择你的标签：
                        <asp:CheckBox ID="Tag1_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag1_Label" class="labeluncheck" for="Tag1_CheckBox" onclick="ClickTag('1')">课程简单</label>
                        <asp:CheckBox ID="Tag2_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag2_Label" class="labeluncheck" for="Tag2_CheckBox" onclick="ClickTag('2')">讲课清晰</label>
                        <asp:CheckBox ID="Tag3_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag3_Label" class="labeluncheck" for="Tag3_CheckBox" onclick="ClickTag('3')">解惑及时</label>
                        <asp:CheckBox ID="Tag4_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag4_Label" class="labeluncheck" for="Tag4_CheckBox" onclick="ClickTag('4')">作业量少</label>
                        <asp:CheckBox ID="Tag5_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag5_Label" class="labeluncheck" for="Tag5_CheckBox" onclick="ClickTag('5')">给分较好</label>
                        <asp:CheckBox ID="Tag6_CheckBox" CssClass="MyTag" runat="server" ></asp:CheckBox><label id="Tag6_Label" class="labeluncheck" for="Tag6_CheckBox" onclick="ClickTag('6')">收获较大</label>
                        <br />
                        <button type='button' disabled='disabled'>请登录后再查看或发表我的评论哦</button>
                  <%} %>

                  <%--用户已登录--%>
                  <%else
                    { %>  
                        <%--如果用户还未对这门课发表评论，if内是发表评论相关区域--%>
                        <%if (mycomment.Tables["CourseComment"].Rows.Count == 0)
                          { %>
                              <asp:TextBox class="TextBox0" ID="MyComment_TextBox" runat="server" onfocus="ClearDafaultText(this)" onblur="AddDefaultText(this)" style="color:#BEBEBE" Text="你还没有对此课程发表任何评论哦" TextMode="MultiLine" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                              <br />
                              选择你的标签：
                              <asp:CheckBox ID="MyCommentTag1_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag1_Label" class="labeluncheck" for="MyCommentTag1_CheckBox" onclick="ClickTag('1')">课程简单</label>
                              <asp:CheckBox ID="MyCommentTag2_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag2_Label" class="labeluncheck" for="MyCommentTag2_CheckBox" onclick="ClickTag('2')">讲课清晰</label>
                              <asp:CheckBox ID="MyCommentTag3_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag3_Label" class="labeluncheck" for="MyCommentTag3_CheckBox" onclick="ClickTag('3')">解惑及时</label>
                              <asp:CheckBox ID="MyCommentTag4_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag4_Label" class="labeluncheck" for="MyCommentTag4_CheckBox" onclick="ClickTag('4')">作业量少</label>
                              <asp:CheckBox ID="MyCommentTag5_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag5_Label" class="labeluncheck" for="MyCommentTag5_CheckBox" onclick="ClickTag('5')">给分较好</label>
                              <asp:CheckBox ID="MyCommentTag6_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTag6_Label" class="labeluncheck" for="MyCommentTag6_CheckBox" onclick="ClickTag('6')">收获较大</label>
                              <br />
                              <asp:Button  class="Button0" ID="MyComment_Button" runat="server" Text="立即发表评论" OnClick="MyComment_Button_Click"/>
                              <asp:Button  class="Button0" ID="MyAnonymousComment_Button" runat="server" Text="匿名发表评论" OnClick="MyAnonymousComment_Button_Click"/>
                        <%} %>

                        <%--如果用户以对这门课发表评论，else内是评论内容相关区域，修改评论按钮显示修改评论区域，取消回到评论内容区域--%>
                        <%else 
                          { %>
                              <%--评论内容区域--%>
                              <div id="ShowMyComment_div">

                                  <a class="Username" href="/aspx/Information.aspx?UID=<%=user.UserID.ToString()%>"><%=mycomment.Tables["CourseComment"].Rows[0]["UserName"] %></a>:
 
                                  <div>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[0] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">课程简单</span>
                                      <%}%>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[1] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">讲课清晰</span>
                                      <%}%>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[2] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">解惑及时</span>
                                      <%}%>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[3] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">作业量少</span>
                                      <%}%>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[4] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">给分较好</span>
                                      <%}%>
                                      <%if (((Byte[])mycomment.Tables["CourseComment"].Rows[0]["Tags"])[5] == (Byte)1) 
                                        { %>
                                            <span class="CommentTag">收获较大</span>
                                      <%}%>
                                  </div>
                                  <pre><%=mycomment.Tables["CourseComment"].Rows[0]["Content"] %></pre>
                                  <p class="CommentTime">
                                  发表时间：<%=mycomment.Tables["CourseComment"].Rows[0]["CreateDate"] %>
                                  <a id="MyReplyShowLink_a" href="javascript:void(0);" onclick="JudgeMyZeroReply('<%=mycomment.Tables["CourseComment"].Rows[0]["CommentID"] %>','<%=mycomment.Tables["CourseComment"].Rows[0]["UserID"] %>')">回复</a>(<div id="MyReplyNum_div" tagname="MyReplyNum_div_Name" runat="server" style="display:inline" ></div>)
                                  &nbsp<asp:Button class="Like_Button" runat="server" OnCommand="LikeMyComment_Button_Click"/>(<%=mycomment.Tables["CourseComment"].Rows[0]["LikeCounts"] %>)

                           </p>
                                   <div class="ReplyRegion" id="MyCommentReplyShowRegion_div" style="display:none">
                                      <asp:DataList ID="MyCommentReplyComment_DataList" runat="server">
                                         <ItemTemplate>
                                          &nbsp&nbsp&nbsp&nbsp<a class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a></span> 回复 <a class="Username" href='<%#int.Parse(Eval("TargetUserID").ToString())>9999? "/aspx/Information.aspx?UID=" + Eval("TargetUserID").ToString():"javascript:void(0);"%>'><%#Eval("TargetUserName") %></a></span>:<%#Eval("Content") %><asp:LinkButton runat="server" Text=" 删除这条回复" CommandArgument='<%#Eval("ReplyID") %>' OnCommand="Delete_CommentReply_LinkButton_Click" Visible='<%#int.Parse(Eval("RealUserID").ToString()) == user.UserID%>'/>
                                         <button class="Button0" type="button" onclick="ShowMyCommentReplyRegion('<%#Eval("ReplyID") %>','<%#Eval("UserID") %>','<%#Eval("CommentID") %>')" >回复</button>
                                         </ItemTemplate>
                                      </asp:DataList>
                                      <button class="Button0" type="button" onclick="ShowMyCommentReplyRegion('0','<%=mycomment.Tables["CourseComment"].Rows[0]["UserID"] %>','<%=mycomment.Tables["CourseComment"].Rows[0]["CommentID"] %>')">我也说一句</button></div>
                                       <div id="MyCommentReplyRegion_div" style="display:none">
                                      <asp:TextBox CssClass="TextBox0" ID="MyReplyComment_TextBox" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                                      <p  id="SubmitComment">
                                      <asp:Button runat="server" class="Button0"  Text="发表" OnCommand="ReplyMyComment_Button_Click"/>
                                      &nbsp<asp:Button runat="server" class="Button0"  Text="匿名发表" OnCommand="ReplyMyCommentAnonymous_Button_Click"/>
                                      &nbsp&nbsp&nbsp</p>
                                  </div>
                                  <br />
                                  <button class="Button0" type="button" onclick="ShowMyCommentModifyRegion()">修改评论</button>
                                  <asp:Button class="Button0" ID="DeleteMyComment_Button" runat="server" Text="删除评论" OnClick="DeleteMyComment_Button_Click"/>
                              </div>
                              
                              <%--修改评论区域--%>
                              <div id="ShowMyCommentModify_div" style="display:none">
                                  修改我的评论：<asp:TextBox class="TextBox0" ID="ModifyMyComment_TextBox" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                                  <br />
                                  修改我的标签：
                                  <asp:CheckBox ID="MyCommentTagModify1_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify1_Label" class="labeluncheck" for="MyCommentTagModify1_CheckBox" onclick="ClickTag('1')">课程简单</label>
                                  <asp:CheckBox ID="MyCommentTagModify2_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify2_Label" class="labeluncheck" for="MyCommentTagModify2_CheckBox" onclick="ClickTag('2')">讲课清晰</label>
                                  <asp:CheckBox ID="MyCommentTagModify3_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify3_Label" class="labeluncheck" for="MyCommentTagModify3_CheckBox" onclick="ClickTag('3')">解惑及时</label>
                                  <asp:CheckBox ID="MyCommentTagModify4_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify4_Label" class="labeluncheck" for="MyCommentTagModify4_CheckBox" onclick="ClickTag('4')">作业量少</label>
                                  <asp:CheckBox ID="MyCommentTagModify5_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify5_Label" class="labeluncheck" for="MyCommentTagModify5_CheckBox" onclick="ClickTag('5')">给分较好</label>
                                  <asp:CheckBox ID="MyCommentTagModify6_CheckBox" CssClass="MyTag" runat="server"></asp:CheckBox><label id="MyCommentTagModify6_Label" class="labeluncheck" for="MyCommentTagModify6_CheckBox" onclick="ClickTag('6')">收获较大</label>
                                  <br />
                                  <asp:Button  class="Button0" ID="ModifyMyComment_Button" runat="server" Text="确定修改" OnClick="ModifyMyComment_Button_Click"/>
                                  <button class="Button0" type="button" onclick="ShowMyCommentRegion()">取消</button>
                              </div>
                        <%} 
                    } %>
              <%--<br />-------------------------------------------------------------------------<br />--%>
            <%--下面这个div是链接到我的建议页面的按钮--%>
              <div>
                  <br />
                  <%if (Session["UserInfo"] == null)
                    { %>
                        <button type='button' disabled='disabled'>请登录后再查看或发表我的建议哦</button>
                  <%} %>
                  <%else
                    { %>
                        <a <%--type="button"--%> href="/aspx/MyAdvice.aspx?CourseID=<%=courseid %>">&nbsp 我的建议</a>
                  <%} %>
              </div>
            </div>

        <%} %>

        <%--助教页面--%>
        <%else if (course.IsCourseTeacher() == 1) 
          { 

              //助教页面若未登录直接跳回登录界面
              if (Session["UserInfo"] == null)
              {
                  Response.Redirect("/aspx/Login.aspx");
              } %>

              <%--链接到这门课的建议页面按钮--%>
              <div>
                  <button class="Button0" type="button" onclick="window.location='/aspx/CourseAdvice.aspx?CourseID=<%=courseid %>'">去看看所有建议</button>
              </div>   
              <%--<br />-------------------------------------------------------------------------<br />--%>   
        <%} %>
        <br />
        <%--下面的div是列出这门课的所有评论，使用DataList控件，当用户未登录时，赞和回复控件失效，只能查看评论和回复--%>
        <div id="AllComments">
            <h2>所有评论：</h2>
            <%--下面<asp:DataList>....</asp:DataList>是第一层DataList，每一条数据是一条评论--%>
            <asp:DataList ID="AllComment_DataList" runat="server" OnItemDataBound="AllComment_DataList_ItemDataBound">
                <%-- 每条数据<ItemTemplate>...</ItemTemplate>--%>
                <ItemTemplate>
                    <div class="OneComment">
                    <a class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a>: 
                    <div>
                    <%#((Byte[])Eval("Tags"))[0] == (Byte)1? "<span class='CommentTag'>课程简单</span>":"" %>
                    <%#((Byte[])Eval("Tags"))[1] == (Byte)1? "<span class='CommentTag'>讲课清晰</span>":"" %>
                    <%#((Byte[])Eval("Tags"))[2] == (Byte)1? "<span class='CommentTag'>解惑及时</span>":"" %>
                    <%#((Byte[])Eval("Tags"))[3] == (Byte)1? "<span class='CommentTag'>作业量少</span>":"" %>
                    <%#((Byte[])Eval("Tags"))[4] == (Byte)1? "<span class='CommentTag'>给分较好</span>":"" %>
                    <%#((Byte[])Eval("Tags"))[5] == (Byte)1? "<span class='CommentTag'>收获较大</span>":"" %>      
                    </div>
                    <pre><%#Eval("Content") %></pre>
                    <p class="CommentTime">
                    发表时间<%#Eval("CreateDate") %>
                    <a id="ReplyShowLink_a<%#Eval("CommentID") %>" href="javascript:void(0);" onclick="JudgeZeroReply('<%#Eval("CommentID") %>','<%#Eval("UserID") %>')">回复</a>(<div id="ReplyNum_div" tagname="ReplyNum_div_Name" runat="server" style="display:inline" ></div>)
                     <%if (Session["UserInfo"] != null)
                      { %>
                          <%--<img id="Like_Img" src="../Imgs/zan.png" alt="赞"  OnClick="LikeComment_Button_Click(<%#Eval("CommentID") %>)"/>--%>
                          <asp:Button class="Like_Button" runat="server" CommandArgument='<%#Eval("CommentID") %>' OnCommand="LikeComment_Button_Click"/>
                    <%}
                      else 
                      {%>
                          <img id="Like" src="../Imgs/zan.png" alt="赞"  />
                    <%} %>
                    (<%#Eval("LikeCounts") %>)
                   </p>
                    <%--下面这个div是嵌套的内层DataList，每条数据是上层一条评论的所有回复,默认隐藏--%>
                    <div class="ReplyRegion" id="ReplyShowRegion_div<%#Eval("CommentID") %>" style="display:none">
                        <%--下面<asp:DataList>....</asp:DataList>是第二层DataList，每一条数据是一条评论的回复--%>
                        <asp:DataList ID="ReplyComment_DataList" runat="server">
                            <%-- 每条数据<ItemTemplate>...</ItemTemplate>--%>
                         <ItemTemplate>
                            &nbsp&nbsp&nbsp&nbsp<a class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a></span> 回复 <a class="Username" href='<%#int.Parse(Eval("TargetUserID").ToString())>9999? "/aspx/Information.aspx?UID=" + Eval("TargetUserID").ToString():"javascript:void(0);"%>'><%#Eval("TargetUserName") %></a></span>:<%#Eval("Content") %><%if (Session["UserInfo"] != null)
                              { %><%--每条回复后面，如果这条回复是我发的就可以删除掉--%><asp:LinkButton runat="server" Text=" 删除这条回复" CommandArgument='<%#Eval("ReplyID") %>' OnCommand="Delete_CommentReply_LinkButton_Click" Visible='<%#int.Parse(Eval("RealUserID").ToString()) == user.UserID%>'/>
                                  <%--每条回复后面，回复这条回复的按钮，实际上这条语句只是显示了下面的评论区输入框和提交按钮--%>
                                  <button class="Button0" type="button" onclick="ShowReplyRegion('<%#Eval("ReplyID") %>','<%#Eval("UserID") %>','<%#Eval("CommentID") %>')" >回复</button>
                            <%} %>
                            </ItemTemplate>
                        </asp:DataList>

                        <%--每条评论后面，回复这条评论的按钮，实际上这条语句只是显示了下面的评论区输入框和提交按钮（与上面回复后面显示的是同一个区域）--%>
                        <%if (Session["UserInfo"] != null)
                          { %>
                              <div><button class="Button0" type="button" onclick="ShowReplyRegion('0','<%#Eval("UserID") %>','<%#Eval("CommentID") %>')">我也说一句</button></div>
                        <%} %>
                        </div>
                    <%--默认隐藏的评论区输入框和提交按钮--%>
                    <div id="ReplyRegion_div<%#Eval("CommentID") %>" style="display:none">
                        <%if (Session["UserInfo"] != null)
                          { %>
                              <asp:TextBox class="TextBox0" ID="Reply_TextBox" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                              <p  id="SubmitComment">
                              <asp:Button  class="Button0" runat="server" Text="发表" CommandArgument='<%#Eval("CommentID") %>' OnCommand="ReplyComment_Button_Click"/>
                              &nbsp<asp:Button  class="Button0" runat="server" Text="匿名发表" CommandArgument='<%#Eval("CommentID") %>' OnCommand="ReplyCommentAnonymous_Button_Click"/>
                              &nbsp&nbsp&nbsp</p>
                        <%} %>
                    </div>
                    
                    <%--<br />-------------------------------------------------------------------------<br />--%>
                </div>
                </ItemTemplate>
            </asp:DataList>
            </ContentTemplate>
            </asp:UpdatePanel>
            <%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
            <webdiyer:AspNetPager CssClass="pagination" PagingButtonSpacing="0" CurrentPageButtonClass="active"
                 ID="AspNetPager" runat="server" AlwaysShow="true"  
                PageSize="6" onpagechanging="AspNetPager_PageChanged" FirstPageText="<<"  
                LastPageText=">>" NextPageText=">" PrevPageText="<" UrlPaging="True" 
                ShowFirstLast="true" ShowPageIndexBox="Never"  EnableUrlRewriting="True" UrlRewritePattern="/aspx/Course.aspx?CourseID=%CourseID%&Page={0}"></webdiyer:AspNetPager>
        </div>
        <br /><br /><br /><br />

    </section>
</body>
    
</html>
</form>


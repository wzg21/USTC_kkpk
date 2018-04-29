<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseAdvice.aspx.cs" Inherits="TimelyEvaluation.aspx.CourseAdvice" %>

<form id="form1" runat="server">

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/Public.css" >
<link rel="stylesheet" type="text/css" href="/css/Course.css" >
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_课程建议</title>

    <script src="/scripts/jquery-2.1.1/jquery.min.js"></script>
    <script type="text/javascript">
        function Exit()
        {
            if (confirm("确定要退出吗？"))
            {
                window.location = "/aspx/Exit.aspx";
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

        function ShowReplyShowRegion(adviceid)
        {
            $("div[id ='ReplyRegion_div" + adviceid + "']").hide();
            if ($("div[id ='ReplyShowRegion_div" + adviceid + "']").is(":hidden"))
            {
                $("div[id ='ReplyShowRegion_div" + adviceid + "']").show();
                $("a[id ='ReplyShowLink_a" + adviceid + "']").text("收起回复");
            }
            else
            {
                $("div[id ='ReplyShowRegion_div" + adviceid + "']").hide();
                $("a[id ='ReplyShowLink_a" + adviceid + "']").text("回复");
            }
        }
        function ShowReplyRegion(targetuser, adviceid)
        {
            $("#TargetUserID_TextBox").val(targetuser);

            $("div[id ='ReplyRegion_div" + adviceid + "']").show();
        }

        function ShowReplyRegionZero(targetuser, adviceid) {
            $("#TargetUserID_TextBox").val(targetuser);
            if ($("div[id ='ReplyRegion_div" + adviceid + "']").is(":hidden")) {
                $("div[id ='ReplyRegion_div" + adviceid + "']").show();
                $("a[id ='ReplyShowLink_a" + adviceid + "']").text("收起回复");
            }
            else {
                $("div[id ='ReplyRegion_div" + adviceid + "']").hide();
                $("a[id ='ReplyShowLink_a" + adviceid + "']").text("回复");
            }
        }

        function JudgeZeroReply(adviceid, targetuser)
        {
            var divList = document.getElementsByTagName('ReplyNum_div_Name' + adviceid);
            if (divList[0].innerHTML.toString() == "0") {
                ShowReplyRegionZero(targetuser, adviceid);
            }
            else {
                ShowReplyShowRegion(adviceid);
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
    <asp:TextBox ID="TargetUserID_TextBox" runat="server" style="display:none"></asp:TextBox>

    <h1 >科科评科</h1>
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
         
    <section id="MyadviceInfo">
        <br />
        <asp:ScriptManager ID="CourseAdvice_ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="CourseAdvice_UpdatePanel" runat="server" UpdateMode="Always"> 
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
                  <%if (Session["UserInfo"] != null && user.isStudent == 0)
                    { %>
                        <asp:Button  class="Button0" runat="server" Text="删除" CommandArgument='<%#Eval("UserID") %>' OnCommand="DeleteTA_Button_Click"/>
                  <%} %>
                 
                 </ItemTemplate>
             </asp:DataList>
            <%if (Session["UserInfo"] != null && user.isStudent == 0)
                 { %>
                     <button class="Button0" type="button" onclick="ShowAddTARegion()">添加助教</button>
                     <div id="AddTA_div" style="display:none">
                         <br />输入助教ID：<asp:TextBox ID="TAID" runat="server"></asp:TextBox>
                         <asp:Button class="Button0" runat="server" Text="添加" OnCommand="AddTA_Button_Click"/>
                     </div>
               <%} %>
            <br />
            <div id="Intro">课程简介：<br /><div class="OneComment"><%=course.Introduction %></div>
               
            <%if (Session["UserInfo"] != null && user.isStudent == 0)
                 { %>
                     <p  id="SubmitComment">
                     <button class="Button0" type="button" onclick="ShowIntroModifyRegion()">修改课程简介</button></p>
               <%} %>
            </div>
            <div id="ShowIntroModify_div" style="display:none">
                课程简介：<br /> <br />
               <asp:TextBox class="TextBox0" ID="Modify_Intro" runat="server" TextMode="MultiLine" onpropertychange=" MaxLength(this,200);" oninput=" MaxLength(this,200);"></asp:TextBox>
               <p  id="SubmitComment">
               <asp:Button  class="Button0" ID="Button1" runat="server" Text="确定修改" OnClick="ModifyIntro_Button_Click"/>
               <button class="Button0" type="button" onclick="HideIntroModifyRegion()">取消</button>
               </p>
            </div>
        </div>
        
        <br />

        <div id="AllCourseIntroduct">
            <h2>所有建议：</h2><br />
            <asp:DataList ID="AllAdvice_DataList" runat="server" OnItemDataBound="AllAdvice_DataList_ItemDataBound">

                <ItemTemplate>
                    <div class="OneComment">
                    <a  class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a>:
                    <pre><%#Eval("Content") %></pre>
                    <br />
                    <p class="CommentTime">发表时间<%#Eval("CreateDate") %>
                    <a id="ReplyShowLink_a<%#Eval("AdviceID") %>" href="javascript:void(0);" onclick="JudgeZeroReply('<%#Eval("AdviceID") %>','<%#Eval("UserID") %>')">回复</a>
                    (<div id="ReplyNum_div" tagname="ReplyNum_div_Name" runat="server" style="display:inline" ></div>)
                   </p>
                     <div id="ReplyShowRegion_div<%#Eval("AdviceID") %>" style="display:none">

                        <asp:DataList ID="ReplyAdvice_DataList" runat="server">
                            <ItemTemplate>
                            &nbsp&nbsp&nbsp&nbsp<a class="Username" class="Username" href='<%#Eval("UserID").ToString()==Eval("RealUserID").ToString()? "/aspx/Information.aspx?UID=" + Eval("UserID").ToString():"javascript:void(0);"%>'><%#Eval("UserName") %></a></span> 回复 <span class="CommentName"><a class="Username" href='<%#int.Parse(Eval("TargetUserID").ToString())>9999? "/aspx/Information.aspx?UID=" + Eval("TargetUserID").ToString():"javascript:void(0);"%>'><%#Eval("TargetUserName") %></a>:<%#Eval("Content")%>
                            <asp:LinkButton runat="server" Text="删除这条回复" CommandArgument='<%#Eval("ReplyID") %>' OnCommand="Delete_AdviceReply_LinkButton_Click" Visible='<%#int.Parse(Eval("RealUserID").ToString()) == user.UserID%>'/>
                            <button class="Button0" type="button" onclick="ShowReplyRegion('<%#Eval("UserID") %>','<%#Eval("AdviceID") %>')" >回复</button>
                            </ItemTemplate>
                        </asp:DataList>

                        <div><button  class="Button0" type="button" onclick="ShowReplyRegion('<%#Eval("UserID") %>','<%#Eval("AdviceID") %>')">我也说一句</button></div>
                    </div>

                    <div id="ReplyRegion_div<%#Eval("AdviceID") %>" style="display:none">
                        <asp:TextBox class="TextBox0" TextMode="MultiLine" ID="Reply_TextBox" runat="server" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
                        <p  id="SubmitComment">
                        <asp:Button  class="Button0" runat="server" Text="发表" CommandArgument='<%#Eval("AdviceID") %>' OnCommand="ReplyAdvice_Button_Click"/>
                        </p>
                    </div>
                    </div>
                    <br />
                </ItemTemplate>
            </asp:DataList>
            </ContentTemplate>
            </asp:UpdatePanel>
        <%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
            <webdiyer:AspNetPager CssClass="pagination"  PagingButtonSpacing="0" CurrentPageButtonClass="active"
                ID="AspNetPager" runat="server" AlwaysShow="true"  
                PageSize="6" onpagechanging="AspNetPager_PageChanged" FirstPageText="<<"  
                LastPageText=">>" NextPageText=">" PrevPageText="<" UrlPaging="True" 
                ShowFirstLast="true" ShowPageIndexBox="Never"  EnableUrlRewriting="True" UrlRewritePattern="/aspx/MyAdvice.aspx?CourseID=%CourseID%&Page={0}"></webdiyer:AspNetPager>
        </div> 
        <br /><br /><br /><br />

    </section>
</body>
    
</html>
</form>

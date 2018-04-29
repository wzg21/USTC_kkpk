﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseList.aspx.cs" Inherits="TimelyEvaluation.aspx.CourseList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/Public.css"/>
<link rel="stylesheet" type="text/css" href="/css/CourseList.css"/>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_首页</title>
    <script type="text/javascript">
        function Exit()
        {
            if (confirm("确定要退出吗？"))
            {
                window.location="/aspx/Exit.aspx";
            }
        }

        function AddDefaultText(obj)
        {
            if (obj.value == "")
            {
                obj.value = "请输入课程名";
                document.getElementById("CourseNameTextBox").style = "color:#BEBEBE";//修改字体颜色  
            }
        }
        function ClearDafaultText(obj)
        {
            if (obj.value == "请输入课程名")
            {
                obj.value = "";
                document.getElementById("CourseNameTextBox").style = "color:#000000";
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
    </script>
</head>

<body>
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

    <form id="List" runat="server">
        课程名：<asp:TextBox ID="CourseName_TextBox" runat="server"></asp:TextBox>
        课程号：<asp:TextBox ID="CourseNum_TextBox" runat="server"></asp:TextBox>
        教师姓名：<asp:TextBox ID="Teacher_TextBox" runat="server"></asp:TextBox>
        
        <asp:Button CssClass="Button0" ID="SearchCourse_Button" runat="server" Text="搜索课程" OnClick="SearchCourse_Button_Click"/>

    <section id="ListRegion">
        <asp:ScriptManager ID="CourseList_ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="CourseList_UpdatePanel" runat="server" UpdateMode="Always"> 
        <ContentTemplate> 
        <asp:DataList ID="CourseList_DataList" runat="server">
            <HeaderTemplate>
                <table class="TableHead">
                    <tr>
                        <td>课程名</td>
                        <td>课程号</td>
                        <td>教师姓名</td>
                        <td>上课时间地点</td>
                        <td>操作</td>
                    </tr>
                </table>
            </HeaderTemplate>
            <ItemTemplate>
                <table class="InDataList"  style='<%#int.Parse(Eval("isChosen").ToString())==0? " background-color:#ffffff;width:100%;":" background-color:rgba(224, 224, 224, 0.45);width:100%;"%>'>
                <tr>
                <td><a href="/aspx/Course.aspx?CourseID=<%#Eval("CourseID") %>"><%#Eval("CourseName") %></a></td> <%--列出课程信息，超链接 --%>
                <td><%#Eval("CourseNumber") %></td>
                <td><%#Eval("TeachersName") %></td>
                <td><%#Eval("CourseTime") %></td>
                <td><%if (Session["UserInfo"] != null && user.isStudent == 1)
                     { %>
                        <asp:Button CssClass="Button0" runat="server" Text='<%#int.Parse(Eval("isChosen").ToString())==0?"添加课程":"删除课程" %>' CommandArgument='<%#Eval("CourseID") + "," +Eval("isChosen") %>' OnCommand="AddDropCourse_Button_Click"/>
                   <%} %></td>
                </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
        </ContentTemplate>
        </asp:UpdatePanel>
        <%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
            <webdiyer:AspNetPager CssClass="pagination" PagingButtonSpacing="0" CurrentPageButtonClass="active"
                 ID="AspNetPager" runat="server" AlwaysShow="true"  
                PageSize="20" onpagechanging="AspNetPager_PageChanged" FirstPageText="<<"  
                LastPageText=">>" NextPageText=">" PrevPageText="<" UrlPaging="True" 
                ShowFirstLast="true" ShowPageIndexBox="Never"  EnableUrlRewriting="True" UrlRewritePattern="/aspx/CourseList.aspx?CNA=%CNA%&CNU=%CNU%&T=%T%&Page={0}"></webdiyer:AspNetPager>
    </section>
        <br /><br /><br /><br /><br />
</form>
</body>
    
</html>

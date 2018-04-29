<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyCourseList.aspx.cs" Inherits="TimelyEvaluation.aspx.MyCourseList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/Public.css" >
<link rel="stylesheet" type="text/css" href="/css/CourseList.css" >
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_我的课程列表</title>

    <script type="text/javascript">
        function Exit() {
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

    </script>

</head>

<form id="form1" runat="server">
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

    <%--下面这个section是与我相关的课程列表，我只写了<%#Eval("CourseName") %>这个属性--%>
    <section id="MyCourseInfo">
        <%--学生页面--%>
        <br />
        <asp:ScriptManager ID="MyCourseList_ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="MyCourseList_UpdatePanel" runat="server" UpdateMode="Always"> 
        <ContentTemplate> 
        <%if (opertype == "AsStudent")
          { %>
              <div class="MyadviceIntroduct">
              <h2>我正在修的课：</h2>
              <asp:DataList ID="MyStudyingCourseList_DataList" runat="server" OnItemDataBound="CourseList_DataList_ItemDataBound">
                  <HeaderTemplate>
                    <table class="TableHead">
                    <tr>
                        <td>课程名</td>
                        <td>课程号</td>
                        <td>上课时间地点</td>
                        <td>教师</td>
                        <td>助教</td>
                        <td>操作</td>
                    </tr>
                    </table>
                  </HeaderTemplate>
                  <ItemTemplate>
                      <table class="InDataList">
                        <tr>
                          <td><a href="/aspx/Course.aspx?CourseID=<%#Eval("CourseID") %>"><%#Eval("CourseName") %></a></td>
                          <td><%#Eval("CourseNumber") %></td>
                          <td><%#Eval("CourseTime") %></td>
                          <td>
                              <asp:DataList ID="Teacher_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                              </asp:DataList>
                              <div id="No_t" runat="server"></div>
                          </td>
                          <td>
                          <asp:DataList ID="TA_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                          </asp:DataList>
                          <div id="No_ta" runat="server"></div>
                          </td>
                          <td>
                            <asp:Button class="Button0" runat="server" Text="删除课程" CommandArgument='<%#Eval("CourseID") %>' OnCommand="DropCourse_Button_Click"/>
                          </td>
                      </tr>  
                      </table>
                  </ItemTemplate>
              </asp:DataList>
              </div>
              <%--<br />-------------------------------------------------------------------------<br />--%>
              <br />
              <div class="MyadviceIntroduct">
              <h2>我已修过的课：</h2>
              <asp:DataList ID="MyStudiedCourseList_DataList" runat="server" OnItemDataBound="CourseList_DataList_ItemDataBound">
                  <HeaderTemplate>
                    <table class="TableHead">
                    <tr>
                        <td>课程名</td>
                        <td>课程号</td>
                        <td>上课时间地点</td>
                        <td>教师</td>
                        <td>助教</td>
                        <td>操作</td>
                    </tr>
                    </table>
                  </HeaderTemplate>
                  <ItemTemplate>
                      <table class="InDataList">
                        <tr>
                          <td><a href="/aspx/Course.aspx?CourseID=<%#Eval("CourseID") %>"><%#Eval("CourseName") %></a></td>
                          <td><%#Eval("CourseNumber") %></td>
                          <td><%#Eval("CourseTime") %></td>
                          <td>
                              <asp:DataList ID="Teacher_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                              </asp:DataList>
                              <div id="No_t" runat="server"></div>
                          </td>
                          <td>
                          <asp:DataList ID="TA_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                          </asp:DataList>
                          <div id="No_ta" runat="server"></div>
                          </td>
                          <td>
                            <asp:Button class="Button0" runat="server" Text="删除课程" CommandArgument='<%#Eval("CourseID") %>' OnCommand="DropCourse_Button_Click"/>
                          </td>
                      </tr>  
                      </table>
                  </ItemTemplate>
              </asp:DataList>
              </div>
              <%--<br />-------------------------------------------------------------------------<br />--%>
              <br />
        <%} %>
       
        <%--助教页面--%>
        <%else if (opertype == "AsTA")
          { %>
              <div class="MyadviceIntroduct">
              <h2>我正在带的课：</h2>
              <asp:DataList ID="MyTeachingCourseList_DataList" runat="server" OnItemDataBound="CourseList_DataList_ItemDataBound">
                    <HeaderTemplate>
                    <table class="TableHead">
                    <tr>
                        <td>课程名</td>
                        <td>课程号</td>
                        <td>上课时间地点</td>
                        <td>教师</td>
                        <td>助教</td>
                        <td>操作</td>
                    </tr>
                    </table>
                  </HeaderTemplate>
                  <ItemTemplate>
                      <table class="InDataList">
                        <tr>
                          <td><a href="/aspx/Course.aspx?CourseID=<%#Eval("CourseID") %>"><%#Eval("CourseName") %></a></td>
                          <td><%#Eval("CourseNumber") %></td>
                          <td><%#Eval("CourseTime") %></td>
                          <td>
                              <asp:DataList ID="Teacher_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                              </asp:DataList>
                              <div id="No_t" runat="server"></div>
                          </td>
                          <td>
                          <asp:DataList ID="TA_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                          </asp:DataList>
                          <div id="No_ta" runat="server"></div>
                          </td>
                          <td>
                            <asp:Button class="Button0" runat="server" Text="删除课程" CommandArgument='<%#Eval("CourseID") %>' OnCommand="DropCourse_Button_Click"/>
                          </td>
                      </tr>  
                      </table>
                  </ItemTemplate>
              </asp:DataList>
              </div>
<%--              <br />-------------------------------------------------------------------------<br />--%>
              <br />
              <div class="MyadviceIntroduct">
              <h2>我已带过的课：</h2>
              <asp:DataList ID="MyTaughtCourseList_DataList" runat="server" OnItemDataBound="CourseList_DataList_ItemDataBound">
                                      <HeaderTemplate>
                    <table class="TableHead">
                    <tr>
                        <td>课程名</td>
                        <td>课程号</td>
                        <td>上课时间地点</td>
                        <td>教师</td>
                        <td>助教</td>
                        <td>操作</td>
                    </tr>
                    </table>
                  </HeaderTemplate>
                  <ItemTemplate>
                      <table class="InDataList">
                        <tr>
                          <td><a href="/aspx/Course.aspx?CourseID=<%#Eval("CourseID") %>"><%#Eval("CourseName") %></a></td>
                          <td><%#Eval("CourseNumber") %></td>
                          <td><%#Eval("CourseTime") %></td>
                          <td>
                              <asp:DataList ID="Teacher_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                              </asp:DataList>
                              <div id="No_t" runat="server"></div>
                          </td>
                          <td>
                          <asp:DataList ID="TA_DataList" runat="server">
                              <ItemTemplate>
                                  <%#Eval("UserName") %>
                              </ItemTemplate>
                          </asp:DataList>
                          <div id="No_ta" runat="server"></div>
                          </td>
                          <td>
                            <asp:Button class="Button0" runat="server" Text="删除课程" CommandArgument='<%#Eval("CourseID") %>' OnCommand="DropCourse_Button_Click"/>
                          </td>
                      </tr>  
                      </table>
                  </ItemTemplate>
              </asp:DataList>
              </div >
        <%} %>
        </ContentTemplate>
        </asp:UpdatePanel>
         <br /> <br /> <br /> <br />
    </section>
</body>
</form>
</html>

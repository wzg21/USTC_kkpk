<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Information.aspx.cs" Inherits="TimelyEvaluation.aspx.Information" %>


<!DOCTYPE html>
<!--本页面用于查看或编辑个人信息，如果请求访问者为信息所属人，则可以进行修改，若为他人信息，则仅能进行查看-->

<!--未完成：给MyUsername_TextBox，MyPersonalProfile_TextBox加JS使其中具有默认文字-->
<%

%>

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="/css/Public.css" >
    <link rel="stylesheet" type="text/css" href="/css/Information.css" >
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科科评科_个人信息</title>

    <script src="/scripts/jquery-2.1.1/jquery.min.js"></script>
    <script type="text/javascript">

        function Exit() {
            if (confirm("确定要退出吗？")) {
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
        function ShowModifyMyPassword() {
            document.getElementById("ModifyMyPassword_div").style.display = 'block';
            document.getElementById("JS_CheckPasswordConfirm_div").style.display = 'block';
            document.getElementById("ModifyMyPassword_button_div").style.display = 'block';
            document.getElementById("ModifyMyInformation_div").style.display = 'none';
            document.getElementById("MyInformation_div").style.display = 'none';
        }
        function ShowModifyMyInformation() {
            document.getElementById("ModifyMyPassword_div").style.display = 'none';
            document.getElementById("JS_CheckPasswordConfirm_div").style.display = 'none';
            document.getElementById("ModifyMyPassword_button_div").style.display = 'none';
            document.getElementById("ModifyMyInformation_div").style.display = 'block';
            document.getElementById("MyInformation_div").style.display = 'none';
        }
        function CancelModify() {
            document.getElementById("ModifyMyPassword_div").style.display = 'none';
            document.getElementById("JS_CheckPasswordConfirm_div").style.display = 'none';
            document.getElementById("ModifyMyPassword_button_div").style.display = 'none';
            document.getElementById("ModifyMyInformation_div").style.display = 'none';
            document.getElementById("MyInformation_div").style.display = 'block';
        }

        ///////////////
        function MyUsername_AddDefaultText(obj) {
            if (obj.value == "") {
                obj.value = "点击编辑自己的昵称";
                if (document.getElementById("MyUsername_TextBox") != null)
                    document.getElementById("MyUsername_TextBox").style.cssText = "color:#BEBEBE";

            }
        }

        function MyUsername_ClearDafaultText(obj) {
            if (obj.value == "点击编辑自己的昵称") {
                obj.value = "";
                if (document.getElementById("MyUsername_TextBox") != null)
                    document.getElementById("MyUsername_TextBox").style.cssText = "color:#000000";
                if (document.getElementById("MyUsername0_TextBox") != null)
                    document.getElementById("MyUsername0_TextBox").style.cssText = "color:#000000";
            }

        }

        function MyPersonalProfile_AddDefaultText(obj) {
            if (obj.value == "") {
                obj.value = "点击编辑自己的个人签名";
                if (document.getElementById("MyPersonalProfile_TextBox") != null)
                    document.getElementById("MyPersonalProfile_TextBox").style.cssText = "color:#BEBEBE";

            }
        }

        function MyPersonalProfile_ClearDafaultText(obj) {
            if (obj.value == "点击编辑自己的个人签名") {
                obj.value = "";
                if (document.getElementById("MyPersonalProfile_TextBox") != null)
                    document.getElementById("MyPersonalProfile_TextBox").style.cssText = "color:#000000";
                if (document.getElementById("MyPersonalProfile0_TextBox") != null)
                    document.getElementById("MyPersonalProfile0_TextBox").style.cssText = "color:#000000";
            }

        }
        ////////////////

        function CheckPasswordConfirm() {
            var password = document.getElementById("ModifyMyPassword_NewPassword_Textbox").value;
            var password_confirm = document.getElementById("ModifyMyPassword_ComfirmPassword_Textbox").value;
            if (password == "") {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "请输入密码!";
                return false;
            }
            else if (password_confirm == "") {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "请输入确认密码!";
                return false;
            }
            else if (password != password_confirm) {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "两次输入不一致!";
                return false;
            }
            else {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "";
                return true;
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

    <style type="text/css">
        #form1 {
            height: 93px;
        }
    </style>
</head>
<body style="height: 100%">
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
                  <li class="li-Guide">
                          <div class="GuideUser">
                              <%=user.UserName %> 
                          </div>

                          <li class="li-Guide"><a href="/aspx/CourseList.aspx">首页</a></li>
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
                          <li><a id="TATag" <%--href=""--%> class="folder" onclick="ShowGuide(TATag,HideTATag)">教师▿</li>
                              <ul id="HideTATag">
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

    <% //以下为信息显示部分 %>
    <form id="form1" runat="server">

        <%if (user.UserID == uid)   //在查看自己信息，查看信息者是信息所有者，可以修改信息
          {  %>
            <br />
            <div id="MyInformation_div">
            <h4><b>我的个人信息</b></h4>
            
            

            <% if(user.isStudent == 0) {%>
            <h3>教师姓名：<%=user.UserName%></h3>
            
            <%} %>

            <% else if (user.isStudent == 1)
               { %>
                <h3>昵&ensp;&ensp;&ensp;&ensp;称：<%if (user.UserName == ""){%><%=default_MyUsername %>
                 <%} %>
                 <%else{ %> 
                 <%=user.UserName%> </h3>
                 <%} %>
            
            <%} %>

            <%else {%>
            ERROR! ----IsStudent取值错误
            <%} %>
            
            <h3>邮&ensp;&ensp;&ensp;&ensp;箱：<%=user.Email %></h3>
            <h3>个人签名：
            <%if (user.PersonalProfile == ""){%>
            <%=default_MyPersonalProfile %>
            <%} %>
            <%else
          { %>
            <% =user.PersonalProfile%><br />
            <%} %>
            </h3>
            <br />
            <p  id="SubmitComment">
            <button class="Button0" type="button" onclick="ShowModifyMyInformation()">修改信息</button>
            <button class="Button0" type="button" onclick="ShowModifyMyPassword()">修改密码</button>
            </p>
        </div>
       
<div id="ModifyMyInformation_div" style="display:none">
            <h5>信息修改</h5>
            <% if (user.isStudent == 0)
               {%>
            <h6>教师姓名：
            <%=user.UserName%></h6>
            <%//教师姓名设置为不能更改 %>
            <%} %>

            <% else if (user.isStudent == 1)
               { %>
           
            <h6>昵&ensp;&ensp;&ensp;&ensp;称：</h6>
            <%if (user.UserName == default_MyUsername)
              {%>
            <asp:TextBox TextMode="MultiLine" ID="MyUsername0_TextBox" class="TextBox0" runat="server" Text="点击编辑自己的昵称" style="color:#BEBEBE" onfocus="MyUsername_ClearDafaultText(this)" onblur="MyUsername_AddDefaultText(this)" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
            <%} %>
            <%else{ %>
            <asp:TextBox TextMode="MultiLine" ID="MyUsername_TextBox" class="TextBox0" runat="server" onfocus="MyUsername_ClearDafaultText(this)" onblur="MyUsername_AddDefaultText(this)" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>                 
            <%} %>
            <%} %>

            <%else {%>
            ERROR! ----IsStudent取值错误
            <%} %>
            
            <h6>邮&ensp;&ensp;&ensp;&ensp;箱：
            <%=user.Email %></h6>
            <% //Email设置为不能更改 %>
            <h6>个人签名：</h6>
            <%if (user.PersonalProfile == default_MyPersonalProfile)
              {%>
            <asp:TextBox TextMode="MultiLine" ID="MyPersonalProfile0_TextBox" class="TextBox0" Text="点击编辑自己的个人签名" runat="server" style="color:#BEBEBE" onfocus="MyPersonalProfile_ClearDafaultText(this)" onblur="MyPersonalProfile_AddDefaultText(this)" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
            <%} %>
            <%else{ %>
            <asp:TextBox TextMode="MultiLine" ID="MyPersonalProfile_TextBox" class="TextBox0" runat="server" onfocus="MyPersonalProfile_ClearDafaultText(this)" onblur="MyPersonalProfile_AddDefaultText(this)" onpropertychange=" MaxLength(this,1000);" oninput=" MaxLength(this,1000);"></asp:TextBox>
            <%} %>
            <br />
            <br />
            <asp:Button class="Button0" ID="ModifyMyInformation_Button" runat="server" Text="提交修改" OnClick="ModifyMyInfomation_Button_Click"  />
            <button  class="Button0" type="button" onclick="CancelModify()">取消</button>
            <br />
            <br />
         </div>

        <div id="ModifyMyPassword_div" style="display:none">
        <h5>密码修改</h5>
        <h6>原&ensp;&ensp;密&ensp;&ensp;码：
            <asp:TextBox  ID="ModifyMyPassword_OldPassword_Textbox" runat="server" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox></h6>
        <h6>新&ensp;&ensp;密&ensp;&ensp;码：
            <asp:TextBox  ID="ModifyMyPassword_NewPassword_Textbox" runat="server"  onblur="CheckPasswordConfirm()" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox></h6>
        <h6>确认新密码：
            <asp:TextBox ID="ModifyMyPassword_ComfirmPassword_Textbox" runat="server"  onblur="CheckPasswordConfirm()" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox></h6>
        </div>
        
        <div id="JS_CheckPasswordConfirm_div" style="display:none"> </div>
        
        <div id="ModifyMyPassword_button_div" style="display:none">
        <br />
        <asp:Button class="Button0" ID="Button1" runat="server" Text="提交修改密码" OnClick="ModifyMyPassword_Button_Click"/>
        <button class="Button0" type="button" onclick="CancelModify()">取消</button>
        <br /><br />
        
        
       </div>
        <%} //end查看自己的信息 %>

        <%else//在查看别人信息，查看信息者不是信息所有者owner，不可以修改
          { %>
        <br />
        <div id="OtherInformation_div">

            <h4><% if (owner.UserName == "")
                   {%>Ta<%} %>
                <%else
                   {%>  <%=owner.UserName%>  <%} %>的个人信息</h4>
            <% if (owner.isStudent == 0)
               {%>
            <h3>教师姓名：<%=owner.UserName%></h3>
            <%} %>

            <% else if (owner.isStudent == 1)
               { %>
                <h3>昵&ensp;&ensp;&ensp;&ensp;称：
                 <%if (owner.UserName == "")
                   {%>
                  暂无
                 <%} %>
                 <%else{ %> 
                 <%=owner.UserName%> 
                 <%} %>
            
            </h3><%} %>

            <%else {%>
            <%--ERROR! ----IsStudent取值错误--%>
            昵&ensp;&ensp;&ensp;&ensp;称：未知用户
            <%} %>

            <h3>邮&ensp;&ensp;&ensp;&ensp;箱：
            <%=owner.Email %>
            </h3>
            <h3>个人签名：
        <%if (owner.PersonalProfile == "")
          {%>这个人很懒，什么也没留下<br />
            <%} %>
            <%else
          { %>
            <% =owner.PersonalProfile %><br />
            <%} %>
            </h3>
        </div>

        <% }//end查看别人的信息 %>
     
    <br /><br /><br /><br />
    </form>
    
</body>
</html>

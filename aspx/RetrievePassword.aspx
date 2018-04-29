﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RetrievePassword.aspx.cs" Inherits="TimelyEvaluation.aspx.RetrievePassword" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/login.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/bootstrap/css/bootstrap.min.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/u.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/c.css"/>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_找回密码</title>
    <script type="text/javascript" src="/scripts/LoginAndRegister/jquery-1.8.3.min.js"></script>
	<script type="text/javascript" src="/scripts/LoginAndRegister/icheck.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $(":input").focus(function () {
                $(this).closest(".c-textbox_wrap").addClass("focused");
            }).blur(function () {
                $(this).closest(".c-textbox_wrap").removeClass("focused");
            });
            $('input').iCheck({
                checkboxClass: 'icheckbox_square-blue',
                radioClass: 'iradio_square-blue',
                increaseArea: '20%' // optional
            });
        });

        function CheckPasswordConfirm() {
            var password = document.getElementById("Password_TextBox").value;
            var password_confirm = document.getElementById("PasswordConfirm_TextBox").value;
            if (password == "") {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "请输入密码!<br/>";
                return false;
            }
            else if (password_confirm == "") {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "请输入确认密码!<br/>";
                return false;
            }
            else if (password != password_confirm) {
                document.getElementById("JS_CheckPasswordConfirm_div").innerHTML = "两次输入不一致!<br/>";
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
</head>

<body>
    <section class="c-screen  c-group-middle" style="background-color:#EDEDED;">
		<div class="p-login-container u-clearfix c-group-middle_content">
			<div class="c-box-login-wrap">
				<div class="p-login-form-links u-bounceInRight">
                    <img src="/Imgs/ustckkpk.jpg"/>
				</div>
				<div class="c-box-login u-bounceInLeft">
					<div class="c-box-login_header">
						<h3>找回密码</h3>
					</div>
                    <form id="frm1" runat="server">
					<div class="c-box-login_content">
						<div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span>
                                <asp:TextBox ID="Email_TextBox" runat="server" class="form-control" placeholder="邮箱" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
							</div>
						</div>

                        <div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i></i>@</span>
                                <select id="Email_select" runat="server" class="form-control">            
                                    <option value="@ustc.edu.cn" selected="selected">ustc.edu.cn</option>
                                    <option value="@mail.ustc.edu.cn">mail.ustc.edu.cn</option>
                                </select>
							</div>
						</div>

						<div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i class="glyphicon glyphicon-asterisk"></i></span>
                                <asp:TextBox ID="VerificationCode_TextBox" runat="server" class="form-control" placeholder="验证码" Width="70%"></asp:TextBox>
                                <asp:Button ID="GetVerificationCode_Button" runat="server" Text="发验证码" class="btn btn-success btn-lg u-f--r" style="font-size:16px;padding:5px" OnClick="GetVerificationCode_Button_Click" />
							</div>
						</div>

                        <div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                                <asp:TextBox ID="Password_TextBox" runat="server" class="form-control" placeholder="密码" onblur="CheckPasswordConfirm()" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
							</div>
                            <p class="Tips">*密码长度为6-18位</p>
						</div>

                        <div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                                <asp:TextBox ID="PasswordConfirm_TextBox" runat="server" class="form-control" placeholder="确认密码" onblur="CheckPasswordConfirm()" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
							</div>
						</div>

                        <div class="c-textbox_wrap">
                            <div id="JS_CheckPasswordConfirm_div" class="Tips"> </div> 
                        </div>

						<div class="c-box-login_footer">
                            <div style="display:inline-block;float:left;width:60%">
                            <div style="display:inline-block;float:left;width:100%"><a href="/aspx/Login.aspx">已有账号？点我登录</a></div>
                            <div style="display:inline-block;float:left;width:100%"><a href="/aspx/Register.aspx">没有账号？点我注册</a></div>
                            </div>
							<div style="display:inline-block;float:right"><asp:Button ID="RetrievePassword_Button" runat="server" Text="提交修改" class="btn btn-success btn-lg u-f--r" OnClick="RetrievePassword_Button_Click" /></div>
						</div>
					</div>
                    </form>
				</div>
				
				<div class="p-login-form-links u-bounceInRight">
                    <a href="/html/about.html">关于我们 了解更多</a>
				</div>
			</div>
		</div>
	</section>
</body>
</html>
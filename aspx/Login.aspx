<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TimelyEvaluation.aspx.Login" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/login.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/bootstrap/css/bootstrap.min.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/u.css"/>
<link rel="stylesheet" type="text/css" href="/css/LoginAndRegister/c.css"/>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_登录</title>
    <script type="text/javascript" src="/scripts/LoginAndRegister/jquery-1.8.3.min.js"></script>
	<script type="text/javascript" src="/scripts/LoginAndRegister/icheck.min.js"></script>
    <script>
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
						<h3>登录</h3>
					</div>
                    <form id="frm1" runat="server">
					<div class="c-box-login_content">
						<div class="c-textbox_wrap">
							<div class="input-group">
								<span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span>
                                <asp:TextBox ID="UserName_TextBox" runat="server" class="form-control" placeholder="邮箱" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
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
								<span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                                <asp:TextBox ID="Password_TextBox" runat="server" class="form-control" placeholder="密码" TextMode="Password" onpropertychange=" MaxLength(this,50);" oninput=" MaxLength(this,50);"></asp:TextBox>
							</div>
						</div>

						<div class="c-box-login_footer">
                            <div style="display:inline-block;float:left;width:60%">
                            <div style="display:inline-block;float:left;width:100%"><a href="/aspx/Register.aspx">没有帐号？点我注册</a></div>
                            <div style="display:inline-block;float:left;width:100%"><a href="/aspx/RetrievePassword.aspx">忘记密码？点我找回</a></div>
                            </div>
							<div style="display:inline-block;float:right"><asp:Button ID="Login_Button" runat="server" Text="登录" class="btn btn-success btn-lg u-f--r" OnClick="Login_Button_Click" /></div>
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

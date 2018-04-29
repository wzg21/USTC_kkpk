<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Wait.aspx.cs" Inherits="TimelyEvaluation.aspx.Wait" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>>跳转中...</title>
    <script src="/scripts/jquery-2.1.1/jquery.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <div id="AutoJumpPage">
            操作成功！请前去登录账户。
            <br />本页将在<span id="time" style="color: #FF0000"></span>秒后自动跳转至<a href="/aspx/Login.aspx">登录页面</a>
            <br /><br />
            如需立即跳转，请直接点击 <a href="/aspx/Login.aspx" style="color: #FF00FF">立即跳转>></a>
            <button class="Button0" type="button" onclick="JumpPage()">start</button>
        </div>
    
    </div>
    </form>
</body>

    <script type="text/javascript">
        //设定倒数秒数为5秒
        var t = 5;
        //显示倒数秒数 
        function showTime() {
            t -= 1;
            document.getElementById('time').innerHTML = t;
            if (t == 0) {
                location.href = '/aspx/Login.aspx';
            }
            //每秒执行一次,showTime() 
            setTimeout("showTime()", 1000);
        }
        //执行showTime() 
        showTime();
</script>



</html>

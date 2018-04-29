<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Activate.aspx.cs" Inherits="TimelyEvaluation.aspx.Activate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>科科评科_激活页面</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
 <% if (type == "error")
    { %>
        <h1 style="color:red">ERROR！</h1>
   <%}
    else if (type == "invalid")
    { %>
        <h1 style="color:red">验证链接无效！</h1>
   <%}
    else if (type == "succeed")
    { %>
        <h1 style="color:red">邮箱验证成功！</h1>
   <%} %>
    
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoogleOAuth.aspx.cs" Inherits="Umbraco8.App_Plugins.Skybrud.Analytics.Dialogs.GoogleOAuth" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=Title %></title>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,700" rel="stylesheet" type="text/css">
    <style>
        body {
            margin: 0;
            font-family: 'Open Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif;
            font-size: 12px;
        }
        .umb-editor-header {
            background: #fff;
            position: absolute;
            padding: 0 20px;
            z-index: 100;
            border-bottom: 1px solid #e9e9eb;
            width: 100%;
            box-sizing: border-box;
            height: 70px;
        }
        .umb-panel-header {
            height: 99px;
            background: #f8f8f8;
            border-bottom: 1px solid #d9d9d9;
            padding: 0 20px;
            line-height: 99px;
        }
        h1 {
            margin: 0;
            line-height: 99px;
            font-size: 18px;
            font-weight: normal;
        }
        .content {
            padding: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="content">
                <asp:Literal runat="server" ID="Content" />
            </div>
        </div>
    </form>
</body>
</html>

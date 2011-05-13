<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RequestRenewal.aspx.cs" Inherits="Pages_Public_SE_RequestRenewal" CodePage="65001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Förnya medlemskap</title>
    <style type="text/css">
        body
        {
            text-align: center;
            margin: 20px; /* shift whole page down by 25 pixels */
            background-color: #FFFFFF;
        }
        #mainForm
        {
            position: relative;
            border: 1px;
            border-style: solid;
            margin: 0 auto;
            text-align: left;
            width: 70%;
            background-color: #ffffff;
        }
        ul.mainForm
        {
            list-style-type: none;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-size: 15px;
        }
        li.mainForm
        {
            padding-bottom: 10px;
        }
        #mainFormError
        {
            position: relative;
            border: 1px;
            border-style: solid;
            margin: 0 auto;
            text-align: left;
            width: 70%;
        }
        input.mainFormError
        {
            background-color: #FADADD;
        }
        textarea.mainFormError
        {
            background-color: #FADADD;
        }
        select.mainFormError
        {
            background-color: #FADADD;
        }
        #formHeader
        {
            position: relative;
            width: 100%;
            background-color: #7F02A5;
            margin: 0 0 0 0;
            padding-bottom: 10px;
        }
        p.formHeader
        {
            text-align: right;
            margin: 0 0 0 0;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-weight: normal;
            color: #ffffff;
            font-size: 25px;
            position: relative;
            left: -5px;
            top: 4px;
            letter-spacing: 2px;
        }
        #formInfo
        {
            position: relative;
            width: 100%;
            background-color: #ffffff;
            margin: 0 0 0 0;
        }
        h2.formInfo
        {
            text-align: left;
            margin: 0 0 0 0;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-weight: normal;
            font-size: 20px;
            position: relative;
            left: 20px;
            top: 0px;
            letter-spacing: 1px;
            line-height: 150%;
            color: #ffffff;
        }
        p.formInfo
        {
            text-align: left;
            margin: 0 0 0 0;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-weight: normal;
            color: #000000;
            font-size: 12px;
            position: relative;
            left: 20px;
            top: 0px;
            color: #ffffff;
        }
        #formFields
        {
            position: relative;
            width: 100%;
            background-color: #ffffff;
            margin: 0 0 0 0;
        }
        label.formFieldQuestion
        {
            line-height: 125%;
            padding: 0 4px 1px 0;
            border: none;
            display: block;
            font-size: 95%;
            font-weight: bold;
        }
        label.formFieldOption
        {
            font-size: 90%;
            display: block;
            line-height: 1.0em;
            margin: -19px 0 0 25px;
            padding: 4px 0 5px 0;
            width: 90%;
        }
        input.formFieldStyle
        {
            display: block;
            line-height: 1.4em;
            margin: 8px 0 0 3px;
            width: 13px;
            height: 13px;
        }
        /* tooltip */a.info
        {
            font-family: Tahoma, Arial, Sans-Serif;
            text-decoration: none;
            position: relative;
        }
        a.info span
        {
            position: relative;
            display: none;
        }
        a.info:hover
        {
            position: relative;
            cursor: default;
        }
        a.info:hover .infobox
        {
            font-weight: normal;
            display: block;
            position: absolute;
            top: 20px; ;left:25px;width:205px;height:70px;border:1pxsolid#ccc;background:#f4f4f4url(question.gif)no-repeatbottomright;color:#000;text-align:left;font-size:0.7em;padding-left:10px;padding-top:10px;}/* pop-up calendar */
        button.calendarStyle
        {
            background-color: transparent;
            border: 0;
            height: 22px;
            width: 22px;
            background-image: url(imgs/calendar.png);
            cursor: pointer;
            cursor: hand;
        }
        p.footer
        {
            text-align: right;
            margin: 0 0 0 0;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-weight: normal;
            color: #ffffff;
            font-size: 9px;
            position: relative;
            top: 4px;
            left: -140px;
            letter-spacing: 2px;
        }
        a.footer
        {
            text-align: right;
            margin: 0 0 0 0;
            font-family: Tahoma, Arial, Verdana, sans-serif;
            font-weight: normal;
            color: #ffffff;
            font-size: 9px;
            position: relative;
            top: 4px;
            letter-spacing: 2px;
        }
    </style>
</head>
<body>
    <div id="mainForm">
        <form id="form1" runat="server">
        <div id="formHeader">
            <h2 class="formInfo">
                Förnya medlemskap</h2>
            <p class="formInfo">
            </p>
        </div>
        <br />
        <div style="padding: 10px">
            <asp:Panel ID="Panel1" runat="server">
                <ul class="mainForm" id="mainForm_1">
                    <li class="mainForm" id="fieldBox_1">
                        <label class="formFieldQuestion">
                            Namn&nbsp;*</label>
                        <asp:TextBox ID="TextBoxName" CssClass="mainForm" runat="server" Width="233px"></asp:TextBox>
                    </li>
                    <li class="mainForm" id="fieldBox_2">
                        <label class="formFieldQuestion">
                            E-postadress&nbsp;*</label>
                        <asp:TextBox ID="TextBoxMail" CssClass="mainForm" runat="server" Width="232px"></asp:TextBox>
                    </li>
                    <li class="mainForm" id="fieldBox_3">
                        <label class="formFieldQuestion" style="padding: 10px">
                            Förnya medlemskap&nbsp;*</label><span>
                                <asp:CheckBox ID="CheckBoxRenew" runat="server" Text="Ja, jag vill förnya mitt medlemskap i Piratpartiet och/eller Ung Pirat." />
                            </span></li>
                    <li class="mainForm">
                        <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
                    </li>
                </ul>
            </asp:Panel>
            <asp:Label CssClass="formFieldQuestion" ID="LabelReply" runat="server" Text=""></asp:Label>
        </div>
        </form>
    </div>
</body>
</html>

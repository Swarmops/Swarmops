<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>

<%
    PaymentCode newCode = PaymentCode.CreateFromPhone("Unknown");
    
	Response.ContentType="text/plain";
    Response.Write("<?php $paymentCode = '" + newCode.PaymentCode + "'; ?>"); 
    //Response.Write(newCode.PaymentCode); 
%>
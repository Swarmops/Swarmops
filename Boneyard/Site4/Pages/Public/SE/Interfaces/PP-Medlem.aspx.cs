using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Pirates;

public partial class Pages_Public_SE_Interfaces_PP_Medlem : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Response.ContentType = "text/plain";
		Response.ContentEncoding = System.Text.Encoding.Default;
		Response.Charset = "iso-8859-1";
		Response.StatusCode = 200;

		string phoneNumber = Request.QueryString["nr"];

		if (phoneNumber == null || phoneNumber.Length < 3)
		{
			Response.Write("Unable to parse phone number.");
			return;
		}
		
		PaymentCode paymentCode = PaymentCode.CreateFromPhone(phoneNumber);

		Response.Write("Tack! Betalningen mottagen. Din kvittokod \xE4r " + paymentCode.PaymentCode + ".");
	}
}

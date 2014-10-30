using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ExtensionMethods;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Support;

namespace Swarmops.Security
{
    public partial class BitId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Persistence.Key["BitIdTest_Raw"] = Request.ToRaw();
            Persistence.Key["BitIdTest_Uri"] = Request.Params["uri"];
            Persistence.Key["BitIdTest_Address"] = Request.Params["address"];
            Persistence.Key["BitIdText_Signature"] = Request.Params["signature"];
        }

        // ReSharper disable once InconsistentNaming
        [WebMethod]
        public static void callback(string uri, string signature, string address)
        {
            Persistence.Key["BitIdTest_Uri"] = uri;
            Persistence.Key["BitIdTest_Address"] = address;
            Persistence.Key["BitIdText_Signature"] = signature;
        }
    }
}



namespace System.Web.ExtensionMethods
{
    /// <summary>
    /// Extension methods for HTTP Request.
    /// <remarks>
    /// See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
    /// for details of implementation decisions.
    /// </remarks>
    /// </summary>
    public static class HttpRequestExtensions
    {

        /// <summary>
        /// Dump the raw http request to a string. 
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> that should be dumped.               </param>
        /// <returns>The raw HTTP request.</returns>
        public static string ToRaw(this HttpRequest request)
        {
            StringWriter writer = new StringWriter();

            WriteStartLine(request, writer);
            WriteHeaders(request, writer);
            WriteBody(request, writer);

            return writer.ToString();
        }

        public static string GetBody(this HttpRequest request)
        {
            StringWriter writer = new StringWriter();
            WriteBody(request, writer);

            return writer.ToString();
        }

        private static void WriteStartLine(HttpRequest request, StringWriter writer)
        {
            const string SPACE = " ";

            writer.Write(request.HttpMethod);
            writer.Write(SPACE + request.Url);
            writer.WriteLine(SPACE + request.ServerVariables["SERVER_PROTOCOL"]);
        }


        private static void WriteHeaders(HttpRequest request, StringWriter writer)
        {
            foreach (string key in request.Headers.AllKeys)
            {
                writer.WriteLine(string.Format("{0}: {1}", key, request.Headers[key]));
            }

            writer.WriteLine();
        }


        private static void WriteBody(HttpRequest request, StringWriter writer)
        {
            StreamReader reader = new StreamReader(request.InputStream);

            try
            {
                string body = reader.ReadToEnd();
                writer.WriteLine(body);
            }
            finally
            {
                reader.BaseStream.Position = 0;
            }
        }
    }
}

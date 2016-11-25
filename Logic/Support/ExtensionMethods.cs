using System.Globalization;
using System.IO;

namespace System.Web.ExtensionMethods
{
    /// <summary>
    ///     Extension methods for HTTP Response.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        ///     Set the response to be JSON format, and add appropriate headers.
        /// </summary>
        /// <param name="response">None.</param>
        public static void SetJson (this HttpResponse response)
        {
            response.ContentType = "application/json";
            response.CacheControl = "no-cache";
            response.AddHeader ("Last-Modified", DateTime.UtcNow.ToString (CultureInfo.InvariantCulture));
            response.AddHeader ("Pragma", "no-cache");
        }
    }


    /// <summary>
    ///     Extension methods for HTTP Request.
    ///     <remarks>
    ///         See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
    ///         for details of implementation decisions.
    ///     </remarks>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        ///     Dump the raw http request to a string.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" /> that should be dumped.               </param>
        /// <returns>The raw HTTP request.</returns>
        public static string ToRaw (this HttpRequest request)
        {
            StringWriter writer = new StringWriter();

            WriteStartLine (request, writer);
            WriteHeaders (request, writer);
            WriteBody (request, writer);

            return writer.ToString();
        }

        public static string GetBody (this HttpRequest request)
        {
            StringWriter writer = new StringWriter();
            WriteBody (request, writer);

            return writer.ToString();
        }

        private static void WriteStartLine (HttpRequest request, StringWriter writer)
        {
            const string SPACE = " ";

            writer.Write (request.HttpMethod);
            writer.Write (SPACE + request.Url);
            writer.WriteLine (SPACE + request.ServerVariables["SERVER_PROTOCOL"]);
        }


        private static void WriteHeaders (HttpRequest request, StringWriter writer)
        {
            foreach (string key in request.Headers.AllKeys)
            {
                writer.WriteLine ("{0}: {1}", key, request.Headers[key]);
            }

            writer.WriteLine();
        }


        private static void WriteBody (HttpRequest request, StringWriter writer)
        {
            StreamReader reader = new StreamReader (request.InputStream);

            try
            {
                string body = reader.ReadToEnd();
                writer.WriteLine (body);
            }
            finally
            {
                reader.BaseStream.Position = 0;
            }
        }
    }
}
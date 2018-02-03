<%@ WebHandler Language="C#" Class="Swarmops.Frontend.Automation.FileTransferHandler" %>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using WebSocketSharp;
using Mono.Unix.Native;
using Swarmops.Logic.Support.BackendServices;

namespace Swarmops.Frontend.Automation
{
    public class FileTransferHandler : DataV5Base, IHttpHandler
    {
        private readonly JavaScriptSerializer js = new JavaScriptSerializer();

        #region IHttpHandler Members

        public new bool IsReusable
        {
            get { return false; }
        }

        public new void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        #endregion

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            string filePath = Document.StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            List<FilesStatus> statuses = new List<FilesStatus>();
            NameValueCollection headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            Stream inputStream = context.Request.Files[0].InputStream;
            string fullName = Document.StorageRoot + Path.GetFileName(fileName);

            throw new NotImplementedException(
                "We should never get to partial upload. If we do, something is very very unexpected and this needs to be implemented.");

            //using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            //{
            //    var buffer = new byte[1024];

            //    int l = inputStream.Read(buffer, 0, 1024);
            //    while (l > 0)
            //    {
            //        fs.Write(buffer, 0, l);
            //        l = inputStream.Read(buffer, 0, 1024);
            //    }
            //    fs.Flush();
            //    fs.Close();
            //}
            //statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            string guid = context.Request.QueryString["Guid"];
            List<string> pdfsForConversion = new List<string>();
            List<string> pdfClientNames = new List<string>();

            AuthenticationData authData = CommonV5.GetAuthenticationDataAndCulture(context);

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile file = context.Request.Files[i];
                string fullName = Path.GetFileName(file.FileName);
                string clientFileName = string.Empty;

                Person uploadingPerson = authData.CurrentUser;
                Organization currentOrg = authData.CurrentOrganization;

                bool convertPdf = false;
                bool pdfGeneratedHere = false;

                if (file.FileName.ToLower().EndsWith(".pdf"))
                {
                    convertPdf = true;
                }
                else
                {
                    // If not PDF, try to load as an image

                    string filterType = context.Request.QueryString["Filter"];

                    if (filterType != "NoFilter")
                    {
                        // Try to load as image. If fails, not an acceptable file

                        try
                        {
                            Image image = Image.FromStream(file.InputStream);
                            image.Dispose();
                        }
                        catch (Exception)
                        {
                            // TODO: If general files accepted, then ok, otherwise error

                            // TODO: Accept general files

                            FilesStatus errorStatus = new FilesStatus(fullName, file.ContentLength);
                            errorStatus.error = "ERR_NOT_IMAGE";
                            errorStatus.url = string.Empty;
                            errorStatus.delete_url = string.Empty;
                            errorStatus.thumbnail_url = string.Empty;

                            statuses.Add(errorStatus);
                            // -1 for length means the file was NOT saved, and that it could not be parsed as image.
                            return;
                        }
                    }
                }

                if (string.IsNullOrEmpty(guid))
                {
                    throw new ArgumentException("No Context Guid supplied with upload");
                }

                string fileNameBase = guid + "-" + uploadingPerson.Identity.ToString("X8").ToLowerInvariant() + "-" +
                                        currentOrg.Identity.ToString("X4").ToLowerInvariant();

                int fileCounter = 0;
                string fileName;
                string storageFolder = Document.DailyStorageFolder;

                do
                {
                    fileCounter++;
                    fileName = fileNameBase + "-" + fileCounter.ToString("X2").ToLowerInvariant();
                } while (File.Exists(storageFolder + fileName) &&
                            fileCounter < 512);

                if (fileCounter >= 512)
                {
                    throw new InvalidOperationException(
                        "File name determination failed; probable file system permissions error");
                }

                string fullyQualifiedFileName = storageFolder + fileName;
                string relativeFileName = fullyQualifiedFileName.Substring(Document.StorageRoot.Length);

                file.InputStream.Position = 0;
                file.SaveAs(fullyQualifiedFileName);

                // Set file permissions to -r--r--r-- if live environment

                if (!Debugger.IsAttached)
                {
                    // Set to readonly, lock out changes, permit all read

                    Syscall.chmod(fullyQualifiedFileName,
                       FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);
                }


                if (convertPdf)
                {
                    // Convert PDF file into a series of PNG images, one per page

                    Process process = null;

                    if (Debugger.IsAttached) // Windows environment
                    {
                        process = Process.Start("cmd.exe",
                            "/c convert -background white -flatten " + fullyQualifiedFileName + " " +
                            fullyQualifiedFileName + "-%04d.png");

                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            FilesStatus errorStatus = new FilesStatus(fullName, -1); //file.ContentLength);
                            errorStatus.error = "ERR_BAD_PDF";
                            errorStatus.url = string.Empty;
                            errorStatus.delete_url = string.Empty;
                            errorStatus.thumbnail_url = string.Empty;

                            statuses.Add(errorStatus);
                            // -1 for length means the file was NOT saved, and that it could not be parsed.
                            continue; // with next file
                        }

                        pdfGeneratedHere = true;
                    }
                    else // live environment, not debug
                    {
                        int pdfPageCount = 0;
                        bool exceptionThrown = false;

                        try
                        {
                            pdfPageCount = PdfProcessor.GetPageCount(fullyQualifiedFileName);
                        }
                        catch (FormatException)
                        {
                            exceptionThrown = true;
                        }

                        if (exceptionThrown || pdfPageCount == 0)
                        {
                            // Bad PDF file

                            FilesStatus errorStatus = new FilesStatus(fullName, -1); //file.ContentLength);
                            errorStatus.error = "ERR_BAD_PDF";
                            errorStatus.url = string.Empty;
                            errorStatus.delete_url = string.Empty;
                            errorStatus.thumbnail_url = string.Empty;

                            statuses.Add(errorStatus);
                            // -1 for length means the file was NOT saved, and that it could not be parsed.
                            continue;
                        }

                        if (pdfPageCount < 10)
                        {
                            // Small file, so convert immediately instead of deferring

                            Documents docs = new PdfProcessor().RasterizeOne (fullyQualifiedFileName, file.FileName, guid, authData.CurrentUser);

                            // Ask backend for high-res conversion -- assumes at least one page

                            RasterizeDocumentHiresOrder backendOrder = new RasterizeDocumentHiresOrder(docs[0]);
                            backendOrder.Create();

                            statuses.Add(new FilesStatus(fullName, file.ContentLength));
                        }
                        else
                        {
                            // Defer low-res conversion to frontend daemon (called from client)

                            FilesStatus requiresConversion = new FilesStatus(fullName, -1);
                            requiresConversion.requiresPdfConversion = true;
                            statuses.Add(requiresConversion);

                            pdfsForConversion.Add(relativeFileName);
                            pdfClientNames.Add(file.FileName);
                        }

                    }
                }

                if (!convertPdf || pdfGeneratedHere)
                {
                    // In all cases except PDF deferred, create the document

                    Document.Create(fullyQualifiedFileName, file.FileName,
                        file.ContentLength,
                        guid, null, authData.CurrentUser);

                    statuses.Add(new FilesStatus(fullName, file.ContentLength));
                }
            }

            if (pdfsForConversion.Count > 0)
            {
                // Backend conversion of long files required

                using (
                    WebSocket socket =
                        new WebSocket("ws://localhost:" + SystemSettings.WebsocketPortFrontend + "/Front?Auth=" +
                                        Uri.EscapeDataString(authData.Authority.ToEncryptedXml())))
                {
                    socket.Connect();
                    JObject data = new JObject();
                    data["ServerRequest"] = "ConvertPdf";
                    data["PdfFiles"] = JArray.FromObject(pdfsForConversion.ToArray());
                    data["Guid"] = (string) guid;
                    data["ClientFileNames"] = JArray.FromObject(pdfClientNames.ToArray());
                    socket.Send(data.ToString());
                    Thread.Sleep(100);
                    socket.Ping(); // wait a little little while for send to work
                    socket.Close();
                }

            }

        }


        [WebMethod]
        public static AjaxConversionCallResult GetPdfConversionResult (string guid)
        {
            return (AjaxConversionCallResult) GuidCache.Get("PdfConversionResult-" + guid);
        }


        public class AjaxConversionCallResult: AjaxCallResult
        {
            public int SuccessCount { get; set; }
            public int FailCount { get; set; }
            public string[] FailFileNames { get; set; }
        }


        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            string jsonObj = this.js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            string filename = context.Request["f"];
            string filePath = Document.StorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            FileInfo[] files =
                new DirectoryInfo(Document.StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly);

            // completely wrong, redo from scratch

            string jsonObj = this.js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }
    }

    public class FilesStatus
    {
        public const string HandlerPath = "/";

        public FilesStatus()
        {
        }

        public FilesStatus(FileInfo fileInfo)
        {
            SetValues(fileInfo.Name, (int) fileInfo.Length);
        }

        public FilesStatus(string fileName, int fileLength)
        {
            SetValues(fileName, fileLength);
        }

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }
        public bool requiresPdfConversion { get; set; }

        private void SetValues(string fileName, int fileLength)
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = HandlerPath + "UploadFileHandler.ashx?f=" + fileName;
            thumbnail_url = HandlerPath + "Thumbnail.ashx?f=" + fileName;
            delete_url = HandlerPath + "UploadFileHandler.ashx?f=" + fileName;
            delete_type = "DELETE";
        }
    }
}

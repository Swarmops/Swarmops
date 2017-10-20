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

namespace Swarmops.Frontend.Automation
{
    public class FileTransferHandler : DataV5Base, IHttpHandler
    {
        private readonly JavaScriptSerializer js = new JavaScriptSerializer();

        public static string StorageRoot = Document.StorageRoot;

        private bool WeAreInDebugEnvironment
        {
            get { return Debugger.IsAttached; }
        }

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
            string filePath = StorageRoot + context.Request["f"];
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
            string fullName = StorageRoot + Path.GetFileName(fileName);

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

                DateTime utcNow = DateTime.UtcNow;

                string fileFolder = utcNow.Year.ToString("0000") + Path.DirectorySeparatorChar +
                                    utcNow.Month.ToString("00") + Path.DirectorySeparatorChar +
                                    utcNow.Day.ToString("00");

                if (!Directory.Exists(StorageRoot + fileFolder))
                {
                    Directory.CreateDirectory(StorageRoot + fileFolder);

                    // Set folder permissions to rwxr-xr-x if live environment

                    if (!WeAreInDebugEnvironment)
                    {
                        Syscall.chmod(StorageRoot + fileFolder,
                            FilePermissions.S_IRWXU | FilePermissions.S_IRGRP | FilePermissions.S_IROTH |
                            FilePermissions.S_IXGRP | FilePermissions.S_IXOTH);
                    }

                }

                int fileCounter = 0;
                string fileName = string.Empty;

                do
                {
                    fileCounter++;
                    fileName = fileNameBase + "-" + fileCounter.ToString("X2").ToLowerInvariant();
                } while (File.Exists(StorageRoot + fileFolder + Path.DirectorySeparatorChar + fileName) &&
                            fileCounter < 512);

                if (fileCounter >= 512)
                {
                    throw new InvalidOperationException(
                        "File name determination failed; probable file system permissions error");
                }

                string relativeFileName = fileFolder + Path.DirectorySeparatorChar + fileName;

                file.InputStream.Position = 0;
                file.SaveAs(StorageRoot + relativeFileName);

                // Set file permissions to rwxr--r-- if live environment

                if (!WeAreInDebugEnvironment)
                {
                    // Set to readonly, lock out changes, permit all read

                    Syscall.chmod(StorageRoot + relativeFileName,
                       FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);
                }


                if (convertPdf)
                {
                    // Convert PDF file into a series of PNG images, one per page

                    Process process = null;

                    if (WeAreInDebugEnvironment)
                    {
                        process = Process.Start("cmd.exe",
                            "/c convert -background white -flatten " + StorageRoot + relativeFileName + " " +
                            StorageRoot + relativeFileName +
                            "-%04d.png");

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
                        // Use qpdf to determine the number of pages in the PDF

                        string pageCountFileName = "/tmp/pagecount-" + guid + ".txt";

                        process = Process.Start("bash",
                            "-c \"qpdf --show-npages " + StorageRoot + relativeFileName + " > " + pageCountFileName +
                            "\"");

                        process.WaitForExit();
                        int pdfPageCount = 0;

                        if (process.ExitCode == 2) // error code for qpdf
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

                        // Read the resulting page count from the file we piped

                        using (StreamReader pageCountReader = new StreamReader(pageCountFileName))
                        {
                            string line = pageCountReader.ReadLine().Trim();
                            while (line.StartsWith("WARNING") || line.StartsWith("qpdf:"))
                            {
                                line = pageCountReader.ReadLine().Trim();  // ignore warnings and chatter
                            }
                            pdfPageCount = Int32.Parse(line);
                        }


                        File.Delete(pageCountFileName);

                        if (pdfPageCount < 10)
                        {
                            // Small file, so convert immediately instead of deferring

                            process = Process.Start("bash",
                                "-c \"convert -density 75 -background white -alpha remove " + Document.StorageRoot + relativeFileName +
                                " " + Document.StorageRoot + relativeFileName + "-%04d.png\"");

                            process.WaitForExit();

                            int pageCounter = 0; // the first produced page will be zero
                            Document lastDocument = null;

                            // Create all document records

                            while (pageCounter < pdfPageCount)
                            {
                                string pageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);

                                if (File.Exists(Document.StorageRoot + pageFileName))
                                {
                                    long fileLength = new FileInfo(Document.StorageRoot + pageFileName).Length;

                                    lastDocument = Document.Create(pageFileName,
                                        file.FileName + (pageCounter + 1).ToString(CultureInfo.InvariantCulture),
                                        fileLength, guid, null, authData.CurrentUser);

                                    if (!WeAreInDebugEnvironment)
                                    {
                                        // Set to readonly, lock out changes, permit all read

                                        Syscall.chmod(Document.StorageRoot + pageFileName,
                                            FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);
                                    }

                                }

                                pageCounter++;
                            }

                            // Ask backend for high-res conversion

                            using (WebSocket socket =
                                new WebSocket("ws://localhost:" + SystemSettings.WebsocketPortFrontend + "/Front?Auth=" +
                                              Uri.EscapeDataString(authData.Authority.ToEncryptedXml())))
                            {
                                socket.Connect();
                                JObject data = new JObject();
                                data["ServerRequest"] = "ConvertPdfHires";
                                data["DocumentId"] = lastDocument.Identity;
                                socket.Send(data.ToString());
                                socket.Ping(); // wait a little little while for send to work
                                socket.Close();
                            }

                            statuses.Add(new FilesStatus(fullName, file.ContentLength));
                        }
                        else
                        {
                            // Defer to backend

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

                    Document.Create(fileFolder + Path.DirectorySeparatorChar + fileName, file.FileName,
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
                    data["PersonId"] = authData.CurrentUser.Identity;
                    data["OrganizationId"] = authData.CurrentOrganization.Identity;
                    socket.Send(data.ToString());
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

        private static void ConvertPdfThread(object args)
        {
            ProcessThreadArguments argsProper = (ProcessThreadArguments) args;
        }


        private class ProcessThreadArguments
        {
            public string Guid { get; set; }
            public Organization Organization { get; set; }
            public Person CurrentUser { get; set; }
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
            string filePath = StorageRoot + filename;

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
                new DirectoryInfo(StorageRoot)
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

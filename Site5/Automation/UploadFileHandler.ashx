<%@ WebHandler Language="C#" Class="Swarmops.Frontend.Automation.FileTransferHandler" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Swarmops.Frontend.Automation {
	public class FileTransferHandler : IHttpHandler {
		private readonly JavaScriptSerializer js = new JavaScriptSerializer();

		public string StorageRoot {
			get
			{
			    if (System.Diagnostics.Debugger.IsAttached)
			    {
			        return @"C:\Windows\Temp\\"; // Windows debugging environment
			    }
			    else
			    {
			        return "/opt/swarmops/upload/"; // production location on Debian installation  TODO: config file
			    }
			}
		}
		public bool IsReusable { get { return false; } }

		public void ProcessRequest (HttpContext context) {
			context.Response.AddHeader("Pragma", "no-cache");
			context.Response.AddHeader("Cache-Control", "private, no-cache");

			HandleMethod(context);
		}

		// Handle request based on method
		private void HandleMethod (HttpContext context) {
			switch (context.Request.HttpMethod) {
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

		private static void ReturnOptions(HttpContext context) {
			context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
			context.Response.StatusCode = 200;
		}

		// Delete file from the server
		private void DeleteFile (HttpContext context) {
			var filePath = StorageRoot + context.Request["f"];
			if (File.Exists(filePath)) {
				File.Delete(filePath);
			}
		}

		// Upload file to the server
		private void UploadFile (HttpContext context) {
			var statuses = new List<FilesStatus>();
			var headers = context.Request.Headers;

			if (string.IsNullOrEmpty(headers["X-File-Name"])) {
				UploadWholeFile(context, statuses);
			} else {
				UploadPartialFile(headers["X-File-Name"], context, statuses);
			}

			WriteJsonIframeSafe(context, statuses);
		}

		// Upload partial file
		private void UploadPartialFile (string fileName, HttpContext context, List<FilesStatus> statuses) {
			if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
			var inputStream = context.Request.Files[0].InputStream;
			var fullName = StorageRoot + Path.GetFileName(fileName);

			using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write)) {
				var buffer = new byte[1024];

				var l = inputStream.Read(buffer, 0, 1024);
				while (l > 0) {
					fs.Write(buffer, 0, l);
					l = inputStream.Read(buffer, 0, 1024);
				}
				fs.Flush();
				fs.Close();
			}
			statuses.Add(new FilesStatus(new FileInfo(fullName)));
		}

		// Upload entire file
		private void UploadWholeFile (HttpContext context, List<FilesStatus> statuses) {
			for (int i = 0; i < context.Request.Files.Count; i++) {
				var file = context.Request.Files[i];
                string fullName = Path.GetFileName(file.FileName);

                // Store in MemoryStream in order to try to load as an image

                /*                
			    MemoryStream ms = new MemoryStream();
                byte[] fileData = new byte[file.ContentLength];

			    file.InputStream.Read(fileData, 0, file.ContentLength);
                ms.Write(fileData, 0, file.ContentLength);
			    ms.Position = 0;*/
                
                // Try to load as image. If fails, not an acceptable file

			    try
			    {
                    Image image = Image.FromStream(file.InputStream);
                }
			    catch (Exception)
			    {
			        FilesStatus errorStatus = new FilesStatus(fullName, file.ContentLength);
			        errorStatus.error = "ERR_NOT_IMAGE";
			        errorStatus.url = string.Empty;
			        errorStatus.delete_url = string.Empty;
			        errorStatus.thumbnail_url = string.Empty;
                    
                    statuses.Add(errorStatus);
                    // -1 for length means the file was NOT saved, and that it could not be parsed as image.
			        return;
			    }
                
                // TODO: Store to file name dependent on GUID
                
                // TODO: Store new image in db

			    file.InputStream.Position = 0;
				file.SaveAs(StorageRoot + Path.GetFileName(file.FileName));
                
				statuses.Add(new FilesStatus(fullName, file.ContentLength));
			}
		}

		private void WriteJsonIframeSafe (HttpContext context, List<FilesStatus> statuses) {
			context.Response.AddHeader("Vary", "Accept");
			try {
				if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
					context.Response.ContentType = "application/json";
				else
					context.Response.ContentType = "text/plain";
			} catch {
				context.Response.ContentType = "text/plain";
			}

			var jsonObj = js.Serialize(statuses.ToArray());
			context.Response.Write(jsonObj);
		}

		private static bool GivenFilename (HttpContext context) {
			return !string.IsNullOrEmpty(context.Request["f"]);
		}

		private void DeliverFile (HttpContext context) {
			var filename = context.Request["f"];
			var filePath = StorageRoot + filename;

			if (File.Exists(filePath)) {
				context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
				context.Response.ContentType = "application/octet-stream";
				context.Response.ClearContent();
				context.Response.WriteFile(filePath);
			} else
				context.Response.StatusCode = 404;
		}

		private void ListCurrentFiles (HttpContext context) {
			var files =
				new DirectoryInfo(StorageRoot)
					.GetFiles("*", SearchOption.TopDirectoryOnly);
            
            // completely wrong, redo from scratch

			string jsonObj = js.Serialize(files);
			context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
			context.Response.Write(jsonObj);
			context.Response.ContentType = "application/json";
		}
	}

    public class FilesStatus
    {
        public const string HandlerPath = "/";

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

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo) { SetValues(fileInfo.Name, (int)fileInfo.Length); }

        public FilesStatus(string fileName, int fileLength) { SetValues(fileName, fileLength); }

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

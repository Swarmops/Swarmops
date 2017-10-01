using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Swarmops.Frontend.Socket;
using Swarmops.Logic;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Swarmops.Frontend.Socket
{ 
    internal class FrontendServices: WebSocketBehavior
    {
        public FrontendServices()
        {
            base.IgnoreExtensions = true; // Necessary to suppress a Deflate exception that kills server otherwise
        }

        protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
        {
            // Basically just echo whatever's sent here

            Console.WriteLine(" - a client says " + e.Data);

            JObject json = JObject.Parse (e.Data);
            string serverRequest = (string) json["ServerRequest"];

            if (string.IsNullOrEmpty (serverRequest))
            {
                // Not a server request, so just echo the contents to everybody
                Sessions.Broadcast(e.Data);
            }

            switch (serverRequest)
            {
                case "AddBitcoinAddress":
                    FrontendLoop.AddBitcoinAddress((string) json["Address"]);
                    break;
                case "Metapackage":
                    ProcessMetapackage((string) json["XmlData"]);
                    break;
                case "Ping":
                    // TODO: Request heartbeat from backend
                    // Sessions.Broadcast("{\"messageType\":\"Heartbeat\"}");
                    break;
                case "ConvertPdf":
                    JArray pdfFilesArray = (JArray) json["PdfFiles"];
                    List<string> pdfFilesList = new List<string>();
                    foreach (string pdfFileString in pdfFilesArray)
                    {
                        pdfFilesList.Add(pdfFileString);
                    }
                    string[] pdfFiles = pdfFilesList.ToArray();

                    ConvertPdf (pdfFiles, (string) json["Guid"], Person.FromIdentity((int) json["PersonId"]), Organization.FromIdentity((int) json["OrganizationId"]));
                    break;

                default:
                    // do nothing;
                    break;
            }
        }



        private void ProcessMetapackage(string xmlData)
        {
            Console.WriteLine("Decoding XML: " + xmlData);
            SocketMessage message = SocketMessage.FromXml(xmlData);

            // Most should just be sent straight to backend. Some could possibly be processed already here.

            switch (message.MessageType)
            {
                case "...":
                    // some future to-be-defined processing
                    break;

                default:
                    // we're not handling here, send to backend
                    Console.WriteLine(" - sending " + message.MessageType + " upstream");
                    FrontendLoop.SendMessageUpstream(message);
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine(" * Attempted socket connection");

            string authBase64 = Context.QueryString["Auth"];

            authBase64 = Uri.UnescapeDataString (authBase64); // Defensive programming - % sign does not exist in base64 so this won't ever collapse a working encoding

            if (Authority.IsSystemAuthorityTokenValid(authBase64))
            {
                _authority = null; // System authority
                Console.WriteLine(" - - valid system authentication token");
            }
            else
            {
                _authority = Authority.FromEncryptedXml(authBase64);
                this.Sessions.RegisterSession(this, this._authority); // Only register non-system sessions
                Console.WriteLine(" - - authenticated: " + this._authority.Person.Canonical);
            }

            base.OnOpen();
        }

        protected override void OnClose (CloseEventArgs e)
        {
            Console.WriteLine(" - client closed");
            base.OnClose (e);

            this.Sessions.UnregisterSession(this);

            // Sessions.Broadcast("{\"messageType\":\"EditorCount\"," + String.Format("\"editorCount\":\"{0}\"", Sessions.ActiveIDs.ToArray().Length) + '}');
        }

        private void ConvertPdf(string[] pdfFiles, string guid, Person person, Organization organization)
        {
            List<string> failedConversionFileNames = new List<string>();

            using (StreamWriter debugWriter = new StreamWriter("/tmp/PdfConversionDebug-" + guid + ".txt"))
            {

                try
                {
                    debugWriter.WriteLine("ConvertPdf started");

                    int fileCount = pdfFiles.Length;
                    int successCount = 0;
                    int failCount = 0;

                    Process process = null;
                    Document lastDocument = null;

                    for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                    {
                        // Set progress to indicate we're at file 'index' of 'fileCount'

                        int progress = fileIndex * 99 / fileCount; // 99 at most -- 100 indicates complete
                        int progressMax = (fileIndex + 1) * 99 / fileCount - 1;
                        int progressFileStep = 99 / fileCount;
                        int currentFilePageCount = 0;
                        int currentFilePageStep = 0;

                        string relativeFileName = pdfFiles[fileIndex];

                        debugWriter.WriteLine("------------------------------------");
                        debugWriter.WriteLine("{3:D2}%, Converting PDF file {0} of {1}; {2}", fileIndex + 1, fileCount,
                            relativeFileName, progress);

                        // Use qpdf to determine the number of pages in the PDF

                        string pageCountFileName = "/tmp/pagecount-" + guid + ".txt";

                        process = Process.Start("bash",
                            "-c \"qpdf --show-npages " + Document.StorageRoot + relativeFileName + " > " + pageCountFileName + "\"");

                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            // Bad PDF file
                            failCount++;
                            failedConversionFileNames.Add(
                                ((string[])GuidCache.Get("PdfClientNames-" + guid))[fileIndex].Replace("'", "")
                                    .Replace("\"", "")); // caution; we're displaying user input, guard against XSS
                            continue;
                        }

                        // Read the resulting page count from the file we piped

                        using (StreamReader pageCountReader = new StreamReader(pageCountFileName))
                        {
                            string pageCountString = pageCountReader.ReadToEnd().Trim();
                            debugWriter.WriteLine("{0:D2}%, page count is '{1}'", progress, pageCountString);
                            currentFilePageCount = Int32.Parse(pageCountString);
                            debugWriter.WriteLine("{0:D2}%, parsed to int as {1}", progress, currentFilePageCount);
                            currentFilePageStep = progressFileStep / currentFilePageCount;
                        }

                        File.Delete(pageCountFileName);

                        // Begin the conversion

                        debugWriter.WriteLine("{0:D2}%, deleted page count file, starting conversion process", progress);

                        // Density 75 means 75dpi means conversion
                        // Hires 600dpi conversion is also made, but from backend after this conversion

                        process = Process.Start("bash",
                            "-c \"convert -density 75 -background white -alpha remove " + Document.StorageRoot + relativeFileName +
                            " " + Document.StorageRoot + relativeFileName + "-%04d.png\"");

                        int pageCounter = 0; // the first produced page will be zero
                        int currentPageBaseProgress = progressFileStep * fileIndex;
                        string testPageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);
                        debugWriter.WriteLine("{0:D2}%, testPageFileName set to {1}", progress, testPageFileName);
                        string lastPageFileName = testPageFileName;

                        // Convert works by first calling imagemagick that creates /tmp/magick-* files

                        int startMagickCount = Directory.GetFiles("/tmp", "magick-*").Count();

                        while (pageCounter < currentFilePageCount)
                        {
                            while (!File.Exists(Document.StorageRoot + testPageFileName))
                            {
                                // Wait for file to appear

                                debugWriter.WriteLine("{0:D2}%, {2:O}, waiting for page #{1} to appear", progress, pageCounter + 1, DateTime.UtcNow);
                                debugWriter.Flush();
                                GuidCache.Set("Pdf-" + guid + "-Progress", progress);

                                if (!process.HasExited)
                                {
                                    process.WaitForExit(250);
                                    // this is a more elaborate version of thread.sleep that prevents Apache recycling
                                }

                                if (pageCounter == 0)
                                {
                                    // If first page hasn't appeared yet, check for the Magick temp files

                                    int currentMagickCount = Directory.GetFiles("/tmp", "magick-*").Count();
                                    int currentFilePercentage = currentMagickCount*50/pageCounter;
                                    if (currentFilePercentage > 50)
                                    {
                                        currentFilePercentage = 50; // we may be not the only one converting right now
                                    }
                                    BroadcastGuidProgress(organization, guid,
                                        progressFileStep*fileIndex + currentFilePercentage*100/progressFileStep);
                                }
                            }

                            progress = progressFileStep * fileIndex + currentFilePageStep/2 + currentFilePageStep * (pageCounter + 1) / 2;
                            debugWriter.WriteLine("{0:D2}%, found page #{1}", progress, pageCounter + 1);
                            BroadcastGuidProgress(organization, guid, progress);

                            // If the page# file that has appeared is 1+, then the preceding file is ready

                            if (pageCounter > 0)
                            {
                                long fileLength = new FileInfo(Document.StorageRoot + lastPageFileName).Length;
                                debugWriter.WriteLine("{0:D2}%, saving page #{1}, bytecount {2}", progress, pageCounter, fileLength);

                                Document.Create(lastPageFileName,
                                    "(BackendConvertedFile) " + pageCounter.ToString(CultureInfo.InvariantCulture),
                                    fileLength, guid, null, person);

                                // Prepare to save the next file
                                lastPageFileName = testPageFileName;
                            }

                            // Increase the page counter and the file we're looking for

                            pageCounter++;
                            testPageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);
                            debugWriter.WriteLine("{0:D2}%, testPageFileName set to {1}", progress, testPageFileName);
                        }

                        // We've seen the last page being written -- wait for process to exit
                        debugWriter.WriteLine("{0:D2}%, waiting for process exit", progress);

                        process.WaitForExit();

                        // Save the last page

                        long fileLengthLastPage = new FileInfo(Document.StorageRoot + lastPageFileName).Length;
                        debugWriter.WriteLine("{0:D2}%, saving last page #{1}, bytecount {2}", progress, pageCounter, fileLengthLastPage);

                        lastDocument = Document.Create(lastPageFileName,
                            "(BackendConvertedFile) " + pageCounter.ToString(CultureInfo.InvariantCulture),
                            fileLengthLastPage, guid, null, person);

                    }

                    // TODO: notify backend to rerasterize doc at 600dpi


                }
                catch (Exception e)
                {
                    debugWriter.WriteLine("Exception thrown: " + e.ToString());

                    throw;
                }
                finally
                {
                    debugWriter.WriteLine("Handler exiting");

                    BroadcastGuidProgress(organization, guid, 100);
                }
            }
        }

        private void BroadcastGuidProgress(Organization organization, string guid, int progress)
        {
            JObject json = new JObject();
            json["Guid"] = guid;
            json["Progress"] = progress;

            FrontendLoop.BroadcastToOrganization(organization, json);
        }

        public Authority Authority { get { return this._authority; } }

        private Authority _authority = null;
    }


}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mono.Unix.Native;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.BackendServices;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class PdfProcessor
    {
        private static readonly string StorageRoot = Document.StorageRoot;

        public delegate void ProgressEventHandler(object sender, ProgressEventArgs args);
        public event ProgressEventHandler RasterizationProgress;

        [Flags]
        public enum PdfProcessorOptions
        {
            None = 0,
            HighQuality = 0x0001,
            ForceOrphans = 0x0002
        };

        public static int GetPageCount(string fullyQualifiedFileName)
        {
            if (Debugger.IsAttached)
            {
                // This call is intended for live environments only; if a debugger is attached, break because something's wrong

                Debugger.Break();
            }

            // If we have the page count in cache, use it

            object cachedObject = GuidCache.Get("PdfPageCount-" + fullyQualifiedFileName);
            if (cachedObject != null)
            {
                return (int) cachedObject;
            }

            // Use qpdf to determine the number of pages in the PDF

            string pageCountFileName = "/tmp/pagecount-" + Guid.NewGuid().ToString() + ".txt";

            Process process = Process.Start("bash",
                "-c \"qpdf --show-npages " + fullyQualifiedFileName + " > " + pageCountFileName +
                "\"");

            process.WaitForExit();
            int pdfPageCount = 0;

            if (process.ExitCode == 2) // error code for qpdf
            {
                throw new FormatException("Bad PDF file");
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

            //File.Delete(pageCountFileName);

            GuidCache.Set("PdfPageCount-" + fullyQualifiedFileName, pdfPageCount);

            return pdfPageCount;
        }


        public Documents RasterizeMany(RasterizationTarget[] targets, string guid, Person uploader,
            Organization organization)
        {
            Documents result = new Documents();

            int pageCountTotal = 0;
            int pageCountRunning = 0;


            foreach (RasterizationTarget target in targets)
            {
                pageCountTotal += GetPageCount(target.FullyQualifiedFileName);
            }


            foreach (RasterizationTarget target in targets)
            {
                int pdfPageCount = GetPageCount(target.FullyQualifiedFileName); // cached

                // Create progress range

                ProgressRange range = new ProgressRange
                {
                    Minimum = pageCountRunning * 100 / pageCountTotal,
                    Maximum = (pageCountRunning + pdfPageCount) * 100 / pageCountTotal
                };

                // Request one conversion

                Documents docs = RasterizeOne(target.FullyQualifiedFileName, target.ClientFileName, guid, uploader,
                    organization, range);

                // Update pages converted

                pageCountRunning += pdfPageCount;

                // Finally, ask the backend to do the high-res conversions, but now we have the basic, fast ones

                if (docs.Count > 0)
                {
                    RasterizeDocumentHiresOrder backendOrder = new RasterizeDocumentHiresOrder(docs[0]);
                    backendOrder.Create();
                }
            }

            // Set progress to 100% to avoid rounding errors freezing the UI at 99%

            BroadcastProgress(organization, guid, 100);

            return result;
        }

        

        public Documents RasterizeOne (string fullyQualifiedFileName, string clientFileName, string guid, Person uploader, Organization organization = null, ProgressRange progressRange = null)
        {
            int pdfPageCount = GetPageCount(fullyQualifiedFileName);
            string relativeFileName = fullyQualifiedFileName.Substring(Document.StorageRoot.Length);

            Process process = Process.Start("bash",
                "-c \"convert -density 75 -background white -alpha remove " + fullyQualifiedFileName +
                " " + fullyQualifiedFileName + "-%04d.png\"");

            Documents documents = new Documents();

            if (progressRange == null)
            {
                progressRange = new ProgressRange();
            }

            int pageCounter = 0; // the first produced page will be zero
            string testPageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);
            string lastPageFileName = testPageFileName;

            // Convert works by first calling imagemagick that creates /tmp/magick-* files

            int lastProgress = progressRange.Minimum;
            int progress = progressRange.Minimum;

            while (pageCounter < pdfPageCount)
            {
                while (!File.Exists(Document.StorageRoot + testPageFileName))
                {
                    // Wait for file to appear

                    if (!process.HasExited)
                    {
                        process.WaitForExit(250);
                    }

                    if (pageCounter == 0)
                    {
                        // If first page hasn't appeared yet, check for the Magick temp files

                        int currentMagickCount = Directory.GetFiles("/tmp", "magick-*").Count();
                        int currentFilePercentage = currentMagickCount * 50 / pdfPageCount;
                        if (currentFilePercentage > 50)
                        {
                            currentFilePercentage = 50; // we may be not the only one converting right now
                        }

                        progress = progressRange.Minimum + currentFilePercentage * 100 / progressRange.Range;
                        if (progress > lastProgress)  // can't use Not-Equal; temp files slowly deleted before next step
                        {
                            BroadcastProgress(organization, guid, progress);
                            lastProgress = progress;
                        }
                    }
                }

                progress = progressRange.Minimum + progressRange.Range / 2 + ((pageCounter + 1) * progressRange.Range / pdfPageCount) / 2;
                if (progress > lastProgress)
                {
                    BroadcastProgress(organization, guid, progress);
                    lastProgress = progress;
                }

                // If the page# file that has appeared is 1+, then the preceding file is ready

                if (pageCounter > 0)
                {
                    long fileLength = new FileInfo(Document.StorageRoot + lastPageFileName).Length;

                    documents.Add( Document.Create(lastPageFileName,
                        clientFileName + " {{LOCPAGE-" + (pageCounter).ToString(CultureInfo.InvariantCulture) + "-" + pdfPageCount.ToString(CultureInfo.InvariantCulture) + "}}",
                        fileLength, guid, null, uploader));

                    // Set to readonly, lock out changes, permit all read

                    Syscall.chmod(Document.StorageRoot + lastPageFileName,
                        FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);

                    // Prepare to save the next file
                    lastPageFileName = testPageFileName;
                }

                // Increase the page counter and the file we're looking for

                pageCounter++;
                testPageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);
            }

            // We've seen the last page being written -- wait for process to exit to assure it's complete

            if (!process.HasExited)
            {
                process.WaitForExit();
            }

            // Save the last page

            long fileLengthLastPage = new FileInfo(Document.StorageRoot + lastPageFileName).Length;

            documents.Add(Document.Create(lastPageFileName,
                clientFileName + " {{LOCPAGE-" + (pageCounter).ToString(CultureInfo.InvariantCulture) + "-" + pdfPageCount.ToString(CultureInfo.InvariantCulture) + "}}",
                fileLengthLastPage, guid, null, uploader));

            // Set to readonly, lock out changes, permit all read

            Syscall.chmod(Document.StorageRoot + lastPageFileName,
                FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);


            /* -- OLD CODE BELOW --
            process.WaitForExit();

            int pageCounter = 0; // the first produced page will be zero

            // Create all document records


            while (pageCounter < pdfPageCount)
            {
                string pageFileName = String.Format("{0}-{1:D4}.png", relativeFileName, pageCounter);

                if (File.Exists(Document.StorageRoot + pageFileName))
                {
                    long fileLength = new FileInfo(Document.StorageRoot + pageFileName).Length;

                    documents.Add(Document.Create(pageFileName,
                        clientFileName + " " + (pageCounter + 1).ToString(CultureInfo.InvariantCulture) + "/" + pdfPageCount.ToString(CultureInfo.InvariantCulture),
                        fileLength, guid, null, uploader));

                    Syscall.chmod(Document.StorageRoot + pageFileName,
                        FilePermissions.S_IRUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH);

                }

                pageCounter++;
            } */

            return documents;
        }

        public static void RerasterizeAll()
        {
            // Because of a bug with transparent-background PDFs converting to black-background bitmaps, all PDFs imaged before
            // DB version 37 need to be re-bitmapped

            Documents documents = Documents.GetAll();

            string firstPart = string.Empty;
            string lastFirstPart = firstPart;
            int startId = 0;
            int lastId = 0;
            bool wroteDots = false;

            foreach (Document document in documents)
            {
                if (document.Description.Length < 32)
                {
                    continue; // no GUID
                }

                // a ServerFileName looks like "2015/06/22/29c88f2b-44e6-4a05-a6b0-f9aaec835c5b-00000001-0007-01-0019.png"
                //                              1234567890123456789012345678901234567890123456789012345678901234

                if (document.ServerFileName.Length < 64)
                {
                    continue;
                }
                if (!document.ServerFileName.ToLowerInvariant().EndsWith(".png"))
                {
                    continue;
                }
                if (document.ForeignId == 0)
                {
                    // orphan document
                    continue;
                }

                firstPart = document.ServerFileName.Substring(0, 64);

                if (firstPart != lastFirstPart)
                {
                    if (startId != 0)
                    {
                        // wrap up the previous one
                        if (lastId != startId)
                        {
                            Console.WriteLine("{0}, done.", lastId);
                        }
                        else
                        {
                            Console.WriteLine(", done.");
                        }
                    }

                    // Start processing a new document
                    wroteDots = false; // this is just cosmetics

                    startId = lastId = document.Identity;

                    Console.Write(@"Regenerating document #{0}", startId);

                    Process process = Process.Start("bash",
                            "-c \"convert -density 300 -background white -alpha remove " + StorageRoot + firstPart + " " +
                            StorageRoot + firstPart +
                            "-%04d-hires.png\""); // Density 300 means 600dpi means production-grade conversion

                    process.WaitForExit();

                    lastFirstPart = firstPart;
                }
                else
                {
                    // if firstPart and lastFirstPart still match
                    // we've already regenerated this document

                    if (!wroteDots) // cosmetics
                    {
                        Console.Write("..");
                        wroteDots = true;
                    }
                    lastId = document.Identity;
                }
            }
        }

        public static void Rerasterize(Document document, PdfProcessorOptions options = PdfProcessorOptions.None)
        {
            if (document.Description.Length < 32)
            {
                throw new InvalidOperationException("Document has no GUID");
            }

            // a ServerFileName looks like "2015/06/22/29c88f2b-44e6-4a05-a6b0-f9aaec835c5b-00000001-0007-01-0019.png"
            //                              1234567890123456789012345678901234567890123456789012345678901234

            if (document.ServerFileName.Length < 64)
            {
                throw new InvalidOperationException("Server file name has wrong scheme");
            }

            if (!document.ServerFileName.ToLowerInvariant().EndsWith(".png"))
            {
                throw new InvalidOperationException("Document is not rasterized");
            }
            if (document.ForeignId == 0 && ((int) options & (int) PdfProcessorOptions.ForceOrphans) == 0)
            {
                throw new InvalidOperationException("Document is an orphan");
            }

            int density = 75;
            string suffix = string.Empty;

            if (((int) options & (int) PdfProcessorOptions.HighQuality) > 0)
            {
                density = 300;
                suffix = "-hires"; // hires conversion uses different filename
            }


            string firstPart = document.ServerFileName.Substring(0, 64);

            string commandLine = "convert -density " + density.ToString(CultureInfo.InvariantCulture) +
                                    " -background white -alpha remove " + StorageRoot + firstPart + " " +
                                    StorageRoot + firstPart +
                                    "-%04d" + suffix + ".png";

            Process process = Process.Start("/bin/bash",
                "-c \"" + commandLine + "\"");
            process.PriorityClass = ProcessPriorityClass.BelowNormal; // play nice - this is a heavy op

            process.WaitForExit();
        }

        public static string TemplateToPdf(string svgFileName)
        {
            if (svgFileName.Contains('\"'))
            {
                throw new ArgumentException("Filename must not contain double quotes");
            }
            if (svgFileName.Contains(' '))
            {
                throw new ArgumentException("Filename must not contain whitespace");
            }

            if (!File.Exists(svgFileName))
            {
                throw new ArgumentException("File does not exist");
            }

            string tempFolder = @"/tmp/";

            Guid guid = new Guid();

            string tempFileName = tempFolder + guid.ToString("D") + ".pdf";

            Process process = Process.Start("bash",
                    "-c \"inkscape --without-gui --export-to-pdf=" + tempFileName + " " + svgFileName + "\"");  // inkscape is a package-level dependency

            process.WaitForExit();

            return tempFileName;
        }

        private void BroadcastProgress(Organization organization, string guid, int progress)
        {
            if (RasterizationProgress != null)
            {
                RasterizationProgress(this, new ProgressEventArgs
                {
                    Guid = guid,
                    Organization = organization,
                    Progress = progress
                });
            }
        }
    }

    public class RasterizationTarget
    {
        public string FullyQualifiedFileName { get; set; }
        public string ClientFileName { get; set; }
    }

    public class ProgressRange
    {
        public ProgressRange()  // default values
        {
            this.Minimum = 0;
            this.Maximum = 100;
        }

        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public int Range => Maximum - Minimum;
    }

    public class ProgressEventArgs : EventArgs
    {
        public Organization Organization { get; set; }
        public string Guid { get; set; }
        public int Progress { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.BotCode
{
    public class PdfProcessor
    {
        private static readonly string StorageRoot = "/var/lib/swarmops/upload/";

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
                            "-c \"convert -density 600 -background white -flatten " + StorageRoot + firstPart + " " +
                            StorageRoot + firstPart +
                            "-%04d.png\""); // Density 600 means 600dpi means production-grade conversion

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

        public static void Rerasterize(Document document)
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
            if (document.ForeignId == 0)
            {
                throw new InvalidOperationException("Document is an orphan");
            }

            string firstPart = document.ServerFileName.Substring(0, 64);

            Console.Write("Regenerating document...");

            Process process = Process.Start("bash",
                    "-c \"convert -density 600 -background white -flatten " + StorageRoot + firstPart + " " +
                    StorageRoot + firstPart +
                    "-%04d.png\""); // Density 600 means 600dpi means production-grade conversion

            process.WaitForExit();

            Console.WriteLine(" done.");
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
    }
}

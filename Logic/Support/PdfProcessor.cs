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
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Support
{
    public class PdfProcessor
    {
        private static readonly string StorageRoot = Document.StorageRoot;

        [Flags]
        public enum PdfProcessorOptions
        {
            None = 0,
            HighQuality = 0x0001,
            ForceOrphans = 0x0002
        };

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
                            "-c \"convert -density 600 -background white -alpha remove " + StorageRoot + firstPart + " " +
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

        public static void Rerasterize(Document document, PdfProcessorOptions options = PdfProcessorOptions.None)
        {
            using (StreamWriter debugWriter = new StreamWriter("/tmp/pdfregen-debug.txt"))
            {
                debugWriter.WriteLine("Document description: " + document.Description);
                debugWriter.Flush();

                if (document.Description.Length < 32)
                {
                    throw new InvalidOperationException("Document has no GUID");
                }

                // a ServerFileName looks like "2015/06/22/29c88f2b-44e6-4a05-a6b0-f9aaec835c5b-00000001-0007-01-0019.png"
                //                              1234567890123456789012345678901234567890123456789012345678901234

                debugWriter.WriteLine("Document server file name: " + document.ServerFileName);
                debugWriter.Flush();

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
                    density = 600;
                    suffix = "-hires"; // hires conversion uses different filename
                }


                string firstPart = document.ServerFileName.Substring(0, 64);

                debugWriter.WriteLine("Regenerating... ");
                debugWriter.Flush();

                string commandLine = "convert -density " + density.ToString(CultureInfo.InvariantCulture) +
                                     " -background white -alpha remove " + StorageRoot + firstPart + " " +
                                     StorageRoot + firstPart +
                                     "-%04d" + suffix + ".png";

                debugWriter.WriteLine("Running \"" + commandLine + "\"");
                debugWriter.Flush();

                Process process = Process.Start("bash",
                    "-c \"" + commandLine + "\"");
                process.PriorityClass = ProcessPriorityClass.Idle; // play nice - this is a heavy op

                process.WaitForExit();

                debugWriter.WriteLine("Done.");
                debugWriter.Flush();
            }
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

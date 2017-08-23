using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.BotCode
{
    public class PdfProcessor
    {
        private static readonly string StorageRoot = "/var/lib/swarmops/upload/";

        public static void RegenerateAll()
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
    }
}

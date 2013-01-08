using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Pirates;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class Documents: List<Document>
    {
        public static Documents ForObject (IHasIdentity identifiableObject)
        {
            Documents newInstance =
                FromArray(
                    PirateDb.GetDatabaseForReading().GetDocumentsForForeignObject(
                        Document.GetDocumentTypeForObject(identifiableObject), identifiableObject.Identity));

            newInstance.sourceObject = identifiableObject;

            return newInstance;
        }

        public static Documents FromArray (BasicDocument[] basicArray)
        {
            var result = new Documents { Capacity = (basicArray.Length * 11 / 10) };

            foreach (BasicDocument basic in basicArray)
            {
                result.Add(Document.FromBasic(basic));
            }

            return result;
        }


        public Document Add (string serverFileName, string clientFileName, Int64 fileSize, 
            string description, Person uploader)
        {
            // This is kind of experimental. Is it a good idea to be able to write
            // Expenses.Documents.Add (...), or is it just stupid?


            if (sourceObject == null)
            {
                throw new InvalidOperationException(
                    "Cannot add documents to a Documents instance that was not created from an object.");
            }

            Document newDocument = 
                Document.Create(serverFileName, clientFileName, fileSize, description,
                                   sourceObject, uploader);

            base.Add(newDocument);

            return newDocument;
        }


        public void SetForeignObjectForAll (IHasIdentity foreignObject)
        {
            foreach (Document doc in this)
            {
                doc.SetForeignObject(foreignObject);
            }
        }

        private IHasIdentity sourceObject = null;
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class Document : BasicDocument
    {
        private Document(BasicDocument document) : base(document)
        {
        }

        public new string ServerFileName
        {
            get { return base.ServerFileName; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetDocumentServerFileName(Identity, value);
                base.ServerFileName = value;
            }
        }

        public IHasIdentity ForeignObject
        {
            get
            {
                switch (DocumentType)
                {
                    case DocumentType.ExpenseClaim:
                        return ExpenseClaim.FromIdentity(ForeignId);

                    case DocumentType.FinancialTransaction:
                        return FinancialTransaction.FromIdentity(ForeignId);

                    case DocumentType.InboundInvoice:
                        return InboundInvoice.FromIdentity(ForeignId);

                    case DocumentType.PaperLetter:
                        return PaperLetter.FromIdentity(ForeignId);

                    case DocumentType.PayrollItem:
                        return PayrollItem.FromIdentity(ForeignId);

                    default:
                        throw new NotImplementedException("DocumentType needs implementation: " +
                                                          DocumentType.ToString());
                }
            }
        }

        protected static string StorageRoot
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return @"C:\Windows\Temp\Swarmops-Debug\"; // Windows debugging environment
                }
                else
                {
                    return "/var/lib/swarmops/upload/"; // production location on Debian installation
                }
            }
        }

        public static Document FromIdentity(int documentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetDocument(documentId));
        }

        public static Document FromIdentityAggressive(int documentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetDocument(documentId));
                // "For writing" is intentional - bypasses a race condition in replication
        }

        public static Document FromBasic(BasicDocument basicDocument)
        {
            return new Document(basicDocument);
        }

        public static Document Create(string serverFileName, string clientFileName, Int64 fileSize,
            string description, IHasIdentity identifiableObject, Person uploader)
        {
            int newDocumentId = SwarmDb.GetDatabaseForWriting().
                CreateDocument(serverFileName, clientFileName, fileSize, description,
                    GetDocumentTypeForObject(identifiableObject),
                    identifiableObject == null ? 0 : identifiableObject.Identity, uploader.Identity);

            return FromIdentityAggressive(newDocumentId);
        }

        public static DocumentType GetDocumentTypeForObject(IHasIdentity foreignObject)
        {
            if (foreignObject == null)
            {
                // docs uploaded; foreign object not yet constructed

                return DocumentType.Unknown;
            }
            else if (foreignObject is Person)
            {
                return DocumentType.PersonPhoto;
            }
            else if (foreignObject is ExpenseClaim)
            {
                return DocumentType.ExpenseClaim;
            }
            else if (foreignObject is FinancialTransaction)
            {
                return DocumentType.FinancialTransaction;
            }
            else if (foreignObject is TemporaryIdentity)
            {
                return DocumentType.Temporary;
            }
            else if (foreignObject is InboundInvoice)
            {
                return DocumentType.InboundInvoice;
            }
            else if (foreignObject is PaperLetter)
            {
                return DocumentType.PaperLetter;
            }
            else if (foreignObject is ExternalActivity)
            {
                return DocumentType.ExternalActivityPhoto;
            }
            else
            {
                throw new ArgumentException("Unrecognized foreign object type:" + foreignObject.GetType().ToString());
            }
        }

        public void SetForeignObject(IHasIdentity foreignObject)
        {
            SwarmDb.GetDatabaseForWriting()
                .SetDocumentForeignObject(Identity, GetDocumentTypeForObject(foreignObject), foreignObject.Identity);
        }


        public void Delete()
        {
            // Unlink, actually

            SetForeignObject(new TemporaryIdentity(0));
            File.Delete(StorageRoot + ServerFileName);
            SwarmDb.GetDatabaseForWriting().SetDocumentDescription(Identity, "Deleted");
        }

        public StreamReader GetReader(Encoding encoding)
        {
            return new StreamReader(StorageRoot + ServerFileName, encoding);
        }

        public StreamReader GetReader(int encodingCodePage)
        {
            return GetReader(Encoding.GetEncoding(encodingCodePage));
        }
    }
}
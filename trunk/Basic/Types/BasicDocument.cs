using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicDocument: IHasIdentity
    {
        public BasicDocument (int documentId, string serverFileName, string clientFileName, string description,
            DocumentType documentType, int foreignId, Int64 fileSize, int uploadedByPersonId, DateTime uploadedDateTime)
        {
            this.documentId = documentId;
            this.serverFileName = serverFileName;
            this.clientFileName = clientFileName;
            this.description = description;
            this.documentType = documentType;
            this.foreignId = foreignId;
            this.fileSize = fileSize;
            this.uploadedByPersonId = uploadedByPersonId;
            this.uploadedDateTime = uploadedDateTime;
        }

        public BasicDocument (BasicDocument original):
            this (original.documentId, original.serverFileName, original.clientFileName, original.description,
            original.documentType, original.foreignId, original.fileSize, original.uploadedByPersonId, original.uploadedDateTime)
        {
            // empty ctor apart from calling the other one
        }

        public int DocumentId
        {
            get { return this.documentId; }
        }

        public string ServerFileName
        {
            get { return this.serverFileName; }
            protected set { this.serverFileName = value; }
        }

        public string ClientFileName
        {
            get { return this.clientFileName; }
        }

        public string Description
        {
            get { return this.description; }
            protected set { this.description = value; }
        }

        public DocumentType DocumentType
        {
            get { return this.documentType; }
        }

        public int ForeignId
        {
            get { return this.foreignId; }
        }

        public Int64 FileSize
        {
            get { return this.fileSize; }
        }
  
        public int UploadedByPersonId
        {
            get { return this.uploadedByPersonId; }
        }

        public DateTime UploadedDateTime
        {
            get { return this.uploadedDateTime; }
        }

        private int documentId;
        private string serverFileName;
        private string clientFileName;
        private string description;
        private DocumentType documentType;
        private int foreignId;
        private Int64 fileSize;
        private int uploadedByPersonId;
        private DateTime uploadedDateTime;


        #region IHasIdentity Members

        public int Identity
        {
            get { return this.DocumentId; }
        }

        #endregion
    }
}

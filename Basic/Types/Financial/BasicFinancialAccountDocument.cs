using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Attributes;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    [Serializable]
    public class BasicFinancialAccountDocument: IHasIdentity
    {
        [Obsolete("This ctor is here only to enable serialization, and so this attribute prevents compile time calls", true)]
        public BasicFinancialAccountDocument()
        {
        }

        public BasicFinancialAccountDocument(int financialAccountDocumentId, int financialAccountId,
            FinancialAccountDocumentType type, DateTime uploadedDateTime, int uploadedByPersonId,
            DateTime concernsPeriodStart, DateTime concernsPeriodEnd, string rawDocumentText)
        {
            this.FinancialAccountDocumentId = financialAccountDocumentId;
            this.FinancialAccountId = financialAccountId;
            this.Type = type;
            this.UploadedDateTime = uploadedDateTime;
            this.UploadedByPersonId = uploadedByPersonId;
            this.ConcernsPeriodStart = concernsPeriodStart;
            this.ConcernsPeriodEnd = concernsPeriodEnd;
            this.RawDocumentText = rawDocumentText;
        }

        public BasicFinancialAccountDocument(BasicFinancialAccountDocument original) :
            this(original.FinancialAccountDocumentId, original.FinancialAccountId, original.Type,
                original.UploadedDateTime, original.UploadedByPersonId, original.ConcernsPeriodStart,
                original.ConcernsPeriodEnd, original.RawDocumentText)
        {
            // empty copy ctor
        }

        // Identity field

        [DbColumnName] [DbIdentity]
        public int FinancialAccountDocumentId { get; private set; }

        // Name-identical dbcolumns / properties

        [DbColumnName] public int FinancialAccountId { get; private set; }
        [DbColumnName] public DateTime UploadedDateTime { get; private set; }
        [DbColumnName] public int UploadedByPersonId { get; private set; }
        [DbColumnName] public DateTime ConcernsPeriodStart { get; private set; }
        [DbColumnName] public DateTime ConcernsPeriodEnd { get; private set; }
        [DbColumnName] public string RawDocumentText { get; private set; }

        // Complex or renamed dbcolumns / properties

        [DbColumnForeignTypeName("FinancialAccountDocumentTypes", "FinancialAccountDocumentTypeId", "Name")]
        public FinancialAccountDocumentType Type { get; private set; }


        public int Identity { get { return this.FinancialAccountDocumentId; } }
    }
}

using System;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class InboundInvoices: PluralBase<InboundInvoices,InboundInvoice,BasicInboundInvoice>
    {

        public InboundInvoices WhereAttested
        {
            get
            {
                InboundInvoices result = new InboundInvoices();
                result.AddRange(this.Where(invoice => invoice.Attested == true));

                return result;
            }
        }


        public InboundInvoices WhereUnattested
        {
            get
            {
                InboundInvoices result = new InboundInvoices();
                result.AddRange(this.Where(invoice => invoice.Attested == false));

                return result;
            }
        }


        /// <summary>
        /// Gets all open inbound invoices for a particular organization.
        /// </summary>
        /// <param name="organization">The organization.</param>
        /// <returns>The list of inbound invoices.</returns>
        public static InboundInvoices ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        /// <summary>
        /// Gets all inbound invoices for an organization, optionally including closed ones.
        /// </summary>
        /// <param name="organization">The organizations.</param>
        /// <param name="includeClosed">True to include closed records.</param>
        /// <returns>The list of inbound invoices.</returns>
        public static InboundInvoices ForOrganization (Organization organization, bool includeClosed)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetInboundInvoices(organization,
                                                             includeClosed
                                                                 ? DatabaseCondition.None
                                                                 : DatabaseCondition.OpenTrue));
        }

        public decimal TotalAmount
        {
            get
            {
                decimal result = 0.0m;

                foreach (InboundInvoice invoice in this)
                {
                    result += invoice.Amount;
                }

                return result;
            }
        }

        public Int64 TotalAmountCents
        {
            get 
            { 
                Int64 result = 0;
            
                foreach (InboundInvoice invoice in this)
                {
                    result += invoice.AmountCents;
                }

                return result;
            }
        }
    }
}

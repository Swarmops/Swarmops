namespace Swarmops.Basic.Enums
{
    public enum ObjectType
    {
        Unknown = 0,
        Person,
        FinancialAccount,
        Organization,
        OutboundInvoice,
        InboundInvoice
    }

    public enum ObjectOptionalDataType
    {
        Unknown = 0,

        /// <summary>
        ///     The personal number (or equivalent) of somebody. In SE, this is a ten digit number.
        /// </summary>
        PersonalNumber,

        /// <summary>
        ///     If a particular person is invisible to all officers.
        /// </summary>
        Anonymous,

        /// <summary>
        ///     ?
        /// </summary>
        CustomEmail,

        /// <summary>
        ///     If somebody should never ever be mailed for any reason.
        /// </summary>
        NeverMail,

        /// <summary>
        ///     The account id of the (currently just Swedish) forum.
        /// </summary>
        ForumAccountId,

        /// <summary>
        ///     Name of a bank.
        /// </summary>
        BankName,

        /// <summary>
        ///     Clearing number of a bank, usually four digits.
        /// </summary>
        BankClearing,

        /// <summary>
        ///     Account number of a bank. Does NOT include clearing number.
        /// </summary>
        BankAccount,

        /// <summary>
        ///     The party email account of a person.
        /// </summary>
        PartyEmail,

        /// <summary>
        ///     Whether this entity is unreachable by mail.
        /// </summary>
        MailUnreachable,

        /// <summary>
        ///     Whether this entity wants mail in text-only format.
        /// </summary>
        LimitMailToText,

        /// <summary>
        ///     Whether this entity wants mail in ISO-8859-1 (as opposed to UTF-8).
        /// </summary>
        LimitMailToLatin1,

        /// <summary>
        ///     ?
        /// </summary>
        ResetPasswordTicket,

        /// <summary>
        ///     Not sure how this is used
        /// </summary>
        EMailIsInvalid,

        /// <summary>
        ///     The name of the photographer, if the person has a photo (Document type)
        /// </summary>
        PortraitPhotographer,

        /// <summary>
        ///     A flag that this Person should be manually inspected
        /// </summary>
        AccountDetailsSuspect,
// ReSharper disable InconsistentNaming
        /// <summary>
        ///     T-shirt size as indicated by Person
        /// </summary>
        TShirtSize,
// ReSharper restore InconsistentNaming
        /// <summary>
        ///     Name of a blog in cleartext
        /// </summary>
        BlogName,

        /// <summary>
        ///     Url of a blog, including preceding http://
        /// </summary>
        BlogUrl,

        /// <summary>
        ///     Public key in PGP Armor format.
        /// </summary>
        CryptoPublicKey,

        /// <summary>
        ///     Secret AND public keypair in PGP Armor format.
        /// </summary>
        CryptoSecretKey,

        /// <summary>
        ///     Revocation certificate for a public key. Do not use until time for revocation.
        /// </summary>
        CryptoRevocation,

        /// <summary>
        ///     Fingerprint of a crypto key, in format "ABCD 0123 4567..."
        /// </summary>
        CryptoFingerprint,

        /// <summary>
        ///     Geography of an account
        /// </summary>
        GeographyId,

        /// <summary>
        ///     Transaction tag for a donation to go to a particular account
        /// </summary>
        BankTransactionTag,

        /// <summary>
        ///     The account name of the (currently just Swedish) forum.
        /// </summary>
        ForumAccountName,

        /// <summary>
        ///     The preferred culture (for translations etc) eg. sv-SE
        /// </summary>
        PreferredCulture,

        /// <summary>
        ///     List of functional mail addresses for organization
        /// </summary>
        OrgFunctionalMail,

        /// <summary>
        ///     A temporary place to store password hash until membership confirmed (for PPFI)
        /// </summary>
        PersonTempPasswordStore,

        /// <summary>
        ///     Flag to indicate if officer notifications are allowed to contain member names (for PPFI)
        /// </summary>
        OrgShowNamesInNotifications,

        /// <summary>
        ///     Flag to indicate if organisation is using PaymentStatus (for PPFI)
        /// </summary>
        OrgUsePaymentStatus,

        /// <summary>
        ///     Organizations: If the org has PW do bookkeeping
        /// </summary>
        OrgEconomyEnabled,

        /// <summary>
        ///     The highest year for which fiscal books are closed, or 0 for never
        /// </summary>
        OrgBooksClosedForYear,

        /// <summary>
        ///     The last year for which fiscal books have been audited, or 0 for never
        /// </summary>
        OrgBooksAuditedForYear,

        /// <summary>
        ///     Bank account for an organization's tax payments
        /// </summary>
        OrgTaxAccount,

        /// <summary>
        ///     OCR sequence on tax payments (to the tax account) for an organization
        /// </summary>
        OrgTaxOcr,

        /// <summary>
        ///     True if the organization declares inbound/outbound Value Added Tax
        /// </summary>
        OrgValueAddedTaxEnabled,
        OrgFinancialMailName,
        OrgFinancialMailAddress,

        /// <summary>
        ///     If invoice is credited, this is the credit invoice id (normative)
        /// </summary>
        OutboundInvoiceCreditedWithInvoice,

        /// <summary>
        ///     This is a credit invoice, crediting this other invoice (informative)
        /// </summary>
        OutboundInvoiceCreditsInvoice,

        /// <summary>
        ///     If invoice is to a person id, this is the person id
        /// </summary>
        OutboundInvoiceToPersonId,

        /// <summary>
        ///     If "1", this financial account may be charged for conferences
        /// </summary>
        FinancialAccountEnabledForConferences,

        /// <summary>
        ///     If "1", this financial account may be charged with expenses
        /// </summary>
        FinancialAccountEnabledForExpensing,

        /// <summary>
        ///     If "1", this financial account may be charged with invoices
        /// </summary>
        FinancialAccountEnabledForInvoicing,

        /// <summary>
        ///     Organization's operations currency, stored as currency code
        /// </summary>
        OrgCurrency,

        /// <summary>
        ///     First bookkeeping year of org in Swarmops (can set inbound balances for this year only)
        /// </summary>
        OrgFirstFiscalYear,

        /// <summary>
        ///     Temporary mechanism for the Swarmops Red release: a coarse access list (read/write access)
        /// </summary>
        OrgTemporaryAccessListWrite,

        /// <summary>
        ///     Temporary mechanism for the Swarmops Red release: a coarse access list (read-only access)
        /// </summary>
        OrgTemporaryAccessListRead,

        /// <summary>
        ///     Free-text description of an inbound invoice (what's it for?)
        /// </summary>
        InboundInvoiceDescription,

        /// <summary>
        ///     Geopos - longitude. Float in degrees. Positive is north.
        /// </summary>
        Longitude,

        /// <summary>
        ///     Geopos - latitude. Float in degrees. Positive is east.
        /// </summary>
        Latitude,

        /// <summary>
        ///     A National ID number, if any. Like US SSN.
        /// </summary>
        NationalIdNumber,

        /// <summary>
        ///     What are ordinary people called in this org?
        /// </summary>
        OrgRegularLabel,

        /// <summary>
        ///     What are activists called in this org?
        /// </summary>
        OrgActivistLabel,

        /// <summary>
        ///     For a person, the BitId address they use to login
        /// </summary>
        BitIdLoginAddress,
        
        /// <summary>
        /// For a person, the last logged-on organization id. Used to go back there on new login.
        /// </summary>
        LastLoginOrganizationId
    }
}
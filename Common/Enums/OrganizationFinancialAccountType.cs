namespace Swarmops.Common.Enums
{
    public enum OrganizationFinancialAccountType
    {
        /// <summary>
        ///     Undefined
        /// </summary>
        Unknown,

        /// <summary>
        ///     Main bank transaction account - not a savings account or similar
        /// </summary>
        AssetsBankAccountMain,

        /// <summary>
        ///     The account where outbound invoices are kept
        /// </summary>
        AssetsOutboundInvoices,

        /// <summary>
        ///     If org has a PayPal account, this is where is goes
        /// </summary>
        AssetsPaypal,

        /// <summary>
        ///     Assets held in bitcoin (if any), immediately accessible.
        /// </summary>
        AssetsBitcoinHot,

        /// <summary>
        ///     Assets held in bitcoin cold storage (if any).
        /// </summary>
        AssetsBitcoinCold,

        /// <summary>
        ///     Any outstanding cash advances that have been paid out and not cleared against receipts or paid back
        /// </summary>
        AssetsOutstandingCashAdvances,

        /// <summary>
        ///     Value Added Tax (inbound) that hasn't been declared yet and put on overall tax balance
        /// </summary>
        AssetsVatInbound,

        /// <summary>
        /// General short term holdings
        /// </summary>
        AssetsShortTerm,

        /// <summary>
        /// Internal transfers that are in progress
        /// </summary>
        AssetsTransfersInProgress,

        /// <summary>
        ///     If virtual banking is enabled, this is where the virtual assets go
        /// </summary>
        AssetsVirtualBanking,

        /// <summary>
        ///     Any expense claims on the organization that have not been paid back
        /// </summary>
        DebtsExpenseClaims,

        /// <summary>
        ///     Invoices sent to the organization until paid against bank account
        /// </summary>
        DebtsInboundInvoices,

        /// <summary>
        ///     Salaries due for payout until cleared against bank account
        /// </summary>
        DebtsSalary,

        /// <summary>
        ///     Taxes due for payout until cleared against bank account
        /// </summary>
        DebtsTax,

        /// <summary>
        ///     Equity - the difference between debt and assets
        /// </summary>
        DebtsEquity,

        /// <summary>
        ///     Undeclared Value Added Tax (outbound) before putting it on tax balance
        /// </summary>
        DebtsVatOutbound,

        /// <summary>
        ///     If virtual banking is enabled, this is the central debt to local assets
        /// </summary>
        DebtsEarmarkedVirtualBanking,

        /// <summary>
        ///     Earmarked assets in other ways
        /// </summary>
        DebtsEarmarkedOtherAssets,

        /// <summary>
        ///     Other debts in general
        /// </summary>
        DebtsOther,

        /// <summary>
        ///     Income from donations - default autodonates here
        /// </summary>
        IncomeDonations,

        /// <summary>
        ///     Income from membership fees - default membership invoices here
        /// </summary>
        IncomeMembershipDues,

        /// <summary>
        ///     Income from sales - default outbound invoices to this acct
        /// </summary>
        IncomeSales,

        /// <summary>
        ///     Gains from currency fluctuation (typically bitcoin or altcoins).
        /// </summary>
        IncomeCurrencyFluctuations,

        /// <summary>
        ///     Bank fees - default small minus posts on bank statements here
        /// </summary>
        CostsBankFees,

        /// <summary>
        /// Miner's fees on the bitcoin network
        /// </summary>
        CostsBitcoinFees,

        /// <summary>
        ///     IT and such - default SMS charges, etc, here
        /// </summary>
        CostsInfrastructure,

        /// <summary>
        ///     If virtual banking is enabled, this is a counterbalance account
        /// </summary>
        CostsLocalDonationTransfers,

        /// <summary>
        ///     No description
        /// </summary>
        CostsAllocatedFunds,

        /// <summary>
        ///     Losses from currency fluctuation (typically bitcoin or altcoins).
        /// </summary>
        CostsCurrencyFluctuations,

        /// <summary>
        ///     Various bookkeeping errors
        /// </summary>
        CostsMiscalculations,

        /// <summary>
        ///     When closing books, this is the yearly result (balanced against DebtsEquity)
        /// </summary>
        CostsYearlyResult, // multi-account type
    };
}
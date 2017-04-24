DROP PROCEDURE IF EXISTS `SetInboundInvoiceVatCents`


#


CREATE PROCEDURE `SetInboundInvoiceVatCents`(
  IN inboundInvoiceId INTEGER,
  IN vatCents BIGINT
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.VatCents=vatCents
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


ALTER TABLE `FinancialTransactions` 
ADD COLUMN `OrganizationSequenceId` INT NOT NULL DEFAULT 0 AFTER `OrganizationId`,
ADD INDEX `Index_SequenceId` (`OrganizationSequenceId` ASC)


#

DROP PROCEDURE IF EXISTS `SetFinancialTransactionOrganizationSequenceId`


#


CREATE PROCEDURE `SetFinancialTransactionOrganizationSequenceId`(
  IN financialTransactionId INTEGER
)
BEGIN

  DECLARE organizationId INTEGER;
  DECLARE earlierTransactionCount INTEGER;

  SELECT OrganizationId 
      FROM FinancialTransactions 
      WHERE FinancialTransactions.FinancialTransactionId = financialTransactionId
      INTO organizationId;

  SELECT COUNT(*)
      FROM FinancialTransactions
      WHERE FinancialTransactions.OrganizationId=organizationId
          AND FinancialTransactions.FinancialTransactionId < financialTransactionId
      INTO earlierTransactionCount;

  UPDATE FinancialTransactions SET OrganizationSequenceId = earlierTransactionCount+1
    WHERE FinancialTransactions.FinancialTransactionId = financialTransactionId;

END


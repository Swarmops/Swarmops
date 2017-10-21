DROP PROCEDURE IF EXISTS `SetVatReportOpenTransaction`

#

CREATE PROCEDURE `CreateFinancialTransactionStub`(
  IN organizationId INTEGER,
  IN financialAccountId INTEGER,
  IN amountCents BIGINT,
  IN dateTime DATETIME,
  IN comment TEXT,
  IN importHash VARCHAR(32),
  IN personId INTEGER
)
BEGIN
  
  DECLARE financialTransactionId INTEGER;

  SELECT 0 INTO financialTransactionId;

  IF ((SELECT COUNT(*) FROM FinancialTransactions WHERE FinancialTransactions.ImportHash=importHash) = 0)
  THEN
    INSERT INTO FinancialTransactions (OrganizationId, DateTime, Comment, ImportHash)
      VALUES (organizationId, dateTime, comment, importHash);

    SELECT LAST_INSERT_ID() INTO financialTransactionId;

    INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, AmountCents, CreatedDateTime, CreatedByPersonId)
      VALUES (financialAccountId, financialTransactionId, amountCents, DateTime, personId);

  END IF;

  SELECT financialTransactionId AS Identity;

END

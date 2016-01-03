DROP procedure IF EXISTS `CreateFinancialTransactionRowPrecise`


#


CREATE PROCEDURE `CreateFinancialTransactionRowPrecise`(
  IN financialTransactionId INTEGER,
  IN financialAccountId INTEGER,
  IN amountCents BIGINT,
  IN dateTime DATETIME,
  IN personId INTEGER
)
BEGIN

  INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, Amount, AmountCents, CreatedDateTime, CreatedByPersonId)
    VALUES (financialAccountId, financialTransactionid, amountCents/100.0, amountCents, dateTime, personId);

  SELECT LAST_INSERT_ID() AS Identity;

END

CREATE TABLE `FinancialTransactionRowsNativeCurrency` (
  `FinancialTransactionRowId` INT NOT NULL,
  `CurrencyId` INT NOT NULL,
  `NativeAmountCents` BIGINT NOT NULL,
  PRIMARY KEY (`FinancialTransactionRowId`))


#


DROP procedure IF EXISTS `SetFinancialTransactionRowNativeCurrency`


#



CREATE PROCEDURE `SetFinancialTransactionRowNativeCurrency`(
	IN financialTransactionRowId INT,
	IN currencyId INT,
	IN nativeAmountCents BIGINT
)
BEGIN

  IF ((SELECT COUNT(*) FROM FinancialTransactionRows
    WHERE FinancialTransactionRows.FinancialTransactionRowId=financialTransactionRowId) = 0)
  THEN

    INSERT INTO FinancialTransactionRowsNativeCurrency (FinancialTransactionRowId,CurrencyId,NativeAmountCents)
      VALUES (financialTransactionRowId,currencyId,nativeAmountCents);

  ELSE

    UPDATE FinancialTransactionRowsNativeCurrency
      SET FinancialTransactionRowsNativeCurrency.CurrencyId=currencyId,
          FinancialTransactionRowsNativeCurrency.NativeAmountCents=nativeAmountCents
      WHERE FinancialTransactionRowsNativeCurrency.FinancialTransactionRowId=
            financialTransactionRowId;

  END IF;

END

DROP procedure IF EXISTS `SetFinancialTransactionRowNativeCurrency`


#


CREATE PROCEDURE `SetFinancialTransactionRowNativeCurrency`(
	IN financialTransactionRowId INT,
	IN currencyId INT,
	IN nativeAmountCents BIGINT
)
BEGIN

  IF NOT EXISTS (SELECT * FROM FinancialTransactionRowsNativeCurrency
    WHERE FinancialTransactionRowsNativeCurrency.FinancialTransactionRowId=financialTransactionRowId)
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

ALTER TABLE `SalaryTaxLevels` 
DROP PRIMARY KEY,
ADD PRIMARY KEY (`TaxLevelId`, `BracketLow`, `CountryId`, `Year`)


#


DROP PROCEDURE `InsertExchangeRateSnapshotData`


#


CREATE PROCEDURE `CreateExchangeRateDatapoint`(
  IN exchangeRateSnapshotId INTEGER,
  IN currencyAId INTEGER,
  IN currencyBId INTEGER,
  IN aPerB DOUBLE
)
BEGIN

  INSERT INTO CurrencyExchangeRateSnapshotData (CurrencyExchangeRateSnapshotId,CurrencyAId,CurrencyBId,APerB)
    VALUES (exchangeRateSnapshotId,currencyAId,currencyBId,aPerB);

END
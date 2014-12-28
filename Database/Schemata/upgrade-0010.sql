ALTER TABLE `CurrencyExchangeRateSnapshotData` 
DROP PRIMARY KEY,
ADD PRIMARY KEY (`CurrencyExchangeRateSnapshotId`, `CurrencyBId`, `CurrencyAId`),
ADD INDEX `Index_SnapshotId` (`CurrencyExchangeRateSnapshotId` ASC),
ADD INDEX `Index_CurrencyA` (`CurrencyAId` ASC),
ADD INDEX `Index_CurrencyB` (`CurrencyBId` ASC)

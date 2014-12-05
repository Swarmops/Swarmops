ALTER TABLE `Memberships` 
ADD COLUMN `MembershipNumberLocal` INT(11) NOT NULL DEFAULT 0 AFTER `MemberSince`,
ADD COLUMN `MembershipNumberFamily` INT(11) NOT NULL DEFAULT 0 AFTER `MembershipNumberLocal`,
ADD INDEX `Index_NumLocal` (`MembershipNumberLocal` ASC),
ADD INDEX `Index_NumFamily` (`MembershipNumberFamily` ASC)

#

ALTER TABLE `Organizations` 
ADD COLUMN `GrandestparentOrganizationId` INT(11) NOT NULL DEFAULT 0 AFTER `ParentOrganizationId`,
ADD INDEX `Index_Parent` (`ParentOrganizationId` ASC),
ADD INDEX `Index_Family` (`GrandestparentOrganizationId` ASC)


#


CREATE TABLE `CurrencyExchangeRateSnapshots` (
  `CurrencyExchangeRateSnapshotId` INT NOT NULL AUTO_INCREMENT,
  `DateTime` DATETIME NOT NULL,
  PRIMARY KEY (`CurrencyExchangeRateSnapshotId`),
  INDEX `Ix_Date` (`DateTime` ASC))


#


CREATE TABLE `CurrencyExchangeRateSnapshotData` (
  `CurrencyExchangeRateSnapshotId` INT NOT NULL AUTO_INCREMENT,
  `CurrencyAId` INT(11) NOT NULL,
  `CurrencyBId` INT(11) NOT NULL,
  `APerB` DOUBLE NOT NULL,
  PRIMARY KEY (`CurrencyExchangeRateSnapshotId`));


#


CREATE PROCEDURE `CreateExchangeRateSnapshot` (
  IN dateTimeUtc DATETIME
)
BEGIN

  INSERT INTO CurrencyExchangeRateSnapshots (DateTime)
  VALUES (dateTimeUtc);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `InsertExchangeRateSnapshotData` (
  IN currencyExchangeRateSnapshotId INTEGER,
  IN currencyAId INTEGER,
  IN currencyBId INTEGER,
  IN aPerB DOUBLE
)
BEGIN

  INSERT INTO CurrencyExchangeRateSnapshotData (CurrencyExchangeRateSnapshotId,CurrencyAId,CurrencyBId,APerB)
    VALUES (currencyExchangeRateSnapshotId,currencyAId,currencyBId,aPerB);

END


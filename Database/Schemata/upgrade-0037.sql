ALTER TABLE `InboundInvoices` 
ADD COLUMN `PayToBitcoinAddress` VARCHAR(128) NOT NULL DEFAULT '' AFTER `SupplierId`,
ADD COLUMN `PayToBitcoinCashAddress` VARCHAR(128) NOT NULL DEFAULT '' AFTER `PayToBitcoinAddress`
ADD COLUMN `PayToEthereumAddress` VARCHAR(128) NOT NULL DEFAULT '' AFTER `PayToBitcoinCashAddress`

#

ALTER TABLE `People`
ADD COLUMN `PayoutSpecId` INT NOT NULL DEFAULT 0 AFTER `GenderId`

#

CREATE TABLE `ExternalContacts` (
  `ExternalContactId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL DEFAULT 0,
  `Name` VARCHAR(128) NOT NULL,
  `Phone` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(128) NOT NULL,
  `CultureId` VARCHAR(64) NOT NULL DEFAULT 'en-US',
  `ExternalContactTypeId` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`ExternalContactId`),
  INDEX `Ix_OrganizationId` (`OrganizationId` ASC),
  INDEX `Ix_TypeId` (`ExternalContactTypeId` ASC),
  INDEX `Ix_Name` (`Name` ASC))

#

CREATE TABLE `ExternalContactTypes` (
  `ExternalContactTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`ExternalContactTypeId`),
  INDEX `Ix_Name` (`Name` ASC))

#

CREATE TABLE `Suppliers` (
  `SupplierId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `Name` VARCHAR(128) NOT NULL DEFAULT '',
  `ExternalContactId` INT NOT NULL DEFAULT 0,
  `Street` VARCHAR(128) NOT NULL DEFAULT '',
  `PostalCode` VARCHAR(45) NOT NULL DEFAULT '',
  `City` VARCHAR(45) NOT NULL DEFAULT '',
  `CountryId` INT NOT NULL DEFAULT '',
  `VatId` VARCHAR(128) NOT NULL DEFAULT '',
  `PayoutSpecId` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`SupplierId`),
  INDEX `Ix_Name` (`Name` ASC),
  INDEX `Ix_Organization` (`OrganizationId` ASC))

#

CREATE TABLE `PayoutSpecs` (
  `PayoutSpecId` IN NOT NULL AUTO_INCREMENT,
  `Bank` VARCHAR(128) NOT NULL DEFAULT '',
  `ClearingRouting` VARCHAR(128) NOT NULL DEFAULT '',
  `Account` VARCHAR(128) NOT NULL DEFAULT '',
  `BitcoinAddress` VARCHAR(128) NOT NULL DEFAULT '',
  `BitcoinCashAddress` VARCHAR(128) NOT NULL DEFAULT '',
  `EthereumAddress` VARCHAR(128) NOT NULL DEFAULT '',
  PRIMARY KEY (`PayoutSpecId`))
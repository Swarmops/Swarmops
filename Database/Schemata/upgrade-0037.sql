ALTER TABLE `FinancialTransactions` 
CHANGE COLUMN `Comment` `Comment` TEXT NOT NULL

#

DROP PROCEDURE `CreateFinancialTransactionStub`

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

    INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, Amount, AmountCents, CreatedDateTime, CreatedByPersonId)
      VALUES (financialAccountId, financialTransactionId, amountCents/100.0, amountCents, DateTime, personId);

  END IF;

  SELECT financialTransactionId AS Identity;

END

#

ALTER TABLE `InboundInvoices` 
ADD COLUMN `PayoutSpecId` INT NOT NULL DEFAULT 0 AFTER `SupplierId`,

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
  `Street` VARCHAR(256) NOT NULL DEFAULT '',
  `PostalCode` VARCHAR(45) NOT NULL DEFAULT '',
  `City` VARCHAR(45) NOT NULL DEFAULT '',
  `CountryId` INT NOT NULL DEFAULT 0,
  `VatId` VARCHAR(128) NOT NULL DEFAULT '',
  `PayoutSpecId` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`SupplierId`),
  INDEX `Ix_Name` (`Name` ASC),
  INDEX `Ix_Country` (`CountryId` ASC),
  INDEX `Ix_Organization` (`OrganizationId` ASC))

#

CREATE TABLE `PayoutSpecs` (
  `PayoutSpecId` INT NOT NULL AUTO_INCREMENT,
  `BankAccountId` INT NOT NULL DEFAULT 0,
  `CryptoAddressId` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`PayoutSpecId`))

#

CREATE TABLE `BankAccountSpecs` (
  `BankSpecId` INT NOT NULL AUTO_INCREMENT,
  `BankId` INT NOT NULL,
  `Iban` VARCHAR(128) NOT NULL DEFAULT '',
  `Routing` BIGINT NOT NULL DEFAULT 0,
  `Clearing` BIGINT NOT NULL DEFAULT 0,
  `Account` BIGINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`BankSpecId`),
  INDEX `Ix_Bank` (`BankId` ASC))  

#

CREATE TABLE `Banks` (
  `BankId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  `Street` VARCHAR(256) NOT NULL,
  `PostalCode` VARCHAR(64) NOT NULL,
  `City` VARCHAR(64) NOT NULL,
  `CountryId` INT NOT NULL DEFAULT 0,
  `Bic` VARCHAR(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`BankId`),
  INDEX `Ix_Name` (`Name` ASC),
  INDEX `Ix_Country` (`CountryId` ASC),
  INDEX `Ix_Bic` (`Bic` ASC))

#

CREATE TABLE `CryptoAddressSpecs` (
  `CryptoAddressSpecId` INT NOT NULL AUTO_INCREMENT,
  `CurrencyId` INT NOT NULL,
  `Address` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`CryptoAddressSpecId`),
  INDEX `Ix_Address` (`CurrencyId` ASC, `Address` ASC))

#

ALTER TABLE `Currencies` 
CHANGE COLUMN `Code` `Code` VARCHAR(64) NOT NULL,
ADD COLUMN `IsCrypto` TINYINT NOT NULL DEFAULT 0 AFTER `Sign`,
ADD INDEX `Ix_Code` (`Code` ASC, `IsCrypto` ASC)

#

DROP PROCEDURE `CreateCurrency`

#

CREATE PROCEDURE `CreateCurrency`(
  IN name VARCHAR(64),
  IN code VARCHAR(16),
  IN sign VARCHAR(8)
)
BEGIN

  INSERT INTO Currencies (Name,Code,Sign,IsCrypto)
    VALUES (name,code,sign,0);

  SELECT LAST_INSERT_ID() AS Identity;

END

#

CREATE PROCEDURE `CreateCryptocurrency`(
  IN name VARCHAR(64),
  IN code VARCHAR(16),
  IN sign VARCHAR(8)
)
BEGIN

  INSERT INTO Currencies (Name,Code,Sign,IsCrypto)
    VALUES (name,code,sign,1);

  SELECT LAST_INSERT_ID() AS Identity;

END
  
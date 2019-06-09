CREATE TABLE `FinancialAccountDocuments` (
  `FinancialAccountDocumentId` INT NOT NULL AUTO_INCREMENT,
  `FinancialAccountId` INT NOT NULL,
  `FinancialAccountDocumentTypeId` INT NOT NULL,
  `UploadedDateTime` DATETIME NOT NULL,
  `UploadedByPersonId` INT NOT NULL,
  `ConcernsPeriodStart` DATETIME NOT NULL,
  `ConcernsPeriodEnd` DATETIME NOT NULL,
  `RawDocumentText` TEXT NOT NULL,
  PRIMARY KEY (`FinancialAccountDocumentId`),
  INDEX `ixAccountId` (`FinancialAccountId` ASC),
  INDEX `ixDocType` (`FinancialAccountDocumentTypeId` ASC),
  INDEX `ixUploadedTime` (`UploadedDateTime` ASC),
  INDEX `ixUploadedPerson` (`UploadedByPersonId` ASC),
  INDEX `ixPeriod` (`ConcernsPeriodStart` ASC))


#


CREATE TABLE `FinancialAccountDocumentTypes` (
  `FinancialAccountDocumentTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`FinancialAccountDocumentTypeId`),
  INDEX `ixName` (`Name` ASC))


#



CREATE TABLE `FinancialTransactionRowImportHashes` (
  `FinancialTransactionRowImportHashId` INT NOT NULL AUTO_INCREMENT,
  `FinancialTransactionRowId` INT NOT NULL,
  `FinancialAccountDocumentId` INT NOT NULL,
  `Sha256` VARCHAR(128) NOT NULL DEFAULT '',
  `ProvidedUniqueId` VARCHAR(512) NOT NULL DEFAULT '',
  PRIMARY KEY (`FinancialTransactionRowImportHashId`),
  INDEX `ixTransactionRow` (`FinancialTransactionRowId` ASC),
  INDEX `ixDocument` (`FinancialAccountDocumentId` ASC),
  INDEX `ixSha256` (`Sha256` ASC),
  INDEX `ixUniqueId` (`ProvidedUniqueId` ASC))


#


DROP PROCEDURE IF EXISTS `CreateFinancialAccountDocument`


#


DROP PROCEDURE IF EXISTS `SetFinancialTransactionRowProvidedUniqueId`


#


DROP PROCEDURE IF EXISTS `SetFinancialTransactionRowImportSha256`


#


CREATE PROCEDURE `CreateFinancialAccountDocument`(
  IN financialAccountId INT,
  IN financialAccountDocumentType VARCHAR(128),
  IN uploadedDateTime DATETIME,
  IN uploadedByPersonId INT,
  IN concernsPeriodStart DATETIME,
  IN concernsPeriodEnd DATETIME,
  IN rawDocumentText TEXT
)
BEGIN

  DECLARE documentTypeId INT;

  IF ((SELECT COUNT(*) FROM FinancialAccountDocumentTypes WHERE FinancialAccountDocumentTypes.Name=financialAccountDocumentType) = 0)
  THEN
    INSERT INTO FinancialAccountDocumentTypes(Name)
      VALUES (financialAccountDocumentType);
    SELECT LAST_INSERT_ID() INTO documentTypeId;
  ELSE
    SELECT FinancialAccountDocumentTypes.FinancialAccountDocumentTypeId
      INTO documentTypeId
      FROM FinancialAccountDocumentTypes
      WHERE FinancialAccountDocumentTypes.Name=financialAccountDocumentType;
  END IF;

  INSERT INTO FinancialAccountDocuments (FinancialAccountId,FinancialAccountDocumentTypeId,UploadedDateTime,UploadedByPersonId,ConcernsPeriodStart,ConcernsPeriodEnd,RawDocumentText)
    VALUES (financialAccountId,documentTypeId,uploadedDateTime,uploadedByPersonId,concernsPeriodStart,concernsPeriodEnd,rawDocumentText);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `SetFinancialTransactionRowProvidedUniqueId`(
  IN financialTransactionRowId INT,
  IN financialAccountDocumentId INT,
  IN providedUniqueId VARCHAR(512)
)
BEGIN

  DECLARE recordId INT;

  IF ((SELECT COUNT(*) FROM FinancialTransactionRowImportHashes 
     WHERE FinancialTransactionRowImportHashes.FinancialTransactionRowId=financialTransactionRowId
       AND FinancialTransactionRowImportHashes.FinancialAccountDocumentId=financialAccountDocumentId) = 0)
  THEN

    SELECT FinancialTransactionRowImportHashes.FinancialTransactionRowImportHashId 
      INTO recordId
      FROM FinancialTransactionRowImportHashes
      WHERE FinancialTransactionRowImportHashes.FinancialTransactionRowId=financialTransactionRowId
        AND FinancialTransactionRowImportHashes.FinancialAccountDocumentId=financialAccountDocumentId;

    UPDATE FinancialTransactionRowImportHashes
      SET FinancialTransactionRowImportHashes.ProvidedUniqueId=providedUniqueId
      WHERE FinancialTransactionRowImportHashes.FinancialTransactionRowImportHashId=recordId;

  ELSE

    INSERT INTO FinancialTransactionRowImportHashes (FinancialTransactionRowId,FinancialAccountDocumentId,ProvidedUniqueId)
      VALUES (financialTransactionRowId,financialAccountDocumentId,providedUniqueId);
    SELECT LAST_INSERT_ID() INTO recordId;

  END IF;

  SELECT recordId AS Identity;

END

#

CREATE PROCEDURE `SetFinancialTransactionRowImportSha256`(
  IN financialTransactionRowId INT,
  IN financialAccountDocumentId INT,
  IN sha256 VARCHAR(128)
)
BEGIN

  DECLARE recordId INT;

  IF ((SELECT COUNT(*) FROM FinancialTransactionRowImportHashes
     WHERE FinancialTransactionRowImportHashes.FinancialTransactionRowId=financialTransactionRowId
       AND FinancialTransactionRowImportHashes.FinancialAccountDocumentId=financialAccountDocumentId) = 0)
  THEN

    SELECT FinancialTransactionRowImportHashes.FinancialTransactionRowImportHashId
      INTO recordId
      FROM FinancialTransactionRowImportHashes
      WHERE FinancialTransactionRowImportHashes.FinancialTransactionId=financialTransactionId
        AND FinancialTransactionRowImportHashes.FinancialAccountDocumentId=financialAccountDocumentId;

    UPDATE FinancialTransactionRowImportHashes
      SET FinancialTransactionRowImportHashes.Sha256=sha256
      WHERE FinancialTransactionRowImportHashes.FinancialTransactionImportHashId=recordId;

  ELSE

    INSERT INTO FinancialTransactionRowImportHashes (FinancialTransactionId,FinancialAccountDocumentId,Sha256)
      VALUES (financialTransactionId,financialAccountDocumentId,sha256);
    SELECT LAST_INSERT_ID() INTO recordId;

  END IF;

  SELECT recordId AS Identity;

END


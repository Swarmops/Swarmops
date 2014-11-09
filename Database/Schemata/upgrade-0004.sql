CREATE TABLE `FinancialAccountAutomationProfiles` (
  `FinancialAccountAutomationProfileId` INT NOT NULL AUTO_INCREMENT,
  `Guid` VARCHAR(48) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `FinancialAccountAutomationProfileTypeId` INT NOT NULL,
  `DataProviderTypeId` INT NOT NULL,
  `ProvidedBy` VARCHAR(512) NOT NULL,
  `DateTimeUpdatedUtc` DATETIME NOT NULL,
  PRIMARY KEY (`FinancialAccountAutomationProfileId`),
  INDEX `Index_DataProviderTypeId` (`DataProviderTypeId` ASC),
  INDEX `Index_AutomationTypeId` (`FinancialAccountAutomationProfileTypeId` ASC),
  INDEX `Index_Updated` (`DateTimeUpdatedUtc` ASC),
  INDEX `Index_Name` (`Name` ASC))

#


CREATE TABLE `FinancialAccountAutomationTypes` (
  `FinancialAccountAutomationTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`FinancialAccountAutomationTypeId`),
  INDEX `Index_Name` (`Name` ASC))


#


CREATE TABLE `DataProviderTypes` (
  `DataProviderTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`DataProviderTypeId`),
  INDEX `Index_Name` (`Name` ASC))


#


CREATE TABLE `FinancialAccountAutomationProfileFieldSets` (
  `FinancialAccountAutomationProfileFieldSetId` INT NOT NULL AUTO_INCREMENT,
  `FinancialAccountAutomationProfileId` INT NOT NULL,
  `Culture` VARCHAR(16) NOT NULL,
  PRIMARY KEY (`FinancialAccountAutomationProfileFieldSetId`),
  INDEX `Index_Profile` (`FinancialAccountAutomationProfileId` ASC),
  INDEX `Index_Culture` (`Culture` ASC))


#


CREATE TABLE `FieldSetData` (
  `FieldSetId` INT NOT NULL,
  `FieldSetTypeId` INT NOT NULL,
  `DataKeyId` INT NOT NULL,
  `DataValue` VARCHAR(2048) NOT NULL,
  `DateTimeUpdatedUtc` DATETIME NOT NULL,
  PRIMARY KEY (`FieldSetId`, `DataKeyId`, `FieldSetTypeId`),
  INDEX `Index_Data` (`DataValue` ASC))


#


CREATE TABLE `DataKeys` (
  `DataKeyId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`DataKeyId`),
  INDEX `Index_Name` (`Name` ASC))


#


CREATE TABLE `FieldSetTypes` (
  `FieldSetTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`FieldSetTypeId`),
  INDEX `Index_Name` (`Name` ASC))


#


CREATE TABLE `FinancialAccountAutomationProfileCountries` (
  `FinancialAccountAutomationProfileId` INT NOT NULL,
  `CountryId` INT NOT NULL,
  PRIMARY KEY (`FinancialAccountAutomationProfileId`, `CountryId`))


#


CREATE TABLE `FinancialAccountAutomationProfileHistory` (
  `FinancialAccountAutomationProfileId` INT NOT NULL,
  `Xml` TEXT NOT NULL,
  `DateTimeUpdatedUtc` VARCHAR(45) NOT NULL,
  INDEX `Index_Profile` (`FinancialAccountAutomationProfileId` ASC),
  INDEX `Index_Updated` (`DateTimeUpdatedUtc` ASC))


#


CREATE TABLE `activizr-dev`.`FinancialAccountAutomationProfileVotes` (
  `FinancialAccountAutomationProfileId` INT NOT NULL,
  `CountryId` INT NOT NULL,
  `Culture` VARCHAR(16) NOT NULL,
  `Yes` TINYINT NOT NULL,
  `SwarmopsInstallationId` VARCHAR(40) NOT NULL,
  `PersonId` VARCHAR(45) NOT NULL,
  `DateTimeUtc` DATETIME NOT NULL,
  PRIMARY KEY (`FinancialAccountAutomationProfileId`, `CountryId`, `Culture`, `PersonId`, `SwarmopsInstallationId`),
  INDEX `Index_Yes` (`Yes` ASC))


#


CREATE PROCEDURE `CreateFinancialAccountAutomationProfile`(
  IN guid VARCHAR(48),
  IN name VARCHAR(128),
  IN financialAccountAutomationTypeName VARCHAR(128),
  IN dataProviderTypeName VARCHAR(128),
  IN providedBy VARCHAR(1024),
  IN dateTimeUpdatedUtc DATETIME
)
BEGIN

  DECLARE automationTypeId INTEGER;
  DECLARE providerTypeId INTEGER;

  SELECT 0 INTO automationTypeId;
  SELECT 0 INTO providerTypeId;


  IF ((SELECT COUNT(*) FROM FinancialAccountAutomationTypes WHERE FinancialAccountAutomationTypes.Name=financialAccountAutomationTypeName) = 0)
  THEN
    INSERT INTO FinancialAccountAutomationTypes (Name)
      VALUES (financialAccountAutomationTypeName);

    SELECT LAST_INSERT_ID() INTO automationTypeId;

  ELSE

    SELECT FinancialAccountAutomationTypes.FinancialAccountAutomationTypeId INTO automationTypeId FROM FinancialAccountAutomationTypes
        WHERE FinancialAccountAutomationTypes.Name=financialAccountAutomationTypeName;

  END IF;


  IF ((SELECT COUNT(*) FROM DataProviderTypes WHERE DataProviderTypes.Name=dataProviderTypeName) = 0)
  THEN
    INSERT INTO DataProviderTypes (Name)
      VALUES (dataProviderTypeName);

    SELECT LAST_INSERT_ID() INTO providerTypeId;

  ELSE

    SELECT DataProviderTypes.DataProviderTypeId INTO providerTypeId FROM DataProviderTypes
        WHERE DataProviderTypes.Name=dataProviderTypeName;

  END IF;


  INSERT INTO FinancialAccountAutomationProfiles (Guid, Name, FinancialAccountAutomationProfileTypeId, DataProviderTypeId, ProvidedBy, DateTimeUpdatedUtc)
  VALUES (guid,name,automationTypeId,providerTypeId,providedBy,dateTimeUtc);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `SetFinancialAccountAutomationProfileField`(
  IN financialAccountAutomationProfileId INT,
  IN culture VARCHAR(16),
  IN dataKey VARCHAR(128),
  IN dataValue VARCHAR(2048),
  IN dateTimeUpdatedUtc DATETIME
)
BEGIN

  DECLARE fieldSetId INTEGER;
  DECLARE dataKeyId INTEGER;
  DECLARE fieldSetTypeId INTEGER;

  SELECT 0 INTO fieldSetId;
  SELECT 0 INTO fieldSetTypeId;
  SELECT 0 INTO dataKeyId;


  IF ((SELECT COUNT(*) FROM FieldSetTypes WHERE FieldSetTypes.Name="FinancialAccountAutomationProfile") = 0)
  THEN

    INSERT INTO FieldSetTypes (Name) VALUES ("FinancialAccountAutomationProfile");
    SELECT LAST_INSERT_ID() INTO fieldSetTypeId;

  ELSE

    SELECT FieldSetTypes.FieldSetTypeId INTO fieldSetTypeId FROM FieldSetTypes
        WHERE FieldSetTypes.Name="FinancialAccountAutomationProfile";

  END IF;


  IF ((SELECT COUNT(*) FROM FinancialAccountAutomationProfileFieldSets WHERE 
      FinancialAccountAutomationProfileFieldSets.FinancialAccountAutomationProfileId=financialAccountAutomationProfileId AND
      FinancialAccountAutomationProfileFieldSets.Culture = culture) = 0)
  THEN

    INSERT INTO FinancialAccountAutomationProfileFieldSets (FinancialAccountAutomationProfileId,Culture)
      VALUES (financialAccountAutomationProfileId,culture);

    SELECT LAST_INSERT_ID() INTO fieldSetId;

  ELSE

    SELECT FinancialAccountAutomationProfileFieldSets.FinancialAccountAutomationProfileFieldSetId INTO fieldSetId FROM FinancialAccountAutomationProfileFieldSets
        WHERE FinancialAccountAutomationProfileFieldSets.FinancialAccountAutomationProfileId=financialAccountAutomationProfileId AND
              FinancialAccountAutomationProfileFieldSets.Culture=culture;

  END IF;


  IF ((SELECT COUNT(*) FROM DataKeys WHERE DataKeys.Name=dataKey) = 0)
  THEN

    INSERT INTO DataKeys (Name) VALUES (dataKey);
    SELECT LAST_INSERT_ID() INTO dataKeyId;

  ELSE

    SELECT DataKeys.DataKeyId INTO dataKeyId FROM DataKeys
        WHERE DataKeys.Name = dataKey;

  END IF;


  IF ((SELECT COUNT(*) FROM FieldSetData 
      WHERE FieldSetData.DataKeyId = dataKeyId
      AND FieldSetData.FieldSetId = fieldSetId
      AND FieldSetData.FieldSetTypeId = fieldSetTypeId) = 0)
  THEN

    INSERT INTO FieldSetData (FieldSetId,FieldSetTypeId,DataKeyId,DataValue,DateTimeUpdatedUtc)
      VALUES (fieldSetId,fieldSetTypeId,dataKeyId,dataValue,dateTimeUpdatedUtc)

  ELSE

    UPDATE FieldSetData SET DataValue=dataValue,UpdatedDateTimeUtc=updatedDateTimeUtc
      WHERE FieldSetData.FieldSetId=fieldSetId
      AND FieldSetData.FieldSetTypeId=fieldSetTypeId
      AND FieldSetData.DataKeyId=dataKeyId;

  END IF;

END


#



CREATE PROCEDURE `SetFinancialAccountAutomationProfileCountryEnabled`(
  IN financialAccountAutomationProfileId INT,
  IN countryId INT,
  IN enabled INT)
BEGIN

  IF (enabled=0)
  THEN

    DELETE FROM FinancialAccountAutomationProfileCountries
      WHERE FinancialAccountAutomationProfileCountries.FinancialAccountAutomationProfileId=financialAccountAutomationProfileId
      AND FinancialAccountAutomationProfileCountries.CountryId=countryId;

  ELSE   

    IF ((SELECT COUNT(*) FROM FinancialAccountAutomationProfileCountries 
        WHERE FinancialAccountAutomationProfileCountries.CountryId=countryId
        AND FinancialAccountAutomationProfileCountries.FinancialAccountAutomationProfileId=financialAccountAutomationProfileId) = 0)
    THEN

      INSERT INTO FinancialAccountAutomationProfileCountries(FinancialAccountAutomationProfileId,CountryId)
        VALUES (financialAccountAutomationProfileId,countryId);

    END IF;

  END IF;

END


#


CREATE PROCEDURE `SetFinancialAccountAutomationProfileHistoryEntry`(
  IN financialAccountAutomationProfileId INT,
  IN xml TEXT,
  IN dateTimeUpdatedUtc DATETIME)
BEGIN

  INSERT INTO FinancialAccountAutomationProfileHistory (FinancialAccountAuomationProfileId, Xml, DateTimeUpdatedUtc)
    VALUES (financialAccountAutomationProfileId, xml, dateTimeUpdatedUtc);

END



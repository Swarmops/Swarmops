ALTER TABLE `GeographyUpdates` 
ADD COLUMN `GeographyUpdateSourceId` INT(11) NOT NULL AFTER `GeographyUpdateTypeId`,
ADD COLUMN `CountryCode` VARCHAR(8) NOT NULL AFTER `EffectiveDateTime`,
ADD COLUMN `Guid` VARCHAR(40) NOT NULL AFTER `GeographyUpdateSourceId`,
ADD INDEX `indexGuid` (`Guid` ASC)


#


CREATE TABLE `GeographyUpdateSources` (
  `GeographyUpdateSourceId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`GeographyUpdateSourceId`),
  INDEX `indexName` (`Name` ASC))


#


ALTER TABLE `GeographyUpdateTypes` 
ADD INDEX `indexName` (`Name` ASC)


#


ALTER TABLE `Countries` 
ADD COLUMN `PostalCodeFormat` VARCHAR(32) NOT NULL DEFAULT '' AFTER `PostalCodeLength`


#


DROP PROCEDURE `CreateGeographyUpdate`


#


CREATE PROCEDURE `CreateGeographyUpdate`(
   geographyUpdateType VARCHAR(128),
   geographyUpdateSource VARCHAR(128),
   guid VARCHAR(40),
   countryCode VARCHAR(8),
   changeDataXml TEXT,
   createdDateTime DATETIME,
   effectiveDateTime DATETIME
)
BEGIN

  DECLARE geographyUpdateTypeId INTEGER;
  DECLARE geographyUpdateSourceId INTEGER;

  IF ((SELECT COUNT(*) FROM GeographyUpdateTypes WHERE GeographyUpdateTypes.Name=geographyUpdateType) = 0)
  THEN
    INSERT INTO GeographyUpdateTypes (Name) VALUES (geographyUpdateType);
    SELECT LAST_INSERT_ID() INTO geographyUpdateTypeId;
  ELSE
    SELECT GeographyUpdateTypes.GeographyUpdateTypeId INTO geographyUpdateTypeId FROM GeographyUpdateTypes
        WHERE GeographyUpdateTypes.Name=geographyUpdateType;
  END IF;

  IF ((SELECT COUNT(*) FROM GeographyUpdateSources WHERE GeographyUpdateSources.Name=geographyUpdateSource) = 0)
  THEN
    INSERT INTO GeographyUpdateSources (Name) VALUES (geographyUpdateSource);
    SELECT LAST_INSERT_ID() INTO geographyUpdateSourceId;
  ELSE
    SELECT GeographyUpdateSources.GeographyUpdateSourceId INTO geographyUpdateSourceId FROM GeographyUpdateSources
        WHERE GeographyUpdateSources.Name=geographyUpdateSource;
  END IF;

  INSERT INTO GeographyUpdates (GeographyUpdateTypeId,GeographyUpdateSourceId,Guid,CreatedDateTime,EffectiveDateTime,CountryCode,ChangeDataXml,Processed)
     VALUES (geographyUpdateTypeId,geographyUpdateSourceId,guid,createdDateTime,effectiveDateTime,countryCode,changeDataXml,0);

  SELECT LAST_INSERT_ID() As Identity;

END

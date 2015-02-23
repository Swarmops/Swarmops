ALTER TABLE `PositionTypes` 
CHANGE COLUMN `PositionTypeId` `PositionTypeId` INT(11) UNSIGNED NOT NULL,
CHANGE COLUMN `Name` `Name` VARCHAR(128) NOT NULL


#


CREATE TABLE `PositionTitles` (
  `PositionTitleId` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`PositionTitleId`),
  KEY `Index_Name` (`Name`)
)


#


ALTER TABLE `PositionsAdditional` 
ADD COLUMN `PositionTitleId` INT NOT NULL AFTER `PositionTypeId`,
CHANGE COLUMN `Active` `Active` TINYINT(4) NOT NULL DEFAULT '1' AFTER `InheritsDownward`,
ADD COLUMN `Overridable` TINYINT NOT NULL AFTER `Volunteerable`


#


ALTER TABLE `PositionsStandard` 
ADD COLUMN `PositionTitleId` INT NOT NULL AFTER `PositionTypeId`


#


ALTER TABLE `PositionAssignments` 
CHANGE COLUMN `ExpiresDateTimeUtc` `ExpiresDateTimeUtc` DATETIME NOT NULL AFTER `Active`


#


DROP procedure IF EXISTS `CreateAdditionalPosition`


#


CREATE PROCEDURE `CreateAdditionalPosition`(
  IN organizationId INT,
  IN geographyId INT,
  IN positionType VARCHAR(128),
  IN positionTitle VARCHAR(128),
  IN overridesHigherPositionId INT,
  IN createdByPersonId INT,
  IN createdDateTimeUtc DATETIME,
  IN volunteerable TINYINT,
  IN overridable TINYINT,
  IN inheritsDownward TINYINT,
  IN reportsToStandardPositionId INT,
  IN reportsToAdditionalPositionId INT,
  IN dotReportsToStandardPositionId INT,
  IN minCount INT,
  IN maxCount INT
)
BEGIN

  DECLARE positionTypeId INTEGER;
  DECLARE positionTitleId INTEGER;

  IF ((SELECT COUNT(*) FROM PositionTitles WHERE PositionTitles.Name=positionTitle) = 0)
  THEN
    INSERT INTO PositionTitles (Name)
      VALUES (positionTitle);

    SELECT LAST_INSERT_ID() INTO positionTitleId;

  ELSE

    SELECT PositionTitles.PositionTitleId INTO positionTitleId FROM PositionTitles
        WHERE PositionTitles.Name=positionTitle;

  END IF;

  IF ((SELECT COUNT(*) FROM PositionTypes WHERE PositionTypes.Name=positionType) = 0)
  THEN
    INSERT INTO PositionTypes (Name)
      VALUES (positionType);

    SELECT LAST_INSERT_ID() INTO positionTypeId;

  ELSE

    SELECT PositionTypes.PositionTypeId INTO positionTypeId FROM PositionTypes
        WHERE PositionTypes.Name=positionType;

  END IF;

  INSERT INTO PositionsAdditional (OrganizationId,GeographyId,OverridesHigherPositionId,CreatedByPersonId,CreatedDateTimeUtc,PositionTypeId,PositionTitleId,InheritsDownward,Volunteerable,Overridable,ReportsToStandardPositionId,ReportsToAdditionalPositionId,DotReportsToStandardPositionId,MinCount,MaxCount)
    VALUES (organizationId,geographyId,overridesHigherPositionId,createdByPersonId,createdDateTimeUtc,positionTypeId,positionTitleId,inheritsDownward,volunteerable,overridable,reportsToStandardPositionId,reportsToAdditionalPositionId,dotReportsToStandardPositionId,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `CreateStandardPosition`


#


CREATE PROCEDURE `CreateStandardPosition`(
  IN organizationId INT,
  IN positionLevel INT,
  IN positionType VARCHAR(128),
  IN positionTitle VARCHAR(128),
  IN volunteerable TINYINT,
  IN overridable TINYINT,
  IN reportsToStandardPositionId INT,
  IN dotReportsToStandardPositionId INT,
  IN minCount INT,
  IN maxCount INT
)
BEGIN

  DECLARE positionTypeId INTEGER;
  DECLARE positionTitleId INTEGER;

  IF ((SELECT COUNT(*) FROM PositionTitles WHERE PositionTitles.Name=positionTitle) = 0)
  THEN
    INSERT INTO PositionTitles (Name)
      VALUES (positionTitle);

    SELECT LAST_INSERT_ID() INTO positionTitleId;

  ELSE

    SELECT PositionTitles.PositionTitleId INTO positionTitleId FROM PositionTitles
        WHERE PositionTitles.Name=positionTitle;

  END IF;

  IF ((SELECT COUNT(*) FROM PositionTypes WHERE PositionTypes.Name=positionType) = 0)
  THEN
    INSERT INTO PositionTypes (Name)
      VALUES (positionType);

    SELECT LAST_INSERT_ID() INTO positionTypeId;

  ELSE

    SELECT PositionTypes.PositionTypeId INTO positionTypeId FROM PositionTypes
        WHERE PositionTypes.Name=positionType;

  END IF;

  INSERT INTO PositionsStandard (OrganizationId,PositionLevel,PositionTypeId,PositionTitleId,Volunteerable,Overridable,ReportsToStandardPositionId,DotReportsToStandardPositionId,MinCount,MaxCount)
    VALUES (organizationId,positionLevel,positionTypeId,positionTitleId,volunteerable,overridable,reportsToStandardPositionId,dotReportsToStandardPositionId,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


DROP TABLE IF EXISTS `PositionsStandard`


#


DROP TABLE IF EXISTS `Positions`


#


ALTER TABLE `PositionsAdditional` 
DROP COLUMN `ReportsToAdditionalPositionId`,
CHANGE COLUMN `AdditionalPositionId` `PositionId` INT(11) NOT NULL AUTO_INCREMENT,
CHANGE COLUMN `ReportsToStandardPositionId` `ReportsToPositionId` INT(11) NOT NULL,
ADD COLUMN `PositionLevel` INT NOT NULL AFTER `PositionId`,
ADD COLUMN `CreatedByPositionId` INT NOT NULL AFTER `CreatedByPersonId`,
DROP INDEX `Index_Report3`,
ADD INDEX `Index_Report2` (`DotReportsToPositionId` ASC),
ADD INDEX `Index_Level` (`PositionLevel` ASC),
DROP INDEX `Index_Report2`,
RENAME TO  `Positions`


#


DROP procedure IF EXISTS `CreateAdditionalPosition`;


#


DROP procedure IF EXISTS `CreateStandardPosition`;


#


DROP procedure IF EXISTS `CreatePosition`;


#




CREATE PROCEDURE `CreatePosition`(
  IN positionLevel INT,
  IN organizationId INT,
  IN geographyId INT,
  IN overridesHigherPositionId INT,

  IN createdByPersonId INT,
  IN createdByPositionId INT,
  IN createdDateTimeUtc DATETIME,
  IN positionType VARCHAR(128),
  IN positionTitle VARCHAR(128),

  IN inheritsDownward TINYINT,
  -- active defaults to true
  IN volunteerable TINYINT,
  IN overridable TINYINT,
  -- covert defaults to false

  IN reportsToPositionId INT,
  IN dotReportsToPositionId INT,
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

  INSERT INTO Positions (PositionLevel,OrganizationId,GeographyId,OverridesHigherPositionId,CreatedByPersonId,CreatedByPositionId,CreatedDateTimeUtc,PositionTypeId,PositionTitleId,InheritsDownward,Volunteerable,Overridable,ReportsToPositionId,DotReportsToPositionId,MinCount,MaxCount)
    VALUES (positionLevel,organizationId,geographyId,overridesHigherPositionId,createdByPersonId,createdByPositionId,createdDateTimeUtc,positionTypeId,positionTitleId,inheritsDownward,volunteerable,overridable,reportsToPositionId,dotReportsToPositionId,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `SetStandardPositionActive`


#


DROP procedure IF EXISTS `SetAdditionalPositionActive`


#


DROP procedure IF EXISTS `SetPositionActive`


#


CREATE PROCEDURE `SetPositionActive`(
  IN positionId INT,
  IN active TINYINT
)
BEGIN

  UPDATE Positions 
    SET 
      Positions.Active=active 
    WHERE 
      Positions.PositionId=positionId;

END


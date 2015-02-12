ALTER TABLE `RolesAdditional` 
CHANGE COLUMN `AdditionalRoleId` `AdditionalPositionId` INT(11) NOT NULL AUTO_INCREMENT ,
CHANGE COLUMN `OverridesHigherRoleId` `OverridesHigherPositionId` INT(11) NOT NULL ,
CHANGE COLUMN `RoleTypeId` `PositionTypeId` INT(11) NOT NULL ,
CHANGE COLUMN `ReportsToDefaultRoleId` `ReportsToStandardPositionId` INT(11) NOT NULL ,
CHANGE COLUMN `ReportsToAdditionalRoleId` `ReportsToAdditionalPositionId` INT(11) NOT NULL ,
CHANGE COLUMN `DotReportsToRoleId` `DotReportsToPositionId` INT(11) NOT NULL ,
RENAME TO `PositionsAdditional`


#


ALTER TABLE `RoleTypes` 
CHANGE COLUMN `RoleTypeId` `PositionTypeId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT ,
RENAME TO  `PositionTypes`


#



ALTER TABLE `RolesDefault` 
CHANGE COLUMN `DefaultRoleId` `StandardPositionId` INT(11) NOT NULL AUTO_INCREMENT ,
CHANGE COLUMN `RoleLevel` `PositionLevel` INT(11) NOT NULL ,
CHANGE COLUMN `RoleTypeId` `PositionTypeId` INT(11) NOT NULL ,
CHANGE COLUMN `ReportsToDefaultRoleId` `ReportsToStandardPositionId` INT(11) NOT NULL ,
CHANGE COLUMN `DotReportsToDefaultRoleId` `DotReportsToStandardPositionId` INT(11) NOT NULL ,
RENAME TO `PositionsStandard`


#


ALTER TABLE `RolesAssigned` 
CHANGE COLUMN `AssignedRoleId` `PositionAssignmentId` INT(11) NOT NULL AUTO_INCREMENT ,
CHANGE COLUMN `RoleId` `PositionId` INT(11) NOT NULL ,
CHANGE COLUMN `CreatedByRoleId` `CreatedByPositionId` INT(11) NOT NULL ,
CHANGE COLUMN `TerminatedByRoleId` `TerminatedByPositionId` INT(11) NOT NULL DEFAULT '0' ,
DROP INDEX `Index_Role` ,
ADD INDEX `Index_Pos` (`PositionId` ASC), 
RENAME TO `PositionAssignments`


#


DROP procedure IF EXISTS `TerminateAssignedRole`


#


CREATE PROCEDURE `TerminatePositionAssignment`(
  IN positionAssignmentId INT,
  IN terminatedDateTimeUtc DATETIME,
  IN terminatedByPersonId INT,
  IN terminatedByPositionId INT,
  IN terminationNotes TEXT
)
BEGIN

  UPDATE PositionAssignments
    SET
      PositionAssignments.Active=0, 
      PositionAssignments.TerminatedDateTimeUtc=terminatedDateTimeUtc, 
      PositionAssignments.TerminatedByPersonId=terminatedByPersonId,
      PositionAssignments.TerminatedByPositionId=terminatedByPositionId,
      PositionAssignments.TerminationNotes=terminationNotes
    WHERE
      PositionAssignments.PositionAssignmentId=positionAssignmentId;

END


#


DROP procedure IF EXISTS `SetDefaultRoleActive`


#


CREATE PROCEDURE `SetStandardPositionActive`(
  IN standardPositionId INT,
  IN active TINYINT
)
BEGIN

  UPDATE PositionsStandard 
    SET 
      PositionsStandard.Active=active 
    WHERE 
      PositionsStandard.StandardPositionId=standardPositionId;

END


#


DROP procedure IF EXISTS `SetAdditionalRoleActive`


#


CREATE PROCEDURE `SetAdditionalPositionActive`(
  IN additionalPositionId INT,
  IN active TINYINT
)
BEGIN

  UPDATE PositionsAdditional 
    SET
      PositionsAdditional.Active=active 
    WHERE 
      PositionsAdditional.AdditionalPositionId=additionalPositionId;

END


#


DROP procedure IF EXISTS `SetAssignedRoleActing`


#


CREATE PROCEDURE `SetPositionAssignmentActing`(
  IN positionAssignmentId INT,
  IN acting TINYINT
)
BEGIN

  UPDATE PositionAssignments
    SET
      PositionAssignments.Acting=acting 
    WHERE 
      PositionAssignments.PositionAssignmentId=positionAssignmentId;

END


#



DROP procedure IF EXISTS `CreateDefaultRole`


#


CREATE PROCEDURE `CreateStandardPosition`(
  IN organizationId INT,
  IN positionLevel INT,
  IN positionType VARCHAR(64),
  IN volunteerable TINYINT,
  IN overridable TINYINT,
  IN reportsToStandardPositionId INT,
  IN dotReportsToStandardPositionId INT,
  IN minCount INT,
  IN maxCount INT
)
BEGIN

  DECLARE positionTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM PositionTypes WHERE PositionTypes.Name=positionType) = 0)
  THEN
    INSERT INTO PositionTypes (Name)
      VALUES (positionType);

    SELECT LAST_INSERT_ID() INTO positionTypeId;

  ELSE

    SELECT PositionTypes.PositionTypeId INTO positionTypeId FROM PositionTypes
        WHERE PositionTypes.Name=positionType;

  END IF;

  INSERT INTO PositionsStandard (OrganizationId,PositionLevel,PositionTypeId,Volunteerable,Overridable,ReportsToStandardPositionId,DotReportsToStandardPositionId,MinCount,MaxCount)
    VALUES (organizationId,positionLevel,positionTypeId,volunteerable,overridable,reportsToStandardPositionId,dotReportsToStandardPositionId,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `CreateAdditionalRole`


#


CREATE PROCEDURE `CreateAdditionalPosition`(
  IN organizationId INT,
  IN geographyId INT,
  IN positionType VARCHAR(64),
  IN overridesStandardPositionId INT,
  IN volunteerable TINYINT,
  IN inheritsDownward TINYINT,
  IN reportsToStandardPositionId INT,
  IN reportsToAdditionalPositionId INT,
  IN dotReportsToStandardPositionId INT,
  IN createdByPersonId INT,
  IN createdDateTimeUtc DATETIME,
  IN minCount INT,
  IN maxCount INT
)
BEGIN

  DECLARE positionTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM PositionTypes WHERE PositionTypes.Name=positionType) = 0)
  THEN
    INSERT INTO PositionTypes (Name)
      VALUES (positionType);

    SELECT LAST_INSERT_ID() INTO positionTypeId;

  ELSE

    SELECT PositionTypes.PositionTypeId INTO positionTypeId FROM PositionTypes
        WHERE PositionTypes.Name=positionType;

  END IF;

  INSERT INTO PositionsAdditional (OrganizationId,GeographyId,PositionTypeId,Volunteerable,InheritsDownward,OverridesStandardPositionId,ReportsToStandardPositionId,ReportsToAdditionalPositionId,DotReportsToStandardPositionId,CreatedByPersonId,CreatedDateTimeUtc,MinCount,MaxCount)
    VALUES (organizationId,geographyId,positionTypeId,volunteerable,inheritsDownward,overridesStandardPositionId,reportsToStandardPositionId,reportsToAdditionalPositionId,dotReportsToStandardPositionId,createdByPersonId,createdDateTimeUtc,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END



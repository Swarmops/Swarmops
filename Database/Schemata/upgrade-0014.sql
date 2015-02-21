DROP procedure IF EXISTS `CreateAssignedRole`


#


DROP procedure IF EXISTS `CreatePositionAssignment`


#


CREATE PROCEDURE `CreatePositionAssignment`(
  IN organizationId INT,
  IN geographyId INT,
  IN positionId INT,
  IN personId INT,
  IN createdDateTimeUtc DATETIME,
  IN createdByPersonId INT,
  IN createdByPositionId INT,
  IN expiresDateTimeUtc DATETIME,
  IN assignmentNotes TEXT
)
BEGIN

  INSERT INTO PositionAssignments (OrganizationId,GeographyId,PositionId,PersonId,ExpiresDateTimeUtc,CreatedDateTimeUtc,CreatedByPersonId,CreatedByPositionId,AssignmentNotes)
    VALUES (organizationId, geographyId, positionId, personId, expiresDateTimeUtc, createdDateTimeUtc, createdByPersonId, createdByPositionId, assignmentNotes);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


ALTER TABLE `PositionTypes` 
CHANGE COLUMN `PositionTypeId` `PositionTypeId` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT




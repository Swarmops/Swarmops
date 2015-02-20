DROP procedure IF EXISTS `CreateAssignedRole`


#


DROP procedure IF EXISTS `CreatePositionAssignment`


#


CREATE PROCEDURE `CreatePositionAssignment`(
  IN organizationId INT,
  IN geographyId INT,
  IN positionId INT,
  IN personId INT,
  IN expiresDateTimeUtc DATETIME,
  IN createdDateTime DATETIME,
  IN createdByPersonId INT,
  IN createdByPositionId INT,
  IN assignmentNotes TEXT
)
BEGIN

  INSERT INTO PositionsAssigned (OrganizationId,GeographyId,PositionId,PersonId,ExpiresDateTimeUtc,CreatedDateTime,CreatedByPersonId,CreatedByPositionId,AssignmentNotes)
    VALUES (organizationId, geographyId, positionId, personId, expiresDateTimeUtc, createdDateTime, createdByPersonId, createdByPositionId, assignmentNotes);

  SELECT LAST_INSERT_ID() AS Identity;

END

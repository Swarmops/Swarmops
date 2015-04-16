DROP procedure IF EXISTS `TerminatePositionAssignment`

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
      PositionAssignments.PositionAssignmentId=positionAssignmentId AND
      PositionAssignments.Active=1;
      
  SELECT ROW_COUNT() AS RowsUpdated;

END

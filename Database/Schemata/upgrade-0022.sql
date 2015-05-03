CREATE TABLE `VolunteerPositions` (
  `VolunteerPositionId` INT NOT NULL AUTO_INCREMENT,
  `VolunteerId` INT NOT NULL,
  `PositionId` INT NOT NULL,
  `GeographyId` INT NOT NULL,
  `Open` TINYINT NOT NULL DEFAULT '1',
  `Assigned` TINYINT NOT NULL DEFAULT '0',
  PRIMARY KEY (`VolunteerPositionId`),
  INDEX `Index_Position` (`PositionId` ASC),
  INDEX `Index_Geography` (`GeographyId` ASC),
  INDEX `Index_Open` (`Open` ASC),
  INDEX `Index_Volunteer` (`VolunteerId` ASC))


#


DROP procedure IF EXISTS `CreateVolunteerPosition`


#

CREATE PROCEDURE `CreateVolunteerPosition`(
  IN volunteerId INT,
  IN positionId INT,
  IN geographyId INT
)
BEGIN

  INSERT INTO VolunteerPositions (VolunteerId,PositionId,GeographyId,Open,Assigned)
  VALUES (volunteerId,positionId,geographyId,1,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `CloseVolunteerPosition`


#


CREATE PROCEDURE `CloseVolunteerPosition`(
  volunteerPositionId INTEGER,
  assigned TINYINT
)
BEGIN

  UPDATE VolunteerPositions
    SET VolunteerPositions.Open=0,VolunteerRoles.Assigned=assigned 
    WHERE VolunteerPositions.VolunteerPositionId=volunteerPositionId AND VolunteerPositions.Open=1;

  SELECT ROW_COUNT() AS RowsUpdated;


END
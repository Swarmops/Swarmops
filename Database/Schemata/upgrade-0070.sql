CREATE TABLE `Applications` (
  `ApplicationId` INT NOT NULL AUTO_INCREMENT,
  `PersonId` INT NOT NULL,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `GrantedDateTime` DATETIME NOT NULL DEFAULT '1800-01-01',
  `GrantedByPersonId` INT NOT NULL DEFAULT 0,
  `Open` TINYINT NOT NULL DEFAULT 1,
  `Score1` BIGINT NOT NULL DEFAULT 0,
  `Score2` BIGINT NOT NULL DEFAULT 0,
  `Score3` BIGINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`ApplicationId`),
  INDEX `Ix_Person` (`PersonId` ASC),
  INDEX `Ix_Organization` (`OrganizationId` ASC),
  INDEX `Ix_Scores` (`Score1` ASC, `Score2` ASC, `Score3` ASC),
  INDEX `Ix_Open` (`Open` ASC),
  INDEX `Ix_Created` (`CreatedDateTime` ASC))


#


DROP PROCEDURE IF EXISTS `CreateApplication`


#


DROP PROCEDURE IF EXISTS `UpdateApplicationScore`


#


CREATE PROCEDURE `CreateApplication`(
  IN personId INT,
  IN organizationId INT,
  IN dateTimeNow DATETIME
)
BEGIN

  INSERT INTO Applications (PersonId, OrganizationId, CreatedDateTime)
  VALUES (personId, organizationId, dateTimeNow);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `UpdateApplicationScore`(
  IN applicationId INT,
  IN deltaScore1 BIGINT,
  IN deltaScore2 BIGINT,
  IN deltaScore3 BIGINT
)

BEGIN

  UPDATE Applications
    SET Applications.Score1 = Applications.Score1 + deltaScore1,
        Applications.Score2 = Applications.Score2 + deltaScore2,
        Applications.Score3 = Applications.Score3 + deltaScore3
    WHERE Applications.ApplicationId = applicationId;

END

DROP TABLE `Applications`

#

CREATE TABLE `Applicants` (
  `ApplicantId` INT NOT NULL AUTO_INCREMENT,
  `PersonId` INT NOT NULL,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `GrantedDateTime` DATETIME NOT NULL DEFAULT '1800-01-01',
  `GrantedByPersonId` INT NOT NULL DEFAULT 0,
  `Open` TINYINT NOT NULL DEFAULT 1,
  `Score1` BIGINT NOT NULL DEFAULT 0,
  `Score2` BIGINT NOT NULL DEFAULT 0,
  `Score3` BIGINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`ApplicantId`),
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



DROP PROCEDURE IF EXISTS `CreateApplicant`


#


DROP PROCEDURE IF EXISTS `UpdateApplicantScore`


#



CREATE PROCEDURE `CreateApplicant`(
  IN personId INT,
  IN organizationId INT,
  IN dateTimeNow DATETIME
)
BEGIN

  INSERT INTO Applicants (PersonId, OrganizationId, CreatedDateTime)
  VALUES (personId, organizationId, dateTimeNow);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `UpdateApplicantScore`(
  IN applicantId INT,
  IN deltaScore1 BIGINT,
  IN deltaScore2 BIGINT,
  IN deltaScore3 BIGINT
)

BEGIN

  UPDATE Applicants
    SET Applicants.Score1 = Applicants.Score1 + deltaScore1,
        Applicants.Score2 = Applicants.Score2 + deltaScore2,
        Applicants.Score3 = Applicants.Score3 + deltaScore3
    WHERE Applicants.ApplicantId = applicantId;

END

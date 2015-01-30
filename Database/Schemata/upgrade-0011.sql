CREATE TABLE `RolesDefault` (
  `DefaultRoleId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `RoleLevel` INT NOT NULL,
  `RoleTypeId` INT NOT NULL,
  `Active` TINYINT NOT NULL DEFAULT 1,
  `Volunteerable` TINYINT NOT NULL,
  `Overridable` TINYINT NOT NULL,
  `Covert` TINYINT NOT NULL DEFAULT 0,
  `ReportsToDefaultRoleId` INT NOT NULL,
  `DotReportsToDefaultRoleId` INT NOT NULL,
  `MinCount` INT NOT NULL,
  `MaxCount` INT NOT NULL,
  PRIMARY KEY (`DefaultRoleId`),
  INDEX `Index_Org` (`OrganizationId` ASC),
  INDEX `Index_Level` (`RoleLevel` ASC),
  INDEX `Index_Type` (`RoleTypeId` ASC),
  INDEX `Index_Reports1` (`ReportsToDefaultRoleId` ASC),
  INDEX `Index_Reports2` (`DotReportsToDefaultRoleId` ASC))


#


CREATE TABLE `RolesAdditional` (
  `AdditionalRoleId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `GeographyId` INT NOT NULL,
  `OverridesHigherRoleId` INT NOT NULL,
  `CreatedByPersonId` INT NOT NULL,
  `CreatedDateTimeUtc` DATETIME NOT NULL,
  `RoleTypeId` INT NOT NULL,
  `InheritsDownward` TINYINT NOT NULL,
  `Volunteerable` TINYINT NOT NULL,
  `Active` TINYINT NOT NULL DEFAULT 1,
  `Covert` TINYINT NOT NULL DEFAULT 0,
  `ReportsToDefaultRoleId` INT NOT NULL,
  `ReportsToAdditionalRoleId` INT NOT NULL,
  `DotReportsToRoleId` INT NOT NULL,
  `MinCount` INT NOT NULL,
  `MaxCount` INT NOT NULL,
  PRIMARY KEY (`AdditionalRoleId`),
  INDEX `Index_Org` (`OrganizationId` ASC),
  INDEX `Index_Geo` (`GeographyId` ASC),
  INDEX `Index_Report1` (`ReportsToDefaultRoleId` ASC),
  INDEX `Index_Report2` (`ReportsToAdditionalRoleId` ASC),
  INDEX `Index_Report3` (`DotReportsToRoleId` ASC))


#


CREATE TABLE `RoleTypes` (
  `RoleTypeId` INT unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  INDEX `Index_Name` (`Name` ASC),
  PRIMARY KEY (`RoleTypeId`))


#


CREATE TABLE `RolesAssigned` (
  `AssignedRoleId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `GeographyId` INT NOT NULL,
  `RoleId` INT NOT NULL,
  `PersonId` INT NOT NULL,
  `ExpiresDateTimeUtc` DATETIME NOT NULL,
  `CreatedDateTimeUtc` DATETIME NOT NULL,
  `CreatedByPersonId` INT NOT NULL,
  `CreatedByRoleId` INT NOT NULL,
  `Active` TINYINT NOT NULL DEFAULT 1,
  `Acting` TINYINT NOT NULL DEFAULT 0,
  `TerminatedDateTimeUtc` DATETIME NOT NULL DEFAULT '1800-01-01',
  `TerminatedByPersonId` INT NOT NULL DEFAULT 0,
  `TerminatedByRoleId` INT NOT NULL DEFAULT 0,
  `AssignmentNotes` TEXT NOT NULL,
  `TerminationNotes` TEXT NOT NULL DEFAULT '',
  PRIMARY KEY (`AssignedRoleId`),
  INDEX `Index_Org` (`OrganizationId` ASC),
  INDEX `Index_Geo` (`GeographyId` ASC),
  INDEX `Index_Person` (`PersonId` ASC),
  INDEX `Index_Active` (`Active` ASC),
  INDEX `Index_Role` (`RoleId` ASC))


#


CREATE PROCEDURE `CreateDefaultRole` (
  IN organizationId INT,
  IN roleLevel INT,
  IN roleType VARCHAR(64),
  IN volunteerable TINYINT,
  IN overridable TINYINT,
  IN reportsToDefaultRoleId INT,
  IN dotReportsToDefaultRoleId INT,
  IN minCount INT,
  IN maxCount INT
)
BEGIN

  DECLARE roleTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM RoleTypes WHERE RoleTypes.Name=roleType) = 0)
  THEN
    INSERT INTO RoleTypes (Name)
      VALUES (roleType);

    SELECT LAST_INSERT_ID() INTO roleTypeId;

  ELSE

    SELECT RoleTypes.RoleTypeId INTO roleTypeId FROM RoleTypes
        WHERE RoleTypes.Name=roleType;

  END IF;

  INSERT INTO RolesDefault (OrganizationId,RoleLevel,RoleTypeId,Volunteerable,Overridable,ReportsToDefaultRoleId,DotReportsToDefaultRoleId,MinCount,MaxCount)
    VALUES (organizationId,roleLevel,roleTypeId,volunteerable,overridable,reportsToDefaultRoleId,dotReportsToDefaultRoleId,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END



#


CREATE PROCEDURE `SetDefaultRoleActive` (
  IN defaultRoleId INT,
  IN active TINYINT
)
BEGIN

  UPDATE RolesDefault 
    SET 
      RolesDefault.Active=active 
    WHERE 
      RolesDefault.DefaultRoleId=defaultRoleId;

END


#


CREATE PROCEDURE `CreateAdditionalRole` (
  IN organizationId INT,
  IN geographyId INT,
  IN roleType VARCHAR(64),
  IN overridesDefaultRoleId INT,
  IN volunteerable TINYINT,
  IN inheritsDownward TINYINT,
  IN reportsToDefaultRoleId INT,
  IN reportsToAdditionalRoleId INT,
  IN dotReportsToDefaultRoleId INT,
  IN createdByPersonId INT,
  IN createdDateTimeUtc DATETIME,
  IN minCount INT,
  IN maxCount INT
)

BEGIN

  DECLARE roleTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM RoleTypes WHERE RoleTypes.Name=roleType) = 0)
  THEN
    INSERT INTO RoleTypes (Name)
      VALUES (roleType);

    SELECT LAST_INSERT_ID() INTO roleTypeId;

  ELSE

    SELECT RoleTypes.RoleTypeId INTO roleTypeId FROM RoleTypes
        WHERE RoleTypes.Name=roleType;

  END IF;

  INSERT INTO RolesAdditional (OrganizationId,GeographyId,RoleTypeId,Volunteerable,InheritsDownward,OverridesDefaultRoleId,ReportsToDefaultRoleId,ReportsToAdditionalRoleId,DotReportsToDefaultRoleId,CreatedByPersonId,CreatedDateTimeUtc,MinCount,MaxCount)
    VALUES (organizationId,geographyId,roleTypeId,volunteerable,inheritsDownward,overridesDefaultRoleId,reportsToDefaultRoleId,reportsToAdditionalRoleId,dotReportsToDefaultRoleId,createdByPersonId,createdDateTimeUtc,minCount,maxCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `SetAdditionalRoleActive` (
  IN additionalRoleId INT,
  IN active TINYINT
)
BEGIN

  UPDATE RolesAdditional 
    SET
      RolesAdditional.Active=active 
    WHERE 
      RolesAdditional.AdditionalRoleId=additionalRoleId;

END


#



CREATE PROCEDURE `CreateAssignedRole` (
  IN organizationId INT,
  IN geographyId INT,
  IN roleId INT,
  IN personId INT,
  IN expiresDateTimeUtc DATETIME,
  IN createdDateTime DATETIME,
  IN createdByPersonId INT,
  IN createdByRoleId INT,
  IN assignmentNotes TEXT
)
BEGIN

  INSERT INTO RolesAssigned (OrganizationId,GeographyId,RoleId,PersonId,ExpiresDateTimeUtc,CreatedDateTime,CreatedByPersonId,CreatedByRoleId,AssignmentNotes)
    VALUES (organizationId, geographyId, roleId, personId, expiresDateTimeUtc, createdDateTime, createdByPersonId, createdByRoleId, assignmentNotes);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `SetAssignedRoleActing` (
  IN assignedRoleId INT,
  IN acting TINYINT
)
BEGIN

  UPDATE RolesAssigned
    SET
      RolesAssigned.Acting=acting 
    WHERE 
      RolesAssigned.AssignedRoleId=assignedRoleId;

END


#



CREATE PROCEDURE `TerminateAssignedRole` (
  IN assignedRoleId INT,
  IN terminatedDateTimeUtc DATETIME,
  IN terminatedByPersonId INT,
  IN terminatedByRoleId INT,
  IN terminationNotes TEXT
)
BEGIN

  UPDATE RolesAssigned 
    SET
      RolesAssigned.Active=0, 
      RolesAssigned.TerminatedDateTimeUtc=terminatedDateTimeUtc, 
      RolesAssigned.TerminatedByPersonId=terminatedByPersonId,
      RolesAssigned.TerminatedByRoleId=terminatedByRoleId,
      RolesAssigned.TerminationNotes=terminationNotes
    WHERE
      RolesAssigned.AssignedRoleId=assignedRoleId;

END


CREATE TABLE `BackendServiceOrders` (
  `BackendServiceOrderId` INT NOT NULL,
  `CreatedDateTime` INT NOT NULL,
  `OrganizationId` INT NOT NULL,
  `PersonId` INT NOT NULL,
  `BackendServiceClassId` INT NOT NULL,
  `OrderXml` TEXT NOT NULL,
  `Open` TINYINT NOT NULL DEFAULT 1,
  `Active` TINYINT NOT NULL DEFAULT 0,
  `StartedDateTime` DATETIME NOT NULL DEFAULT '1800-01-01',
  `ClosedDateTime` DATETIME NOT NULL DEFAULT '1800-01-01',
  `ExceptionText` TEXT NOT NULL DEFAULT '',
  PRIMARY KEY (`BackendServiceOrderId`),
  INDEX `Ix_Created` (`CreatedDateTime` ASC),
  INDEX `Ix_Organization` (`OrganizationId` ASC),
  INDEX `Ix_Person` (`PersonId` ASC),
  INDEX `Ix_Active` (`Active` ASC),
  INDEX `Ix_Open` (`Open` ASC))


#


CREATE TABLE `BackendServiceClasses` (
  `BackendServiceClassId` INT NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`BackendServiceClassId`),
  INDEX `Ix_Name` (`Name` ASC))


#


DROP PROCEDURE IF EXISTS `CreateBackendServiceOrder`


#


DROP PROCEDURE IF EXISTS `SetBackendServiceOrderActive`


#


DROP PROCEDURE IF EXISTS `SetBackendServiceOrderClosed`


#


DROP PROCEDURE IF EXISTS `SetBackendServiceOrderExceptionText`


#


CREATE PROCEDURE `CreateBackendServiceOrder`(
  IN dateTime DATETIME,
  IN organizationId INT,
  IN personId INT,
  IN backendServiceClassName VARCHAR(128),
  IN orderXml TEXT
)
BEGIN

  DECLARE backendServiceClassId INTEGER;

  IF ((SELECT COUNT(*) FROM BackendServiceClasses WHERE BackendServiceClasses.Name=backendServiceClassName) = 0)
  THEN

    INSERT INTO BackendServiceClasses
      (Name)
    VALUES 
      (backendServiceClassName);

    SELECT LAST_INSERT_ID() INTO backendServiceClassId;

  ELSE

    SELECT BackendServiceClasses.BackendServiceClassId INTO backendServiceClassId
      FROM BackendServiceClasses
      WHERE WHERE BackendServiceClasses.Name=backendServiceClassName;

  END IF;

  INSERT INTO BackendServiceOrders
    (CreatedDateTime, OrganizationId, PersonId, BackendServiceClassId, OrderXml)
  VALUES
    (dateTime, organizationId, personId, backendServiceClassId, orderXml);

  SELECT LAST_INSERT_ID() AS Identity;


#


CREATE PROCEDURE `SetBackendServiceOrderActive`(
  IN backendServiceOrderId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE BackendServiceOrders
    SET 
      BackendServiceOrders.Active=1,
      BackendServiceOrders.StartedDateTime=dateTime
    WHERE
      BackendServiceOrders.BackendServiceOrderId=backendServiceOrderId AND
      BackendServiceOrders.Active=0;

  SELECT ROW_COUNT() AS RecordsUpdated;

END


#


CREATE PROCEDURE `SetBackendServiceOrderClosed`(
  IN backendServiceOrderId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE BackendServiceOrders
    SET 
      BackendServiceOrders.Active=0,
      BackendServiceOrders.Open=0,
      BackendServiceOrders.ClosedDateTime=dateTime
    WHERE
      BackendServiceOrders.BackendServiceOrderId=backendServiceOrderId AND
      BackendServiceOrders.Open=1;

  SELECT ROW_COUNT() AS RecordsUpdated;

END


#


CREATE PROCEDURE `SetBackendServiceOrderExceptionText`(
  IN backendServiceOrderId INTEGER,
  IN exceptionText TEXT
)
BEGIN

  UPDATE BackendServiceOrders
    SET 
      BackendServiceOrders.ExceptionText=exceptionText,
      BackendServiceOrders.Open=0,
      BackendServiceOrders.Active=0
    WHERE
      BackendServiceOrders.BackendServiceOrderId=backendServiceOrderId;

  SELECT ROW_COUNT() AS RecordsUpdated;

END


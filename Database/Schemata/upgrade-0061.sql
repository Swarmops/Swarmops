ALTER TABLE `BackendServiceOrders` 
CHANGE COLUMN `CreatedDateTime` `CreatedDateTime` DATETIME NOT NULL ;


#



DROP PROCEDURE IF EXISTS `CreateBackendServiceOrder`


#


DROP PROCEDURE IF EXISTS `SetBackendServiceOrderActive`


#


DROP PROCEDURE IF EXISTS `SetBackendServiceOrderClosed`


#



CREATE PROCEDURE `CreateBackendServiceOrder`(
  IN nowUtc DATETIME,
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
      WHERE BackendServiceClasses.Name=backendServiceClassName;

  END IF;

  INSERT INTO BackendServiceOrders
    (CreatedDateTime, OrganizationId, PersonId, BackendServiceClassId, OrderXml, ExceptionText)
  VALUES
    (nowUtc, organizationId, personId, backendServiceClassId, orderXml, '');

  SELECT LAST_INSERT_ID() AS Identity;

END

#


CREATE PROCEDURE `SetBackendServiceOrderActive`(
  IN backendServiceOrderId INTEGER,
  IN nowUtc DATETIME
)
BEGIN

  UPDATE BackendServiceOrders
    SET 
      BackendServiceOrders.Active=1,
      BackendServiceOrders.StartedDateTime=nowUtc
    WHERE
      BackendServiceOrders.BackendServiceOrderId=backendServiceOrderId AND
      BackendServiceOrders.Active=0;

  SELECT ROW_COUNT() AS RecordsUpdated;

END


#


CREATE PROCEDURE `SetBackendServiceOrderClosed`(
  IN backendServiceOrderId INTEGER,
  IN nowUtc DATETIME
)
BEGIN

  UPDATE BackendServiceOrders
    SET 
      BackendServiceOrders.Active=0,
      BackendServiceOrders.Open=0,
      BackendServiceOrders.ClosedDateTime=nowUtc
    WHERE
      BackendServiceOrders.BackendServiceOrderId=backendServiceOrderId AND
      BackendServiceOrders.Open=1;

  SELECT ROW_COUNT() AS RecordsUpdated;

END




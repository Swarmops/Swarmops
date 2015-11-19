ALTER TABLE `OutboundComms` 
CHANGE COLUMN `ResolverClassId` `ResolverDataXml` TEXT NOT NULL


#


DROP procedure IF EXISTS `CreateOutboundComm`


#


CREATE PROCEDURE `CreateOutboundComm`(
  IN senderPersonId INT,
  IN fromPersonId INT,
  IN organizationId INT,
  IN resolverDataXml TEXT,
  IN recipientDataXml TEXT,
  IN transmitterClass VARCHAR(512),
  IN payloadXml TEXT,
  IN priority INT,
  IN createdDateTime DATETIME
)
BEGIN

  DECLARE transmitterClassId INT;

  IF ((SELECT COUNT(*) FROM TransmitterClasses WHERE TransmitterClasses.Name=transmitterClass) = 0)
  THEN
    INSERT INTO TransmitterClasses(Name) VALUES (transmitterClass);
    SELECT LAST_INSERT_ID() INTO transmitterClassId;
  ELSE
    SELECT TransmitterClasses.TransmitterClassId INTO transmitterClassId FROM TransmitterClasses WHERE TransmitterClasses.Name = transmitterClass;
  END IF;

  INSERT INTO OutboundComms (SenderPersonId,FromPersonId,OrganizationId,ResolverDataXml,RecipientDataXml,TransmitterClassId,PayloadXml,Priority,CreatedDateTime)
    VALUES (senderPersonId,fromPersonId,organizationId,resolverDataXml,recipientDataXml,transmitterClassId,payloadXml,priority,createdDateTime);

  SELECT LAST_INSERT_ID() AS Identity;

END



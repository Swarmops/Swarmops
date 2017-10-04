ALTER TABLE `VatReports` 
ADD COLUMN `Guid` VARCHAR(128) NOT NULL AFTER `OrganizationId`,
ADD INDEX `Ix_Guid` (`Guid` ASC)

#


DROP PROCEDURE IF EXISTS `CreateVatReport`


#


CREATE PROCEDURE `CreateVatReport`(
  organizationId INT,
  guid VARCHAR(128),
  createdDateTime DATETIME,
  yearMonthStart BIGINT,
  monthCount INT  
)
BEGIN
  INSERT INTO VatReports
    (OrganizationId,Guid,CreatedDateTime,YearMonthStart,MonthCount,Open,TurnoverCents,VatInboundCents,VatOutboundCents,UnderConstruction)
  VALUES
    (organizationId,guid,createdDateTime,yearMonthStart,monthCount,1,0,0,0,1);
    
  SELECT LAST_INSERT_ID() AS Identity;  
END





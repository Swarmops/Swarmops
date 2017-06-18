CREATE TABLE `VatReports` (
  `VatReportId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `DateTimeStart` DATETIME NOT NULL,
  `MonthCount` INT NOT NULL,
  `Open` TINYINT NOT NULL,
  `VatOutboundCents` BIGINT NOT NULL,
  `VatInboundCents` BIGINT NOT NULL,
  `UnderConstruction` TINYINT NOT NULL COMMENT 'This is 1 when rows are being populated and the report is not yet ready to display.',
  PRIMARY KEY (`VatReportId`),
  INDEX `Ix_Organization` (`OrganizationId` ASC),
  INDEX `Ix_DateTimeStart` (`DateTimeStart` ASC),
  INDEX `Ix_Open` (`Open` ASC),
  INDEX `Ix_Construction` (`UnderConstruction` ASC))


#


CREATE TABLE `VatReportItems` (
  `VatReportItemId` INT NOT NULL AUTO_INCREMENT,
  `VatReportId` INT NOT NULL,
  `FinancialTransactionRowId` INT NOT NULL,
  `ForeignObjectId` INT NOT NULL,
  `FinancialDependencyTypeId` INT NOT NULL,
  `VatInboundCents` BIGINT NOT NULL,
  `VatOutboundCents` BIGINT NOT NULL,
  PRIMARY KEY (`VatReportItemId`),
  INDEX `Ix_ReportId` (`VatReportId` ASC),
  INDEX `Ix_TxRowId` (`FinancialTransactionRowId` ASC))



#

DROP PROCEDURE IF EXISTS `CreateVatReport`

#

DROP PROCEDURE IF EXISTS `SetVatReportComplete`

#

DROP PROCEDURE IF EXISTS `SetVatReportOpen`

#

DROP PROCEDURE IF EXISTS `CreateVatReportItem`

#

CREATE PROCEDURE `CreateVatReport` (
  organizationId INT,
  createdDateTime DATETIME,
  dateTimeStart DATETIME,
  monthCount INT  
)

BEGIN
  INSERT INTO VatReports
    (OrganizationId,CreatedDateTime,DateTimeStart,MonthCount,Open,VatInboundCents,VatOutboundCents,UnderConstruction)
  VALUES
    (organizationId,createdDateTime,dateTimeStart,monthCount,1,0,0,1);
    
  SELECT LAST_INSERT_ID() AS Identity;  
END


Continue with procs to create Vat report, to set open and under construction, to calculate inbound/outbound cents
procs to create vat report item, including copying tx's foreign object and dependency

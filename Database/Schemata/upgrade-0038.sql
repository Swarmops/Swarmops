CREATE TABLE `VatReports` (
  `VatReportId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `DateTimeStart` DATETIME NOT NULL,
  `MonthCount` INT NOT NULL,
  `Open` TINYINT NOT NULL,
  `Turnover` BIGINT NOT NULL,
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
  `TurnoverCents` BIGINT NOT NULL,
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
    (OrganizationId,CreatedDateTime,DateTimeStart,MonthCount,Open,TurnoverCents,VatInboundCents,VatOutboundCents,UnderConstruction)
  VALUES
    (organizationId,createdDateTime,dateTimeStart,monthCount,1,0,0,0,1);
    
  SELECT LAST_INSERT_ID() AS Identity;  
END


#

CREATE PROCEDURE `SetVatReportCompleted` (
  vatReportId INT
)

BEGIN
  UPDATE VatReports
    SET VatReports.UnderConstruction = 0 WHERE VatReports.VatReportId=vatReportId;
END

#

CREATE PROCEDURE `SetVatReportOpen` (
  vatReportId INT,
  open INT
)

BEGIN
  UPDATE VatReports
    SET VatReports.Open = open WHERE VatReports.VatReportId=vatReportId;
END

#


CREATE PROCEDURE `AddVatReportItem` (
  vatReportId INT,
  financialTransactionRowId INT,
  foreignObjectId INT,
  financialDepedencyTypeId INT,
  turnoverCents BIGINT,
  vatInboundCents BIGINT,
  vatOutboundCents BIGINT
)

BEGIN
  UPDATE VatReports
    Set VatReports.TurnoverCents = VatReports.TurnoverCents + turnoverCents,
        VatReports.VatInboundCents = VatReports.VatInboundCents + vatInboundCents,
        VatReports.VatOutboundCents = VatReports.VatOutboundCents + vatOutboundCents
    WHERE VatReportId = vatReportId;

  INSERT INTO VatReportItems (VatReportId, FinancialTransactionRowId, ForeignObjectId, FinancialDependencyTypeId, TurnoverCents, VatInboundCents, VatOutboundCents)
    VALUES (vatReportId, financialTransactionRowId, foreignObjectId, financialDependencyTypeId, turnoverCents, vatInboundCents, vatOutboundCents);
END

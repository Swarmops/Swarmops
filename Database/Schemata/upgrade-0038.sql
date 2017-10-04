CREATE TABLE `VatReports` (
  `VatReportId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `YearMonthStart` BIGINT NOT NULL,
  `MonthCount` INT NOT NULL,
  `Open` TINYINT NOT NULL,
  `Turnover` BIGINT NOT NULL,
  `VatOutboundCents` BIGINT NOT NULL,
  `VatInboundCents` BIGINT NOT NULL,
  `UnderConstruction` TINYINT NOT NULL COMMENT 'This is 1 when rows are being populated and the report is not yet ready to display.',
  PRIMARY KEY (`VatReportId`),
  INDEX `Ix_Organization` (`OrganizationId` ASC),
  INDEX `Ix_YearMonthStart` (`YearMonthStart` ASC),
  INDEX `Ix_Open` (`Open` ASC),
  INDEX `Ix_Construction` (`UnderConstruction` ASC))


#


CREATE TABLE `VatReportItems` (
  `VatReportItemId` INT NOT NULL AUTO_INCREMENT,
  `VatReportId` INT NOT NULL,
  `FinancialTransactionId` INT NOT NULL,
  `ForeignId` INT NOT NULL,
  `FinancialDependencyTypeId` INT NOT NULL,
  `TurnoverCents` BIGINT NOT NULL,
  `VatInboundCents` BIGINT NOT NULL,
  `VatOutboundCents` BIGINT NOT NULL,
  PRIMARY KEY (`VatReportItemId`),
  INDEX `Ix_ReportId` (`VatReportId` ASC),
  INDEX `Ix_TxId` (`FinancialTransactionId` ASC))


#

DROP PROCEDURE IF EXISTS `CreateVatReport`

#

DROP PROCEDURE IF EXISTS `SetVatReportReleased`

#

DROP PROCEDURE IF EXISTS `SetVatReportOpen`

#

DROP PROCEDURE IF EXISTS `CreateVatReportItem`

#

CREATE PROCEDURE `CreateVatReport` (
  organizationId INT,
  createdDateTime DATETIME,
  yearMonthStart BIGINT,
  monthCount INT  
)

BEGIN
  INSERT INTO VatReports
    (OrganizationId,CreatedDateTime,YearMonthStart,MonthCount,Open,TurnoverCents,VatInboundCents,VatOutboundCents,UnderConstruction)
  VALUES
    (organizationId,createdDateTime,yearMonthStart,monthCount,1,0,0,0,1);
    
  SELECT LAST_INSERT_ID() AS Identity;  
END


#

CREATE PROCEDURE `SetVatReportReleased` (
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
  financialTransactionId INT,
  foreignObjectId INT,
  financialDependencyType VARCHAR(64),
  turnoverCents BIGINT,
  vatInboundCents BIGINT,
  vatOutboundCents BIGINT
)

BEGIN

  DECLARE financialDependencyTypeId INTEGER;

  SELECT 0 INTO financialDependencyTypeId;

  IF ((SELECT COUNT(*) FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name=financialDependencyType) = 0)
  THEN
    INSERT INTO FinancialDependencyTypes (Name)
      VALUES (financialDependencyType);

    SELECT LAST_INSERT_ID() INTO financialDependencyTypeId;

  ELSE

    SELECT FinancialDependencyTypes.FinancialDependencyTypeId INTO FinancialDependencyTypeId FROM FinancialDependencyTypes
        WHERE FinancialDependencyTypes.Name=financialDependencyType;

  END IF;

  UPDATE VatReports
    Set VatReports.TurnoverCents = VatReports.TurnoverCents + turnoverCents,
        VatReports.VatInboundCents = VatReports.VatInboundCents + vatInboundCents,
        VatReports.VatOutboundCents = VatReports.VatOutboundCents + vatOutboundCents
    WHERE VatReportId = vatReportId;

  INSERT INTO VatReportItems (VatReportId, FinancialTransactionId, ForeignObjectId, FinancialDependencyTypeId, TurnoverCents, VatInboundCents, VatOutboundCents)
    VALUES (vatReportId, financialTransactionId, foreignObjectId, financialDependencyTypeId, turnoverCents, vatInboundCents, vatOutboundCents);
END

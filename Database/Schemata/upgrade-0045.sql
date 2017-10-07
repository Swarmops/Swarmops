ALTER TABLE `VatReports` 
CHANGE COLUMN `VatInboundCents` `VatInboundCents` BIGINT(20) NOT NULL AFTER `Turnover`


#


DROP PROCEDURE IF EXISTS `AddVatReportItem`


#


CREATE PROCEDURE `CreateVatReportItem` (
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

  INSERT INTO VatReportItems (VatReportId, FinancialTransactionId, ForeignObjectId, FinancialDependencyTypeId, TurnoverCents, VatInboundCents, VatOutboundCents)
    VALUES (vatReportId, financialTransactionId, foreignObjectId, financialDependencyTypeId, turnoverCents, vatInboundCents, vatOutboundCents);
END



#


DROP PROCEDURE IF EXISTS `SetVatReportReleased`


#


CREATE PROCEDURE `SetVatReportReleased` (
  vatReportId INT
)

BEGIN
  UPDATE VatReports
    SET
       VatReports.TurnoverCents = (SELECT SUM(TurnoverCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
       VatReports.VatInboundCents = (SELECT SUM(VatInboundCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
       VatReports.VatOutboundCents = (SELECT SUM(VatOutboundCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
       VatReports.UnderConstruction = 0 

    WHERE VatReports.VatReportId=vatReportId;

END

DROP PROCEDURE IF EXISTS `CreateVatReportItem`

#

CREATE PROCEDURE `CreateVatReportItem`(
  vatReportId INT,
  financialTransactionId INT,
  foreignId INT,
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

  INSERT INTO VatReportItems (VatReportId, FinancialTransactionId, ForeignId, FinancialDependencyTypeId, TurnoverCents, VatInboundCents, VatOutboundCents)
    VALUES (vatReportId, financialTransactionId, foreignId, financialDependencyTypeId, turnoverCents, vatInboundCents, vatOutboundCents);


  SELECT LAST_INSERT_ID() AS Identity;

END

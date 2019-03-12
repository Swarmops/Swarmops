ALTER TABLE `ExpenseClaims` 
CHANGE COLUMN `ExpenseClaimId` `ExpenseClaimId` INT(11) UNSIGNED NOT NULL ,
CHANGE COLUMN `ClaimingPersonId` `ClaimingPersonId` INT(11) UNSIGNED NOT NULL ,
CHANGE COLUMN `LastExpenseEventTypeId` `LastExpenseEventTypeId` INT(11) UNSIGNED NOT NULL DEFAULT '0' COMMENT 'Legacy, ignore' ,
ADD COLUMN `OrganizationSequenceId` INT NOT NULL DEFAULT 0 AFTER `OrganizationId`,
DROP COLUMN `Amount`,
ADD INDEX `Index_OrgSequenceId` (`OrganizationSequenceId` ASC)


#


ALTER TABLE `CashAdvances` 
ADD COLUMN `OrganizationSequenceId` INT NOT NULL DEFAULT 0 AFTER `OrganizationId`,
ADD INDEX `IdxOrgSequenceId` (`OrganizationSequenceId` ASC)



#


DROP PROCEDURE IF EXISTS `CreateExpenseClaim`


#


DROP PROCEDURE IF EXISTS `CreateExpenseClaimPrecise`


#


DROP PROCEDURE IF EXISTS `CreateCashAdvance`


#


DROP PROCEDURE IF EXISTS `SetExpenseClaimOrganizationSequenceId`


#


DROP PROCEDURE IF EXISTS `SetCashAdvanceOrganizationSequenceId`


#



CREATE PROCEDURE `CreateExpenseClaim`(
  IN claimingPersonId INTEGER,
  IN createdDateTime DATETIME,
  IN organizationId INTEGER,
  IN budgetId INTEGER,
  IN expenseDate DATETIME,
  IN description VARCHAR(256),
  IN amountCents BIGINT
)
BEGIN

  DECLARE newIdentity INTEGER;
  DECLARE sequenceNumber INTEGER;

  INSERT INTO ExpenseClaims (ClaimingPersonId,CreatedDateTime,Open,Attested,Validated,
    Claimed,OrganizationId,GeographyId,BudgetId,ExpenseDate,Description,
    PreApprovedAmount,AmountCents)

    VALUES (claimingPersonId, createdDateTime,1,0,0,1,organizationId,0,budgetId,
      expenseDate,description,0.0,amountCents);

  SELECT LAST_INSERT_ID() INTO newIdentity;

  SELECT COUNT(*) FROM ExpenseClaims WHERE
    ExpenseClaims.OrganizationId = organizationId AND
    ExpenseClaims.expenseClaimId <= newIdentity INTO sequenceNumber;

  UPDATE ExpenseClaims
    SET ExpenseClaims.OrganizationSequenceId = sequenceNumber
    WHERE ExpenseClaims.expenseClaimId = newIdentity;

  SELECT newIdentity AS Identity;

END


#


CREATE PROCEDURE `CreateCashAdvance`(
  personId INT,
  createdDateTime DATETIME,
  createdByPersonId INT,
  organizationId INT,
  amountCents BIGINT,
  financialAccountId INT,
  description VARCHAR(128)
)
BEGIN

  DECLARE newIdentity INTEGER;
  DECLARE sequenceNumber INTEGER;

  INSERT INTO CashAdvances(
      PersonId,CreatedByPersonId,CreatedDateTime,OrganizationId,FinancialAccountId,
      AmountCents,Description,Open,Attested,AttestedByPersonId,
      AttestedDateTime,PaidOut)
    VALUES (
        personId,createdByPersonId,createdDateTime,organizationid,financialAccountId,
        amountCents,description,1,0,0,'1970-01-01',0);

  SELECT LAST_INSERT_ID() INTO newIdentity;

  SELECT COUNT(*) FROM CashAdvances WHERE
    CashAdvances.OrganizationId = organizationId AND
    CashAdvances.cashAdvanceId <= newIdentity INTO sequenceNumber;

  UPDATE CashAdvances
    SET CashAdvances.OrganizationSequenceId = sequenceNumber
    WHERE CashAdvances.cashAdvanceId = newIdentity;

  SELECT newIdentity AS Identity;

END


#


CREATE PROCEDURE `SetExpenseClaimOrganizationSequenceId`(
  IN expenseClaimId INTEGER
)
BEGIN

  DECLARE organizationId INTEGER;
  DECLARE sequenceNumber INTEGER;

  SELECT ExpenseClaims.OrganizationId 
      FROM ExpenseClaims
      WHERE ExpenseClaims.ExpenseClaimId = expenseClaimId
      INTO organizationId;

  SELECT COUNT(*)
      FROM OutboundInvoices
      WHERE OutboundInvoices.OrganizationId=organizationId
          AND OutboundInvoices.OutboundInvoiceId <= outboundInvoiceId
      INTO sequenceNumber;

  UPDATE OutboundInvoices SET OrganizationSequenceId = sequenceNumber
    WHERE OutboundInvoices.OutboundInvoiceId = outboundInvoiceId;

  SELECT sequenceNumber AS OrganizationSequenceId;  

END


#


CREATE PROCEDURE `SetCashAdvanceOrganizationSequenceId`(
  IN cashAdvanceId INTEGER
)
BEGIN

  DECLARE organizationId INTEGER;
  DECLARE sequenceNumber INTEGER;

  SELECT CashAdvances.OrganizationId 
      FROM CashAdvances
      WHERE CashAdvances.CashAdvanceId = cashAdvanceId
      INTO organizationId;

  SELECT COUNT(*)
      FROM CashAdvances
      WHERE CashAdvances.OrganizationId=organizationId
          AND CashAdvances.CashAdvanceId <= cashAdvanceId
      INTO sequenceNumber;

  UPDATE CashAdvances SET OrganizationSequenceId = sequenceNumber
    WHERE CashAdvances.CashAdvanceId = cashAdvanceId;

  SELECT sequenceNumber AS OrganizationSequenceId;  

END

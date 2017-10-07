ALTER TABLE `InboundInvoices` 
DROP COLUMN `Amount`,
ADD COLUMN `OrganizationSequenceId` INT NOT NULL DEFAULT 0 AFTER `OrganizationId`,
ADD INDEX `Index_SequenceId` (`OrganizationSequenceId` ASC)


#

DROP PROCEDURE IF EXISTS `CreateInboundInvoice`


#


DROP PROCEDURE IF EXISTS `CreateInboundInvoicePrecise`


#

CREATE PROCEDURE `CreateInboundInvoice`(
  organizationId INTEGER,
  createdDateTime DATETIME,
  dueDate DATETIME,
  budgetId INTEGER,
  supplier VARCHAR(64),
  payToAccount VARCHAR(64),
  ocr VARCHAR(64),
  invoiceReference VARCHAR(64),
  amountCents BIGINT,
  createdByPersonId INTEGER
)
BEGIN

  DECLARE newIdentity INTEGER;
  DECLARE sequenceNumber INTEGER;

  INSERT INTO InboundInvoices
    (OrganizationId, CreatedDateTime, DueDate, BudgetId, Supplier, Attested, Open, PayToAccount, Ocr,
     InvoiceReference, ClosedDateTime, AmountCents, CreatedByPersonId, ClosedByPersonId)
    VALUES
      (organizationId, createdDateTime, dueDate, budgetId, supplier, 0, 1, payToAccount, ocr,
       invoiceReference, createdDateTime, amountCents, createdByPersonId, 0);

  SELECT LAST_INSERT_ID() INTO newIdentity;

  SELECT COUNT(*) FROM InboundInvoices WHERE
        InboundInvoices.OrganizationId = organizationId AND
        InboundInvoices.InboundInvoiceId <= newIdentity INTO sequenceNumber;

  UPDATE InboundInvoices 
    SET InboundInvoices.OrganizationSequenceId = sequenceNumber
    WHERE InboundInvoices.InboundInvoiceId = newIdentity;

  SELECT newIdentity AS Identity;

END


#


CREATE PROCEDURE `SetInboundInvoiceOrganizationSequenceId`(
  IN inboundInvoiceId INTEGER
)
BEGIN

  DECLARE organizationId INTEGER;
  DECLARE sequenceNumber INTEGER;

  SELECT InboundInvoices.OrganizationId 
      FROM InboundInvoices
      WHERE InboundInvoices.InboundInvoiceId = inboundInvoiceId
      INTO organizationId;

  SELECT COUNT(*)
      FROM InboundInvoices
      WHERE InboundInvoices.OrganizationId=organizationId
          AND InboundInvoices.InboundInvoiceId <= inboundInvoiceId
      INTO sequenceNumber;

  UPDATE InboundInvoices SET OrganizationSequenceId = sequenceNumber
    WHERE InboundInvoices.InboundInvoiceId = inboundInvoiceId;

  SELECT sequenceNumber AS OrganizationSequenceId;  

END
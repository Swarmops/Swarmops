DROP PROCEDURE IF EXISTS `CreateOutboundInvoice`

#

CREATE PROCEDURE `CreateOutboundInvoice`(
  organizationId INTEGER,
  createdDateTime DATETIME,
  createdByPersonId INTEGER,
  dueDate DATETIME,
  budgetId INTEGER,
  customerName VARCHAR(128),
  invoiceAddressPaper VARCHAR(256),
  invoiceAddressMail VARCHAR(128),
  currencyId INTEGER,
  reference VARCHAR(64),
  domestic TINYINT(1),
  securityCode VARCHAR(16),
  theirReference VARCHAR(64)
)
BEGIN

  DECLARE newIdentity INTEGER;
  DECLARE sequenceNumber INTEGER;

  INSERT INTO OutboundInvoices
    (CustomerName,InvoiceAddressPaper,InvoiceAddressMail,CurrencyId,
     BudgetId,OrganizationId,CreatedDateTime,CreatedByPersonId,
     DueDate,ReminderCount,Reference,Open,Domestic,Sent,SecurityCode,TheirReference)
  VALUES
    (customerName,invoiceAddressPaper,invoiceAddressMail,currencyId,
     budgetId,organizationId,createdDateTime,createdByPersonId,
     dueDate,0,reference,1,domestic,0,securityCode,theirReference);

  SELECT LAST_INSERT_ID() INTO newIdentity;

  SELECT COUNT(*) FROM OutboundInvoices WHERE
    OutboundInvoices.OrganizationId = organizationId AND
    OutboundInvoices.OutboundInvoiceId <= newIdentity INTO sequenceNumber;

  UPDATE OutboundInvoices 
    SET OutboundInvoices.OrganizationSequenceId = sequenceNumber
    WHERE OutboundInvoices.OutboundInvoiceId = newIdentity;

  SELECT newIdentity AS Identity;

END


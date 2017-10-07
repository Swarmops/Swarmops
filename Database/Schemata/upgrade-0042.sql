DROP PROCEDURE IF EXISTS `SetInboundInvoiceOrganizationSequenceId`


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

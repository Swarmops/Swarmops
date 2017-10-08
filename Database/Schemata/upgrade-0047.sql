DROP PROCEDURE IF EXISTS `SetVatReportReleased`

#

CREATE PROCEDURE `SetVatReportReleased`(
  vatReportId INT
)
BEGIN

  IF EXISTS (SELECT * FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId)
  THEN

    UPDATE VatReports
      SET
         VatReports.TurnoverCents = (SELECT SUM(TurnoverCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
         VatReports.VatInboundCents = (SELECT SUM(VatInboundCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
         VatReports.VatOutboundCents = (SELECT SUM(VatOutboundCents) FROM VatReportItems WHERE VatReportItems.VatReportId=vatReportId), 
         VatReports.UnderConstruction = 0 

      WHERE VatReports.VatReportId=vatReportId;
      
   ELSE
   
      UPDATE VatReports
        SET VatReports.UnderConstruction = 0 
        WHERE VatReports.VatReportId=vatReportId;
      
   END IF;

END
ALTER TABLE `VatReports` 
ADD COLUMN `OpenTransactionId` INT NOT NULL DEFAULT 0 AFTER `Open`,
ADD COLUMN `CloseTransactionId` INT NOT NULL DEFAULT 0 AFTER `OpenTransactionId`,
ADD INDEX `Ix_OpenTx` (`OpenTransactionId` ASC),
ADD INDEX `Ix_CloseTx` (`CloseTransactionId` ASC);


#

DROP PROCEDURE IF EXISTS `SetVatReportOpenTransaction`

#

DROP PROCEDURE IF EXISTS `SetVatReportCloseTransaction`

#

CREATE PROCEDURE `SetVatReportOpenTransaction`(
  IN vatReportId INTEGER,
  IN openTransactionId INTEGER
)
BEGIN

  UPDATE VatReports SET OpenTransactionId=openTransactionId
    WHERE VatReports.VatReportId=vatReportId;

  SELECT ROW_COUNT() AS RecordsUpdated;

END


#

CREATE PROCEDURE `SetVatReportCloseTransaction`(
  IN vatReportId INTEGER,
  IN closeTransactionId INTEGER
)
BEGIN

  UPDATE VatReports SET CloseTransactionId=closeTransactionId
    WHERE VatReports.VatReportId=vatReportId
    AND VatReports.CloseTransactionId=0;

  SELECT ROW_COUNT() AS RecordsUpdated;

END

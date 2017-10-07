ALTER TABLE `Documents` 
ADD COLUMN `HiresStatus` INT NULL DEFAULT 0 AFTER `UploadedDateTime`,
ADD INDEX `Index_HiresStatus` (`HiresStatus` ASC)


#


UPDATE Documents SET Documents.HiresStatus=1 WHERE Documents.HiresStatus=0;


#


DROP PROCEDURE IF EXISTS `SetDocumentHiresStatus`


#


CREATE PROCEDURE `SetDocumentHiresStatus`(
  documentId INT,
  hiresStatus INT
)
BEGIN

  UPDATE Documents 
    SET Documents.HiresStatus = hiresStatus
    WHERE Documents.DocumentId = documentId;

END

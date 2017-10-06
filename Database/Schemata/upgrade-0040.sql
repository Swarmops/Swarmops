ALTER TABLE `Documents` 
ADD COLUMN `HiresStatus` INT NULL DEFAULT 0 AFTER `UploadedDateTime`,
ADD INDEX `Index_HiresStatus` (`HiresStatus` ASC)


#


DROP PROCEDURE IF EXISTS `SetDocumentHiresStatus`


#


CREATE PROCEDURE `SetDocumentHiresStatus`(
  organizationId INT,
  guid VARCHAR(128),
  createdDateTime DATETIME,
  yearMonthStart BIGINT,
  monthCount INT  
)

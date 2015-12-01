CREATE TABLE `FinancialTransactionForeignIds` (
  `FinancialTransactionForeignIdId` INT NOT NULL AUTO_INCREMENT,
  `FinancialTransactionId` INT NOT NULL,
  `ForeignIdType` INT NOT NULL,
  `ForeignId` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`FinancialTransactionForeignIdId`),
  INDEX `Ix_TxId` (`FinancialTransactionId` ASC),
  INDEX `Ix_IdType` (`ForeignIdType` ASC),
  INDEX `Ix_Identifier` (`ForeignId` ASC)
)


#


DROP procedure IF EXISTS `SetFinancialTransactionForeignIdentity`


#


CREATE PROCEDURE `SetFinancialTransactionForeignIdentity`(
	IN financialTransactionId INT,
	IN foreignIdType INT,
	IN foreignId VARCHAR(256)
)
BEGIN

  IF ((SELECT COUNT(*) FROM FinancialTransactionForeignIds 
    WHERE FinancialTransactionForeignIds.FinancialTransactionId=financialTransactionId
      AND FinancialTransactionForeignIds.ForeignIdType=foreignIdType) = 0)
  THEN

    INSERT INTO FinancialTransactionForeignIds (FinancialTransactionId,ForeignIdType,ForeignId)
      VALUES (financialTransactionId,foreignIdType,foreignId);
    SELECT LAST_INSERT_ID() AS Identity;

  ELSE

    UPDATE FinancialTransactionForeignIds
      SET FinancialTransactionForeignId.ForeignId=foreignId
      WHERE FinancialTransactionForeignIds.FinancialTransactionId=financialTransactionId
        AND FinancialTransactionForeignIds.ForeignIdType=foreignIdType;

    SELECT 0 AS Identity;

  END IF;

END

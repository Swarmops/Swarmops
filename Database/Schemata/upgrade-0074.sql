ALTER TABLE `FinancialTransactions` 
ADD COLUMN `ImportSha256` VARCHAR(80) NOT NULL DEFAULT '' AFTER `ImportHash`

#

DROP PROCEDURE IF EXISTS `CreateFinancialTransactionStub`

#

DROP PROCEDURE IF EXISTS `SetFinancialTransactionImportSha256`

#

CREATE PROCEDURE `CreateFinancialTransactionStub`(
  IN organizationId INTEGER,
  IN financialAccountId INTEGER,
  IN amountCents BIGINT,
  IN dateTime DATETIME,
  IN comment TEXT,
  IN importHash VARCHAR(32),
  IN importSha256 VARCHAR(80),
  IN personId INTEGER
)
BEGIN
  
  DECLARE financialTransactionId INTEGER;
  DECLARE existingSha256 VARCHAR(80);

  SELECT 0 INTO financialTransactionId;

  IF ((SELECT COUNT(*) FROM FinancialTransactions WHERE FinancialTransactions.ImportHash=importHash) = 0)
  THEN
    INSERT INTO FinancialTransactions (OrganizationId, DateTime, Comment, ImportHash, ImportSha256)
      VALUES (organizationId, dateTime, comment, importHash, importSha256);

    SELECT LAST_INSERT_ID() INTO financialTransactionId;

    INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, AmountCents, CreatedDateTime, CreatedByPersonId)
      VALUES (financialAccountId, financialTransactionId, amountCents, DateTime, personId);

  ELSE
  
    SELECT FinancialTransactions.FinancialTransactionId INTO financialTransactionId FROM FinancialTransactions WHERE FinancialTransactions.ImportHash=importHash;
    SELECT FinancialTransactions.ImportSha256 INTO existingSha256 FROM FinancialTransactions WHERE FinancialTransactions.ImportHash=importHash;

    IF (existingSha256 != importSha256)
    THEN
    
      UPDATE FinancialTransactions 
        SET FinancialTransactions.ImportSha256=importSha256
        WHERE FinancialTransactions.FinancialTransactionId=financialTransactionId;
    
      SELECT -financialTransactionId INTO financialTransactionId;
    
    END IF;

  END IF;

  SELECT financialTransactionId AS Identity;

END

#


CREATE PROCEDURE `SetFinancialTransactionImportSha256`(
  IN financialTransactionId INTEGER,
  IN importSha256 VARCHAR(80)
)
BEGIN

  UPDATE FinancialTransactions SET FinancialTransactions.ImportSha256 = importSha256
    WHERE FinancialTransactions.FinancialTransactionId = financialTransactionId;

END


ALTER TABLE `FinancialAccounts` 
ADD COLUMN `Active` TINYINT NOT NULL DEFAULT '1' AFTER `Administrative`,
ADD COLUMN `LinkBackward` INT NOT NULL DEFAULT 0 AFTER `Active`,
ADD COLUMN `LinkForward` INT NOT NULL DEFAULT 0 AFTER `LinkBackward`


#


CREATE PROCEDURE `SetFinancialAccountActive`(
  IN financialAccountId INTEGER,
  IN active TINYINT
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.Active = active
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#



CREATE PROCEDURE `SetFinancialAccountExpensable`(
  IN financialAccountId INTEGER,
  IN expensable TINYINT
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.Expensable = expensable
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#



CREATE PROCEDURE `SetFinancialAccountAdministrative`(
  IN financialAccountId INTEGER,
  IN administrative TINYINT
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.Administrative = administrative
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#



CREATE PROCEDURE `SetFinancialAccountLinkBackward`(
  IN financialAccountId INTEGER,
  IN linkBackward INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.LinkBackward = linkBackward
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountLinkForward`(
  IN financialAccountId INTEGER,
  IN linkForward INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.LinkForward = linkForward
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#



CREATE PROCEDURE `SetFinancialAccountParent`(
  IN financialAccountId INTEGER,
  IN parentFinancialAccount INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.ParentFinancialAccountId = parentFinancialAccountId
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `MoveTransactionRowRangeToNewFinancialAccount`(
  IN oldFinancialAccountId INTEGER,
  IN newFinancialAccountId INTEGER,
  IN startDate DATETIME,
  IN endDate DATETIME
)
BEGIN

  UPDATE FinancialTransactionRows 
    JOIN FinancialTransactions ON FinancialTransactions.FinancialTransactionId=FinancialTransactionRows.FinancialTransactionId
    SET FinancialTransactionRows.FinancialAccountId=newFinancialAccountId
    WHERE FinancialTransactions.DateTime >= startDate AND FinancialTransactions.DateTime < endDate
    AND FinancialTransactionRows.FinancialAccountId=oldFinancialAccountId;

END

ALTER TABLE `ExpenseClaims` 
ADD COLUMN `ExpenseClaimGroupId` INT(11) NOT NULL DEFAULT 0 AFTER `ClosedDateTime`,
ADD INDEX `Index_GroupId` (`ExpenseClaimGroupId` ASC)

#

CREATE TABLE `ExpenseClaimGroups` (
  `ExpenseClaimGroupId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `CreatedDateTime` DATETIME NOT NULL,
  `CreatedByPersonId` INT NOT NULL,
  `Open` TINYINT NOT NULL DEFAULT 1,
  `ExpenseClaimGroupTypeId` INT NOT NULL,
  `ExpenseClaimGroupData` TEXT NOT NULL,
  PRIMARY KEY (`ExpenseClaimGroupId`),
  INDEX `Idx_Person` (`CreatedByPersonId` ASC),
  INDEX `Idx_Time` (`CreatedDateTime` ASC),
  INDEX `Idx_Org` (`OrganizationId` ASC),
  INDEX `Idx_Open` (`Open` ASC),
  INDEX `Idx_Type` (`ExpenseClaimGroupTypeId` ASC))

#

CREATE TABLE `ExpenseClaimGroupTypes` (
  `ExpenseClaimGroupTypeId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(64) NOT NULL,
  PRIMARY KEY (`ExpenseClaimGroupTypeId`))

#

DROP PROCEDURE IF EXISTS `CreateExpenseClaimGroup`

#

DROP PROCEDURE IF EXISTS `SetExpenseClaimGroup`

#

DROP PROCEDURE IF EXISTS `SetExpenseClaimGroupClosed`

#

CREATE PROCEDURE `CreateExpenseClaimGroup`(
  IN organizationId INT,
  IN dateTime DATETIME,
  IN createdByPersonId INT,
  IN expenseClaimGroupType VARCHAR(64),
  IN expenseClaimGroupData TEXT
)
BEGIN

  DECLARE expenseClaimGroupTypeId INT;

  IF ((SELECT COUNT(*) FROM ExpenseClaimGroupTypes WHERE ExpenseClaimGroupTypes.Name=expenseClaimGroupType) = 0)
  THEN
    INSERT INTO ExpenseClaimGroupTypes (Name) VALUES (expenseClaimGroupType);
    SELECT LAST_INSERT_ID() INTO expenseClaimGroupTypeId;
  ELSE
    SELECT ExpenseClaimGroupTypes.ExpenseClaimGroupTypeId
      INTO expenseclaimGroupTypeId FROM ExpenseClaimGroupTypes 
      WHERE ExpenseClaimGroupTypes.Name = expenseClaimGroupType;
  END IF;

  INSERT INTO ExpenseClaimGroups (OrganizationId,CreatedDateTime,CreatedByPersonId,ExpenseClaimGroupTypeId,ExpenseClaimGroupData)
    VALUES (organizationId,dateTime,createdByPersonId,expenseClaimGroupTypeId,expenseClaimGroupData);

  SELECT LAST_INSERT_ID() AS Identity;

END

#

CREATE PROCEDURE `SetExpenseClaimGroupClosed` (
  IN expenseClaimGroupId
)
BEGIN
  DECLARE createdByPersonId INT;
  DECLARE organizationId INT;

  SELECT ExpenseClaimGroups.CreatedByPersonId
    INTO createdByPersonId
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  SELECT ExpenseClaimGroups.OrganizationId
    INTO organizationId
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  UPDATE ExpenseClaimGroups 
    SET ExpenseClaimGroups.Open = 0
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  UPDATE ExpenseClaims
    SET ClaimingPersonId = createdByPersonId,
        OrganizationId = organizationId
    WHERE ExpenseClaimGroupId = expenseClaimGroupId;

END

#

CREATE PROCEDURE `SetExpenseClaimGroup` (
  IN expenseClaimId,
  IN expenseClaimGroupId
)
BEGIN
  UPDATE ExpenseClaims
    SET ExpenseClaimGroupId=expenseClaimGroupId
    WHERE ExpenseClaimId=expenseClaimId;
END



CREATE TABLE `MaintenanceDonations` (
  `MaintenanceDonationId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `OpenedFinancialTransactionId` INT NOT NULL,
  `Satoshis` BIGINT NOT NULL,
  `ClosedFinancialTransactionId` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`MaintenanceDonationId`),
  INDEX `ixOpened` (`OpenedFinancialTransactionId` ASC),
  INDEX `ixClosed` (`ClosedFinancialTransactionId` ASC),
  INDEX `ixOrg` (`OrganizationId` ASC))

#


DROP PROCEDURE IF EXISTS `CreateMaintenanceDonation`


#


DROP PROCEDURE IF EXISTS `SetMaintenanceDonationClosed`


#


CREATE PROCEDURE `CreateMaintenanceDonation` (
  IN organizationId INT,
  IN openedFinancialId INT,
  IN satoshis BIGINT
)

BEGIN
  INSERT INTO MaintenanceDonations (OrganizationId,OpenedFinancialId,Satoshis)
    VALUES (organizationId,openedFinancialId,satoshis);

  SELECT LAST_INSERT_ID() AS Identity;
END


#


CREATE PROCEDURE `SetMaintenanceDonationClosed` (
  IN maintenanceDonationId INT,
  IN closedFinancialId INT
)

BEGIN
  UPDATE MaintenanceDonations
    SET MaintenanceDonations.ClosedFinancialId=closedFinancialId
    WHERE MaintenanceDonations.MaintenanceDonationId=maintenanceDonationId;

  SELECT ROW_COUNT() AS RecordsUpdated;

END

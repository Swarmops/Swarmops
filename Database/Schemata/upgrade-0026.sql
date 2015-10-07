CREATE TABLE `HotBitcoinAddresses` (
  `HotBitcoinAddressId` INT NOT NULL AUTO_INCREMENT,
  `OrganizationId` INT NOT NULL,
  `DerivationPath` VARCHAR(256) NOT NULL,
  `AddressString` VARCHAR(48) NOT NULL,
  `BalanceSatoshis` BIGINT NOT NULL,
  `ThroughputSatoshis` BIGINT NOT NULL,
  PRIMARY KEY (`HotBitcoinAddressId`),
  INDEX `Ix_Organization` (`OrganizationId` ASC),
  INDEX `Ix_Balance` (`BalanceSatoshis` ASC),
  INDEX `Ix_Address` (`AddressString` ASC),
  INDEX `Ix_Used` (`ThroughputSatoshis` ASC))


#


CREATE TABLE `HotBitcoinAddressUnspents` (
  `HotBitcoinAddressUnspentId` INT NOT NULL AUTO_INCREMENT,
  `AmountSatoshis` BIGINT NOT NULL,
  `TransactionHash` VARCHAR(128) NOT NULL,
  `TransactionOutputIndex` INT NOT NULL,
  `ConfirmationCount` INT NOT NULL,
  PRIMARY KEY (`HotBitcoinAddressUnspentId`),
  INDEX `Ix_Confirmations` (`ConfirmationCount` ASC),
  INDEX `Ix_Amount` (`AmountSatoshis` ASC),
  INDEX `Ix_TxHash` (`TransactionHash` ASC))


#


DROP procedure IF EXISTS `CreateHotBitcoinAddress`


#


CREATE PROCEDURE `CreateHotBitcoinAddress` (
  IN organizationId INT,
  IN derivationPath VARCHAR(256),
  IN addressString VARCHAR(48)
)
BEGIN
  INSERT INTO HotBitcoinAddresses
    (OrganizationId,DerivationPath,AddressString,BalanceSatoshis,ThroughputSatoshis)
  VALUES 
    (organizationId,derivationPath,addressString,0,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `SetHotBitcoinAddressBalance`


#


CREATE PROCEDURE `SetHotBitcoinAddressBalance` (
  IN hotBitcoinAddressId INT,
  IN balanceSatoshis BIGINT
)
BEGIN
  UPDATE HotBitcoinAddresses
  SET HotBitcoinAddresses.BalanceSatoshis=balanceSatoshis
  WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

  SELECT ROW_COUNT() AS RowsUpdated;

END


#


DROP procedure IF EXISTS `SetHotBitcoinAddressThroughput`


#


CREATE PROCEDURE `SetHotBitcoinAddressThroughput` (
  IN hotBitcoinAddressId INT,
  IN throughputSatoshis BIGINT
)
BEGIN
  UPDATE HotBitcoinAddresses
  SET HotBitcoinAddresses.ThroughputSatoshis=throughputSatoshis
  WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

  SELECT ROW_COUNT() AS RowsUpdated;

END

#


DROP procedure IF EXISTS `CreateHotBitcoinAddressUnspent`


#


CREATE PROCEDURE `CreateHotBitcoinAddressUnspent` (
  IN amountSatoshis BIGINT,
  IN transactionHash VARCHAR(128),
  IN transactionOutputIndex INT,
  IN confirmationCount INT
)
BEGIN
  INSERT INTO HotBitcoinAddressUnspent
    (AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
  VALUES 
    (amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `SetHotBitcoinAddressUnspentConfirmations`


#


CREATE PROCEDURE `SetHotBitcoinAddressUnspentConfirmations` (
  IN hotBitcoinAddressUnspentId INT,
  IN confirmationCount INT
)
BEGIN
  UPDATE HotBitcoinAddressUnspents
  SET HotBitcoinAddressUnspents.ConfirmationCount=confirmationCount
  WHERE HotBitcoinAddressUnspents.HotBitcoinAddressUnspentId=hotBitcoinAddressUnspentId;

  SELECT ROW_COUNT() AS RowsUpdated;

END


#


DROP procedure IF EXISTS `DeleteHotBitcoinAddressUnspent`


#


CREATE PROCEDURE `DeleteHotBitcoinAddressUnspent` (
  IN hotBitcoinAddressUnspentId INT
)
BEGIN
  DELETE FROM HotBitcoinAddressUnspents
  WHERE HotBitcoinAddressUnspents.HotBitcoinAddressUnspentId=hotBitcoinAddressUnspentId;

  SELECT ROW_COUNT() AS RowsDeleted;

END



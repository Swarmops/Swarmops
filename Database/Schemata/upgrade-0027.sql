ALTER TABLE `HotBitcoinAddresses` 
ADD UNIQUE INDEX `Ix_Address` (`AddressString` ASC);


#


ALTER TABLE `activizr-dev`.`HotBitcoinAddressUnspents` 
ADD UNIQUE INDEX `Ix_HashIndex` (`TransactionHash` ASC, `TransactionOutputIndex` ASC);


#


DROP procedure IF EXISTS `CreateHotBitcoinAddressConditional`


#


CREATE PROCEDURE `CreateHotBitcoinAddressConditional` (
  IN organizationId INT,
  IN derivationPath VARCHAR(256),
  IN addressString VARCHAR(48)
)
BEGIN

  IF ((SELECT COUNT(*) FROM HotBitcoinAddresses WHERE HotBitcoinAddresses.AddressString=addressString) = 0)
  THEN

    INSERT INTO HotBitcoinAddresses
      (OrganizationId,DerivationPath,AddressString,BalanceSatoshis,ThroughputSatoshis)
    VALUES 
      (organizationId,derivationPath,addressString,0,0);

    SELECT LAST_INSERT_ID() AS Identity;

  ELSE

    SELECT HotBitcoinAddresses.HotBitcoinAddressId AS Identity
    WHERE HotBitcoinAddresses.AddressString=addressString;

  END IF;

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
  INSERT INTO HotBitcoinAddressUnspents
    (AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
  VALUES 
    (amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `CreateHotBitcoinAddressUnspentConditional`


#


CREATE PROCEDURE `CreateHotBitcoinAddressUnspentConditional` (
  IN amountSatoshis BIGINT,
  IN transactionHash VARCHAR(128),
  IN transactionOutputIndex INT,
  IN confirmationCount INT
)
BEGIN

  DECLARE unspentId INT;

  IF ((SELECT COUNT(*) FROM HotBitcoinAddressUnspents 
    WHERE HotBitcoinAddressUnspents.TransactionHash=transactionHash
      AND HotBitcoinAddressUnspents.TransactionOutputIndex=transactionOutputIndex) = 0)
  THEN

    INSERT INTO HotBitcoinAddressUnspents
      (AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
    VALUES 
      (amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

    SELECT LAST_INSERT_ID() AS Identity;

  ELSE

    SELECT HotBitcionAddressUnspents.HotBitcoinAddressUnspentId INTO unspentId
      FROM HotBitcoinAddressUnspents
      WHERE HotBitcoinAddressUnspents.TransactionHash=transactionHash
      AND HotBitcoinAddressUnspents.TransactionOutputIndex=transactionOutputIndex;

    UPDATE HotBitcoinAddressUnspents
      SET HotBitcoinAddressUnspents.ConfirmationCount=confirmationCount
      WHERE HotBitcoinAddressUnspents.HotBitcoinAddressUnspentId=unspentId;

    SELECT unspentId AS Identity;

  END IF;

END

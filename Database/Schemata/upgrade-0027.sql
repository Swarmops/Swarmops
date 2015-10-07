ALTER TABLE `HotBitcoinAddresses` 
ADD UNIQUE INDEX `Ix_Address` (`AddressString` ASC)


#


ALTER TABLE `HotBitcoinAddressUnspents` 
CHANGE COLUMN `AmountSatoshis` `AmountSatoshis` BIGINT(20) NOT NULL AFTER `TransactionOutputIndex`,
ADD COLUMN `HotBitcoinAddressId` INT NOT NULL AFTER `HotBitcoinAddressUnspentId`,
ADD INDEX `Ix_Address` (`HotBitcoinAddressId` ASC),
ADD UNIQUE INDEX `Ix_HashIndex` (`TransactionHash` ASC, `TransactionOutputIndex` ASC)


#


DROP procedure IF EXISTS `CreateHotBitcoinAddressConditional`



#



DROP procedure IF EXISTS `CreateHotBitcoinAddressUnspent`


#


CREATE PROCEDURE `CreateHotBitcoinAddressUnspent` (
  IN hotBitcoinAddressId INT,
  IN amountSatoshis BIGINT,
  IN transactionHash VARCHAR(128),
  IN transactionOutputIndex INT,
  IN confirmationCount INT
)
BEGIN
  INSERT INTO HotBitcoinAddressUnspents
    (HotBitcoinAddressId,AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
  VALUES 
    (hotBitcoinAddressId,amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


DROP procedure IF EXISTS `CreateHotBitcoinAddressUnspentConditional`


#


CREATE PROCEDURE `CreateHotBitcoinAddressUnspentConditional` (
  IN hotBitcoinAddressId INT,
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
      (HotBitcoinAddressId,AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
    VALUES 
      (hotBitcoinAddressId,amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

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

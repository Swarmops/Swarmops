ALTER TABLE `HotBitcoinAddresses` 
ADD COLUMN `UniqueDerive` INT NOT NULL DEFAULT -1 AFTER `DerivationPath`,
ADD INDEX `Ix_Path` (`DerivationPath` ASC),
ADD INDEX `Ix_Unique` (`UniqueDerive` ASC)


#


DROP PROCEDURE IF EXISTS `SetHotBitcoinAddressAddress`


#


DROP PROCEDURE IF EXISTS `SetHotBitcoinAddress`


#


DROP PROCEDURE IF EXISTS `SetHotBitcoinAddressFallbackAddress`


#


DROP PROCEDURE IF EXISTS `CreateHotBitcoinAddress`


#


CREATE PROCEDURE `CreateHotBitcoinAddress`(
  IN organizationId INT,
  IN bitcoinChainId INT,
  IN derivationPath VARCHAR(256)
)
BEGIN

  DECLARE newIdentity INTEGER;
  DECLARE sequenceNumber INTEGER;

  INSERT INTO HotBitcoinAddresses
    (OrganizationId,BitcoinChainId,DerivationPath,UniqueDerive,AddressString,AddressStringFallback,BalanceSatoshis,ThroughputSatoshis)
  VALUES 
    (organizationId,bitcoinChainId,derivationPath,-1,'--TBD--','',0,0);

  SELECT LAST_INSERT_ID() INTO newIdentity;

  SELECT COUNT(*) FROM HotBitcoinAddresses WHERE
        HotBitcoinAddresses.DerivationPath = derivationPath AND
        HotBitcoinAddresses.OrganizationId = organizationId AND
        HotBitcoinAddresses.HotBitcoinAddressId <= newIdentity INTO sequenceNumber;

  UPDATE HotBitcoinAddresses
    SET HotBitcoinAddresses.UniqueDerive = sequenceNumber
    WHERE HotBitcoinAddresses.HotBitcoinAddressId = newIdentity;

  SELECT newIdentity AS Identity;

END


#


CREATE PROCEDURE `SetHotBitcoinAddressAddress`(
  IN hotBitcoinAddressId INT,
  IN addressString VARCHAR(48)
)
BEGIN
  UPDATE HotBitcoinAddresses
  SET HotBitcoinAddresses.AddressString=addressString
  WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

  SELECT ROW_COUNT() AS RowsUpdated;

END


#


CREATE PROCEDURE `SetHotBitcoinAddressFallbackAddress`(
  IN hotBitcoinAddressId INT,
  IN addressStringFallback VARCHAR(48)
)
BEGIN
  UPDATE HotBitcoinAddresses
  SET HotBitcoinAddresses.AddressStringFallback=addressStringFallback
  WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

  SELECT ROW_COUNT() AS RowsUpdated;

END



DROP PROCEDURE IF EXISTS `CreateHotBitcoinAddressConditional`

#


CREATE PROCEDURE `CreateHotBitcoinAddressConditional`(
  IN organizationId INT,
  IN bitcoinChainId INT,
  IN derivationPath VARCHAR(256),
  IN addressString VARCHAR(48),
  IN addressStringFallback VARCHAR(48)
)
BEGIN

  IF ((SELECT COUNT(*) FROM HotBitcoinAddresses WHERE HotBitcoinAddresses.AddressString=addressString AND HotBitcoinAddresses.BitcoinChainId=bitcoinChainId) = 0)
  THEN

    INSERT INTO HotBitcoinAddresses
      (OrganizationId,BitcoinChainId,DerivationPath,AddressString,AddressStringFallback,BalanceSatoshis,ThroughputSatoshis)
    VALUES 
      (organizationId,bitcoinChainId,derivationPath,addressString,addressStringFallback,0,0);

    SELECT LAST_INSERT_ID() AS Identity;

  ELSE

    SELECT HotBitcoinAddresses.HotBitcoinAddressId AS Identity
      FROM HotBitcoinAddresses
      WHERE HotBitcoinAddresses.AddressString=addressString AND HotBitcoinAddresses.BitcoinChainId=bitcoinChainId;

  END IF;

END


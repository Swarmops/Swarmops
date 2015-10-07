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

DROP PROCEDURE IF EXISTS `UpdateHotBitcoinAddressUnspentTotals`

#

DROP PROCEDURE IF EXISTS `UpdateHotBitcoinAddressUnspentTotal`


#


CREATE PROCEDURE `UpdateHotBitcoinAddressUnspentTotal` (
  IN hotBitcoinAddressId INTEGER
)
BEGIN

  UPDATE HotBitcoinAddresses
    INNER JOIN
    (
      SELECT HotBitcoinAddressId, SUM(AmountSatoshis) AS UnspentAmountSum
      FROM HotBitcoinAddressUnspents
      GROUP BY HotBitcoinAddressId
    ) innerSelection ON HotBitcoinAddresses.HotBitcoinAddressId = innerSelection.HotBitcoinAddressId

    SET HotBitcoinAddresses.BalanceSatoshis = innerSelection.UnspentAmountSum
    WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

END


#


CREATE PROCEDURE `UpdateHotBitcoinAddressUnspentTotals` ()
BEGIN

  UPDATE HotBitcoinAddresses
    INNER JOIN
    (
      SELECT HotBitcoinAddressId, SUM(AmountSatoshis) AS UnspentAmountSum
      FROM HotBitcoinAddressUnspents
      GROUP BY HotBitcoinAddressId
    ) innerSelection ON HotBitcoinAddresses.HotBitcoinAddressId = innerSelection.HotBitcoinAddressId

    SET HotBitcoinAddresses.BalanceSatoshis = innerSelection.UnspentAmountSum;

END


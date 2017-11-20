DROP PROCEDURE IF EXISTS `UpdateHotBitcoinAddressUnspentTotals`

#

DROP PROCEDURE IF EXISTS `CreateHotBitcoinAddressUnspentConditional`

#


CREATE PROCEDURE `CreateHotBitcoinAddressUnspentConditional`(
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
      AND HotBitcoinAddressUnspents.TransactionOutputIndex=transactionOutputIndex
      AND HotBitcoinAddressUnspents.HotBitcoinAddressId=hotBitcoinAddressId) = 0)
  THEN

    INSERT INTO HotBitcoinAddressUnspents
      (HotBitcoinAddressId,AmountSatoshis,TransactionHash,TransactionOutputIndex,ConfirmationCount)
    VALUES 
      (hotBitcoinAddressId,amountSatoshis,transactionHash,transactionOutputIndex,confirmationCount);

    SELECT LAST_INSERT_ID() AS Identity;

  ELSE

    SELECT HotBitcoinAddressUnspents.HotBitcoinAddressUnspentId INTO unspentId
      FROM HotBitcoinAddressUnspents
      WHERE HotBitcoinAddressUnspents.TransactionHash=transactionHash
      AND HotBitcoinAddressUnspents.TransactionOutputIndex=transactionOutputIndex
      AND HotBitcoinAddressId=hotBitcoinAddressId;

    UPDATE HotBitcoinAddressUnspents
      SET HotBitcoinAddressUnspents.ConfirmationCount=confirmationCount
      WHERE HotBitcoinAddressUnspents.HotBitcoinAddressUnspentId=unspentId;

    SELECT -unspentId AS Identity;

  END IF;

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

    SET HotBitcoinAddresses.BalanceSatoshis = innerSelection.UnspentAmountSum
    WHERE HotBitcoinAddresses.HotBitcoinAddressId=hotBitcoinAddressId;

END


ALTER TABLE `HotBitcoinAddressUnspents` 
DROP INDEX `Ix_HashIndex` ,
ADD UNIQUE INDEX `Ix_HashIndex` (`TransactionHash` ASC, `TransactionOutputIndex` ASC, `HotBitcoinAddressId` ASC)

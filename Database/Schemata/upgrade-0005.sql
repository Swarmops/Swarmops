UPDATE `Countries` SET `PostalCodeLength`='4' WHERE `Code`='DK'

#

UPDATE `Countries` SET `PostalCodeLength`='4' WHERE `Code`='NO'

#

UPDATE `Countries` SET `PostalCodeLength`='0' WHERE `Code`='GH'

#

ALTER TABLE `Countries` ADD COLUMN `CurrencyCode` VARCHAR(8) NOT NULL DEFAULT '' AFTER `Culture`

#

UPDATE `Countries` SET `CurrencyCode`='SEK' WHERE `Code`='SE'

#

UPDATE `Countries` SET `CurrencyCode`='DKK' WHERE `Code`='DK'

#

UPDATE `Countries` SET `CurrencyCode`='NOK' WHERE `Code`='NO'

#

UPDATE `Countries` SET `CurrencyCode`='EUR' WHERE `Code`='FI'

UPDATE `Countries` SET `CurrencyCode`='EUR', `PostalCodeLength`='6' WHERE `CountryCode`='NL'

#

ALTER TABLE `PostalCodes` 
CHANGE COLUMN `Lat` `Latitude` DOUBLE NOT NULL DEFAULT 0.0 ,
CHANGE COLUMN `Long` `Longitude` DOUBLE NOT NULL DEFAULT 0.0

#

ALTER TABLE `People` 
ADD COLUMN `Latitude` DOUBLE NOT NULL DEFAULT 0.0 AFTER `GeographyId`,
ADD COLUMN `Longitude` DOUBLE NOT NULL DEFAULT 0.0 AFTER `Latitude`,
ADD INDEX `Index_Latitude` (`Latitude` ASC),
ADD INDEX `Index_Longitude` (`Longitude` ASC)

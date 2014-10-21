ALTER TABLE `activizr-dev`.`SalaryTaxLevels` 
ADD COLUMN `Year` INT(11) NOT NULL DEFAULT 2014 AFTER `Tax`,
ADD INDEX `Index_Year` (`Year` ASC)


#


CREATE PROCEDURE `CreateSalaryTaxLevel` (
  taxLevelId INTEGER,
  countryId INTEGER,
  bracketLow INTEGER,
  tax INTEGER,
  year INTEGER
)
BEGIN

  INSERT INTO SalaryTaxLevels (TaxLevelId,CountryId,BracketLow,Tax,Year)
    VALUES (taxLevelId,countryId,bracketLow,tax,year);

END


#


DROP PROCEDURE `CreateTaxLevel`


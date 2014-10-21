ALTER TABLE `activizr-dev`.`SalaryTaxLevels` 
ADD COLUMN `Year` INT(11) NOT NULL DEFAULT 2014 AFTER `Tax`,
ADD INDEX `Index_Year` (`Year` ASC)



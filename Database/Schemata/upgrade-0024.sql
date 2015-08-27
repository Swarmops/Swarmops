DROP procedure IF EXISTS `CreatePayrollItem`


#


CREATE PROCEDURE `CreatePayrollItem` (
  IN personId INT,
  IN organizationId INT,
  IN employedDate DATETIME,
  IN reportsToPersonId INT,
  IN countryId INT,
  IN baseSalaryCents INT,
  IN budgetId INT,
  IN open TINYINT,
  IN additiveTaxLevel DOUBLE,
  IN subtractiveTaxLevelId INT,
  IN isContractor TINYINT
)
BEGIN
  INSERT INTO Payroll 
	 (PersonId, OrganizationId, EmployedDate, ReportsToPersonId,
      CountryId, BaseSalaryCents, BudgetId, Open, AdditiveTaxLevel,
      SubtractiveTaxLevelId, IsContractor, TerminatedDate)
  VALUES 
     (personId, organizationId, employedDate, reportsToPersonId,
	  countryId, baseSalaryCents, budgetId, open, additiveTaxLevel,
      subtractiveTaxLevelId, isContractor, '2099-12-31');
END

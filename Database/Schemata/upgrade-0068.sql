DROP PROCEDURE IF EXISTS `SetSalaryOpen`


#


CREATE PROCEDURE `SetSalaryOpen`(
  IN salaryId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE Salaries SET Salaries.Open=open
    WHERE Salaries.SalaryId=salaryId;

END

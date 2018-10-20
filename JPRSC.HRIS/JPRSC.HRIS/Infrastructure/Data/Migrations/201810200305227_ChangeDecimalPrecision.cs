namespace JPRSC.HRIS.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDecimalPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PayrollRecords", "DaysWorkedValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "HoursWorkedValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "OvertimeValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "HoursLateValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "HoursUndertimeValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "COLADailyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "EarningsValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "DeductionsValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "SSSValueEmployee", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "SSSValueEmployer", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "PHICValueEmployee", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "PHICValueEmployer", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "PagIbigValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "TaxValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PayrollRecords", "LoanPaymentValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "COLADaily", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "COLAHourly", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "COLAMonthly", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "DailyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "HourlyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Employees", "MonthlyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "COLADailyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "COLAHourlyOTValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "DailyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "DaysWorkedValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "HourlyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "HoursLateValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "HoursUndertimeValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "HoursWorkedValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.DailyTimeRecords", "MonthlyRate", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Loans", "DeductionAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Loans", "InterestAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Loans", "PrincipalAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Loans", "PrincipalAndInterestAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Loans", "RemainingBalance", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.Overtimes", "NumberOfHoursValue", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PagIbigRecords", "EmployeeAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PagIbigRecords", "EmployerAmount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.TaxRanges", "From", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.TaxRanges", "Plus", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.TaxRanges", "To", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.EarningDeductionRecords", "Amount", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PhicRecords", "MaximumDeduction", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.PhicRecords", "MinimumDeduction", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SSSRecords", "ECC", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SSSRecords", "Employee", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SSSRecords", "Employer", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SSSRecords", "Range1", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SystemSettings", "MinimumDeductionOfContribution", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.SystemSettings", "MinimumNetPay", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SystemSettings", "MinimumNetPay", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SystemSettings", "MinimumDeductionOfContribution", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SSSRecords", "Range1", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SSSRecords", "Employer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SSSRecords", "Employee", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SSSRecords", "ECC", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PhicRecords", "MinimumDeduction", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PhicRecords", "MaximumDeduction", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.EarningDeductionRecords", "Amount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.TaxRanges", "To", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.TaxRanges", "Plus", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.TaxRanges", "From", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PagIbigRecords", "EmployerAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PagIbigRecords", "EmployeeAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Overtimes", "NumberOfHoursValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Loans", "RemainingBalance", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Loans", "PrincipalAndInterestAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Loans", "PrincipalAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Loans", "InterestAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Loans", "DeductionAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "MonthlyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "HoursWorkedValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "HoursUndertimeValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "HoursLateValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "HourlyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "DaysWorkedValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "DailyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "COLAHourlyOTValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DailyTimeRecords", "COLADailyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "MonthlyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "HourlyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "DailyRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "COLAMonthly", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "COLAHourly", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Employees", "COLADaily", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "LoanPaymentValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "TaxValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "PagIbigValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "PHICValueEmployer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "PHICValueEmployee", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "SSSValueEmployer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "SSSValueEmployee", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "DeductionsValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "EarningsValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "COLAMonthlyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "COLAHourlyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "COLADailyValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "HoursUndertimeValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "HoursLateValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "OvertimeValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "HoursWorkedValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PayrollRecords", "DaysWorkedValue", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

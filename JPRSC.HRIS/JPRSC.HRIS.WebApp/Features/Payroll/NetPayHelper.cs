namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class NetPayHelper
    {
        public static decimal GetNetPay(Models.SystemSettings systemSettings, decimal basicPay, decimal totalEarnings, decimal totalGovDeductions, decimal deductionsFromDeductions, decimal deductionsFromLoans)
        {
            var totalDeductions = basicPay >= systemSettings.MinimumDeductionOfContribution.Value ?
                totalGovDeductions + deductionsFromDeductions + deductionsFromLoans :
                deductionsFromDeductions + deductionsFromLoans;

            var netPay = totalEarnings - totalDeductions;
            if (totalDeductions <= 0 || netPay >= systemSettings.MinimumNetPay.Value) return netPay;

            var netPayWithoutLoans = totalEarnings - (totalDeductions - deductionsFromLoans);
            if (netPayWithoutLoans >= systemSettings.MinimumNetPay.Value) return netPayWithoutLoans;

            var netPayWithoutAnyDeduction = totalEarnings;
            return netPayWithoutAnyDeduction;
        }
    }
}
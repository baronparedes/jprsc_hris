namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class NetPayHelper
    {
        public static decimal GetNetPay(Models.SystemSettings systemSettings, decimal basicPay, decimal totalEarnings, decimal totalGovDeductions, decimal deductionsFromDeductions, decimal deductionsFromLoans)
        {
            return GetNetPay(systemSettings, basicPay, totalEarnings, totalGovDeductions, deductionsFromDeductions, deductionsFromLoans, out bool govDeductionsDeducted, out bool loansDeducted, out bool anythingDeducted);
        }

        public static decimal GetNetPay(Models.SystemSettings systemSettings, decimal basicPay, decimal totalEarnings, decimal totalGovDeductions, decimal deductionsFromDeductions, decimal deductionsFromLoans,
            out bool govDeductionsDeducted, out bool loansDeducted, out bool anythingDeducted)
        {
            var totalDeductions = 0m;

            if (basicPay >= systemSettings.MinimumDeductionOfContribution.Value)
            {
                govDeductionsDeducted = true;
                totalDeductions = totalGovDeductions + deductionsFromDeductions + deductionsFromLoans;
            }
            else
            {
                govDeductionsDeducted = false;
                totalDeductions = deductionsFromDeductions + deductionsFromLoans;
            }

            loansDeducted = true;
            anythingDeducted = true;

            var netPay = totalEarnings - totalDeductions;
            if (totalDeductions <= 0 || netPay >= systemSettings.MinimumNetPay.Value) return netPay;

            loansDeducted = false;

            var netPayWithoutLoans = totalEarnings - (totalDeductions - deductionsFromLoans);
            if (netPayWithoutLoans >= systemSettings.MinimumNetPay.Value) return netPayWithoutLoans;

            anythingDeducted = false;

            var netPayWithoutAnyDeduction = totalEarnings;
            return netPayWithoutAnyDeduction;
        }
    }
}
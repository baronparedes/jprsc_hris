namespace JPRSC.HRIS.Features.Payroll
{
    public class NetPayHelper
    {
        public static decimal GetNetPay(Models.SystemSettings systemSettings, decimal basicPay, decimal totalEarnings, decimal totalGovDeductions, decimal deductionsFromDeductions, decimal deductionsFromLoans,
            out bool govDeductionsDeducted, out bool loansDeducted, out bool anythingDeducted, out decimal deductionBasis)
        {
            deductionBasis = 0m;

            if (basicPay >= systemSettings.MinimumDeductionOfContribution.Value)
            {
                govDeductionsDeducted = true;
                deductionBasis = totalGovDeductions + deductionsFromDeductions + deductionsFromLoans;
            }
            else
            {
                govDeductionsDeducted = false;
                deductionBasis = deductionsFromDeductions + deductionsFromLoans;
            }

            loansDeducted = true;
            anythingDeducted = true;

            var netPay = totalEarnings - deductionBasis;
            if (deductionBasis <= 0 || netPay >= systemSettings.MinimumNetPay.Value) return netPay;

            deductionBasis = deductionBasis - deductionsFromLoans;
            loansDeducted = false;

            var netPayWithoutLoans = totalEarnings - deductionBasis;
            if (netPayWithoutLoans >= systemSettings.MinimumNetPay.Value) return netPayWithoutLoans;

            deductionBasis = 0;
            anythingDeducted = false;

            var netPayWithoutAnyDeduction = totalEarnings;

            return netPayWithoutAnyDeduction;
        }

        public static decimal GetNetPayLoanExempt(Models.SystemSettings systemSettings, decimal basicPay, decimal totalEarnings, decimal totalGovDeductions, decimal deductionsFromDeductions,
            out bool govDeductionsDeducted, out bool anythingDeducted, out decimal deductionBasis)
        {
            deductionBasis = 0m;

            if (basicPay >= systemSettings.MinimumDeductionOfContribution.Value)
            {
                govDeductionsDeducted = true;
                deductionBasis = totalGovDeductions + deductionsFromDeductions;
            }
            else
            {
                govDeductionsDeducted = false;
                deductionBasis = deductionsFromDeductions;
            }

            anythingDeducted = true;

            var netPayWithoutLoans = totalEarnings - deductionBasis;
            if (deductionBasis <= 0 || netPayWithoutLoans >= systemSettings.MinimumNetPay.Value) return netPayWithoutLoans;

            deductionBasis = 0;
            anythingDeducted = false;

            var netPayWithoutAnyDeduction = totalEarnings;

            return netPayWithoutAnyDeduction;
        }
    }
}
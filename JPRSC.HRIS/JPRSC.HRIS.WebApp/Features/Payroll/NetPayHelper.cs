namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class NetPayHelper
    {
        public static decimal GetNetPay(decimal minimumNetPay, decimal totalEarnings, decimal totalDeductions, decimal deductionsFromLoans)
        {
            var netPay = totalEarnings - totalDeductions;
            if (totalDeductions <= 0 || netPay >= minimumNetPay) return netPay;

            var netPayWithoutLoans = totalEarnings - (totalDeductions - deductionsFromLoans);
            if (netPayWithoutLoans >= minimumNetPay) return netPayWithoutLoans;

            var netPayWithoutAnyDeduction = totalEarnings;
            return netPayWithoutAnyDeduction;
        }
    }
}
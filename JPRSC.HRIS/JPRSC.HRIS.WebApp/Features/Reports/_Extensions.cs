using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JPRSC.HRIS.WebApp.Features.Reports
{
    public static class Helpers
    {
        public static async Task<List<PayrollProcessBatch>> PayrollProcessBatchesByMonthAndYear(this ApplicationDbContext _db, List<int> clientIds, int? payrollPeriodMonth, int payrollPeriodYear)
        {
            var payrollProcessBatches = payrollPeriodMonth == -1 ?
                await _db.PayrollProcessBatches
                    .Include(ppb => ppb.PayrollRecords)
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                    .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodFrom.HasValue && ppb.PayrollPeriodFrom.Value.Year == payrollPeriodYear)
                    .ToListAsync() :
                await _db.PayrollProcessBatches
                    .Include(ppb => ppb.PayrollRecords)
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                    .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodFrom.HasValue && ppb.PayrollPeriodFrom.Value.Year == payrollPeriodYear && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == payrollPeriodMonth)
                    .ToListAsync();

            if (payrollPeriodMonth == -1 || payrollPeriodMonth == 10)
            {
                var decemberPayrollPeriodOnePayrollProcessBatchesFromLastYear = await _db.PayrollProcessBatches
                    .Include(ppb => ppb.PayrollRecords)
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee))
                    .Include(ppb => ppb.PayrollRecords.Select(pr => pr.Employee.Department))
                    .Where(ppb => !ppb.DeletedOn.HasValue && clientIds.Contains(ppb.ClientId.Value) && ppb.PayrollPeriodFrom.HasValue && ppb.PayrollPeriodFrom.Value.Year == payrollPeriodYear - 1 && ppb.PayrollPeriodFrom.Value.Month == 12 && ppb.PayrollPeriodMonth.HasValue && (int)ppb.PayrollPeriodMonth == payrollPeriodMonth)
                    .ToListAsync();

                payrollProcessBatches.AddRange(decemberPayrollPeriodOnePayrollProcessBatchesFromLastYear);
            }

            return payrollProcessBatches;
        }
    }
}
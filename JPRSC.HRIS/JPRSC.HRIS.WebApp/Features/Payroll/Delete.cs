using Dapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using MediatR;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Features.Payroll
{
    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public int? PayrollProcessBatchId { get; set; }
        }

        public class CommandResult
        {
            public string Code { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<CommandResult> Handle(Command command, CancellationToken token)
            {
                var payrollProcessBatch = await _db
                    .PayrollProcessBatches
                    .SingleOrDefaultAsync(ppb => ppb.Id == command.PayrollProcessBatchId && !ppb.DeletedOn.HasValue && !ppb.EndProcessedOn.HasValue);

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deleteDailyTimeRecords = "DELETE dtr FROM DailyTimeRecords dtr " +
                        "JOIN Employees e ON dtr.EmployeeId = e.Id " +
                        "WHERE dtr.PayrollPeriodFrom = @PayrollPeriodFrom AND dtr.PayrollPeriodTo = @PayrollPeriodTo AND e.ClientId = @ClientId";
                    await connection.ExecuteAsync(deleteDailyTimeRecords, new { PayrollPeriodFrom = payrollProcessBatch.PayrollPeriodFrom, PayrollPeriodTo = payrollProcessBatch.PayrollPeriodTo, ClientId = payrollProcessBatch.ClientId });
                }

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deleteOvertimes = "DELETE o FROM Overtimes o " +
                        "JOIN Employees e ON o.EmployeeId = e.Id " +
                        "WHERE o.PayrollPeriodFrom = @PayrollPeriodFrom AND o.PayrollPeriodTo = @PayrollPeriodTo AND e.ClientId = @ClientId";
                    await connection.ExecuteAsync(deleteOvertimes, new { PayrollPeriodFrom = payrollProcessBatch.PayrollPeriodFrom, PayrollPeriodTo = payrollProcessBatch.PayrollPeriodTo, ClientId = payrollProcessBatch.ClientId });
                }

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deleteEarningDeductionRecords = "DELETE edr FROM EarningDeductionRecords edr " +
                        "JOIN Employees e ON edr.EmployeeId = e.Id " +
                        "WHERE edr.PayrollPeriodFrom = @PayrollPeriodFrom AND edr.PayrollPeriodTo = @PayrollPeriodTo AND e.ClientId = @ClientId";
                    await connection.ExecuteAsync(deleteEarningDeductionRecords, new { PayrollPeriodFrom = payrollProcessBatch.PayrollPeriodFrom, PayrollPeriodTo = payrollProcessBatch.PayrollPeriodTo, ClientId = payrollProcessBatch.ClientId });
                }

                var payrollRecords = await _db
                    .PayrollRecords
                    .AsNoTracking()
                    .Where(pr => pr.PayrollProcessBatchId == payrollProcessBatch.Id)
                    .ToListAsync();

                var payrollRecordIds = payrollRecords.Select(pr => pr.Id).ToList();

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deleteLoanDeductions = "DELETE FROM LoanDeductions " +
                        "WHERE PayrollRecordId IN @PayrollRecordIds";
                    await connection.ExecuteAsync(deleteLoanDeductions, new { PayrollRecordIds = payrollRecordIds });
                }

                var clientEmployeeIds = await _db
                    .Employees
                    .AsNoTracking()
                    .Where(e => !e.DeletedOn.HasValue && e.ClientId == payrollProcessBatch.ClientId && e.DailyRate.HasValue)
                    .Select(e => e.Id)
                    .ToListAsync();

                var activeLoans = await _db.Loans
                    .Where(l => !l.DeletedOn.HasValue && clientEmployeeIds.Contains(l.EmployeeId.Value) && !l.ZeroedOutOn.HasValue && DbFunctions.TruncateTime(l.StartDeductionDate) <= DbFunctions.TruncateTime(payrollProcessBatch.PayrollPeriodTo))
                    .ToListAsync();

                activeLoans = activeLoans.Where(l => l.LoanPayrollPeriods.Contains(payrollProcessBatch.PayrollPeriod.Value)).ToList();

                foreach (var payrollRecord in payrollRecords)
                {
                    if (!payrollRecord.LoansDeducted) continue;

                    var employeeLoans = activeLoans.Where(l => l.EmployeeId == payrollRecord.EmployeeId);

                    foreach (var loan in employeeLoans)
                    {
                        loan.RemainingBalance += loan.DeductionAmount.GetValueOrDefault();
                        loan.AmountPaid -= loan.DeductionAmount.GetValueOrDefault();
                    }
                }

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var deletePayrollRecords = "DELETE FROM PayrollRecords WHERE PayrollProcessBatchId = @PayrollProcessBatchId";
                    await connection.ExecuteAsync(deletePayrollRecords, new { PayrollProcessBatchId = command.PayrollProcessBatchId });
                }

                _db.PayrollProcessBatches.Remove(payrollProcessBatch);

                await _db.SaveChangesAsync();

                return new CommandResult();
            }
        }
    }
}
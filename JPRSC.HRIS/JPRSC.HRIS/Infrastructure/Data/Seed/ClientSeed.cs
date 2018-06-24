using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class ClientSeed
    {
        internal static Client[] Clients = new Client[]
        {
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "Code 01",
                CurrentPayrollPeriod = 1,
                DaysPerWeek = 5,
                Description = "Description",
                HoursPerDay = 8,
                Id = 1,
                Name = "Client 01",
                NumberOfPayrollPeriodsAMonth = 2,
                NumberOfWorkingDaysForThisPayrollPeriod = 15,
                PayrollCode = PayrollCode.SemiMonthly,
                PayrollPeriodFrom = new DateTime(2017, 9, 1),
                PayrollPeriodMonth = Month.September,
                PayrollPeriodTo = new DateTime(2017, 9, 15),
                TaxTable = TaxTable.SemiMonthly,
                ZeroBasic = false
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "Code 02",
                CurrentPayrollPeriod = 1,
                DaysPerWeek = 5,
                Description = "Description",
                HoursPerDay = 8,
                Id = 2,
                Name = "Client 02",
                NumberOfPayrollPeriodsAMonth = 2,
                NumberOfWorkingDaysForThisPayrollPeriod = 15,
                PayrollCode = PayrollCode.SemiMonthly,
                PayrollPeriodFrom = new DateTime(2017, 9, 1),
                PayrollPeriodMonth = Month.September,
                PayrollPeriodTo = new DateTime(2017, 9, 15),
                TaxTable = TaxTable.SemiMonthly,
                ZeroBasic = false
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "Code 03",
                CurrentPayrollPeriod = 1,
                DaysPerWeek = 5,
                Description = "Description",
                HoursPerDay = 8,
                Id = 3,
                Name = "Client 03",
                NumberOfPayrollPeriodsAMonth = 2,
                NumberOfWorkingDaysForThisPayrollPeriod = 15,
                PayrollCode = PayrollCode.SemiMonthly,
                PayrollPeriodFrom = new DateTime(2017, 9, 1),
                PayrollPeriodMonth = Month.September,
                PayrollPeriodTo = new DateTime(2017, 9, 15),
                TaxTable = TaxTable.SemiMonthly,
                ZeroBasic = false
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "Code 04",
                CurrentPayrollPeriod = 1,
                DaysPerWeek = 5,
                Description = "Description",
                HoursPerDay = 8,
                Id = 4,
                Name = "Client 04",
                NumberOfPayrollPeriodsAMonth = 2,
                NumberOfWorkingDaysForThisPayrollPeriod = 15,
                PayrollCode = PayrollCode.SemiMonthly,
                PayrollPeriodFrom = new DateTime(2017, 9, 1),
                PayrollPeriodMonth = Month.September,
                PayrollPeriodTo = new DateTime(2017, 9, 15),
                TaxTable = TaxTable.SemiMonthly,
                ZeroBasic = false
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "Code 05",
                CurrentPayrollPeriod = 1,
                DaysPerWeek = 5,
                Description = "Description",
                HoursPerDay = 8,
                Id = 5,
                Name = "Client 05",
                NumberOfPayrollPeriodsAMonth = 2,
                NumberOfWorkingDaysForThisPayrollPeriod = 15,
                PayrollCode = PayrollCode.SemiMonthly,
                PayrollPeriodFrom = new DateTime(2017, 9, 1),
                PayrollPeriodMonth = Month.September,
                PayrollPeriodTo = new DateTime(2017, 9, 15),
                TaxTable = TaxTable.SemiMonthly,
                ZeroBasic = false
            }
        };
    }
}
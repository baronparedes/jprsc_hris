using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class EarningDeductionSeed
    {
        internal static EarningDeduction[] EarningDeductions = new EarningDeduction[]
        {
            new EarningDeduction
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "ED01",
                Description = "Earning Deduction 01",
                Id = 1,
                EarningDeductionType = EarningDeductionType.Deductions
            },
            new EarningDeduction
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "ED02",
                Description = "Earning Deduction 02",
                Id = 2,
                EarningDeductionType = EarningDeductionType.Deductions
            },
            new EarningDeduction
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "ED03",
                Description = "Earning Deduction 03",
                Id = 3,
                EarningDeductionType = EarningDeductionType.Deductions
            },
            new EarningDeduction
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "ED04",
                Description = "Earning Deduction 04",
                Id = 4,
                EarningDeductionType = EarningDeductionType.Earnings
            },
            new EarningDeduction
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "ED05",
                Description = "Earning Deduction 05",
                Id = 5,
                EarningDeductionType = EarningDeductionType.Earnings
            }
        };
    }
}
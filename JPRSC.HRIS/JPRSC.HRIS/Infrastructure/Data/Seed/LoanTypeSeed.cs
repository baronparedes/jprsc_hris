using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class LoanTypeSeed
    {
        internal static LoanType[] LoanTypes = new LoanType[]
        {
            new LoanType
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "LT01",
                Description = "Loan Type 01",
                Id = 1
            },
            new LoanType
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "LT02",
                Description = "Loan Type 02",
                Id = 2
            },
            new LoanType
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "LT03",
                Description = "Loan Type 03",
                Id = 3
            },
            new LoanType
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "LT04",
                Description = "Loan Type 04",
                Id = 4
            },
            new LoanType
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "LT05",
                Description = "Loan Type 05",
                Id = 5
            }
        };
    }
}
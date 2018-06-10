using JPRSC.HRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class BankSeed
    {
        internal static Bank[] Banks = new Bank[]
        {
            new Bank
            {
                AddedOn = new DateTime(2017, 9, 1),
                AccountNumber = "Account Number 01",
                Address1 = "Address 1 01",
                Address2 = "Address 2 01",
                BatchNumber = "Batch Number 01",
                BranchCode = "Branch Code 01",
                Code = "Code 01",
                CompanyCode = "Company Code 01",
                ContactPerson = "Contact Person 01",
                Description = "Description 01",
                Id = 1,
                Name = "Name 01",
                OtherBankInfo = "Other Bank Info 01",
                Position = "Position 01"
            },
            new Bank
            {
                AddedOn = new DateTime(2027, 9, 1),
                AccountNumber = "Account Number 02",
                Address1 = "Address 1 02",
                Address2 = "Address 2 02",
                BatchNumber = "Batch Number 02",
                BranchCode = "Branch Code 02",
                Code = "Code 02",
                CompanyCode = "Company Code 02",
                ContactPerson = "Contact Person 02",
                Description = "Description 02",
                Id = 2,
                Name = "Name 02",
                OtherBankInfo = "Other Bank Info 02",
                Position = "Position 02"
            },
            new Bank
            {
                AddedOn = new DateTime(2037, 9, 1),
                AccountNumber = "Account Number 03",
                Address1 = "Address 1 03",
                Address2 = "Address 2 03",
                BatchNumber = "Batch Number 03",
                BranchCode = "Branch Code 03",
                Code = "Code 03",
                CompanyCode = "Company Code 03",
                ContactPerson = "Contact Person 03",
                Description = "Description 03",
                Id = 3,
                Name = "Name 03",
                OtherBankInfo = "Other Bank Info 03",
                Position = "Position 03"
            },
            new Bank
            {
                AddedOn = new DateTime(2047, 9, 1),
                AccountNumber = "Account Number 04",
                Address1 = "Address 1 04",
                Address2 = "Address 2 04",
                BatchNumber = "Batch Number 04",
                BranchCode = "Branch Code 04",
                Code = "Code 04",
                CompanyCode = "Company Code 04",
                ContactPerson = "Contact Person 04",
                Description = "Description 04",
                Id = 4,
                Name = "Name 04",
                OtherBankInfo = "Other Bank Info 04",
                Position = "Position 04"
            },
            new Bank
            {
                AddedOn = new DateTime(2057, 9, 1),
                AccountNumber = "Account Number 05",
                Address1 = "Address 1 05",
                Address2 = "Address 2 05",
                BatchNumber = "Batch Number 05",
                BranchCode = "Branch Code 05",
                Code = "Code 05",
                CompanyCode = "Company Code 05",
                ContactPerson = "Contact Person 05",
                Description = "Description 05",
                Id = 5,
                Name = "Name 05",
                OtherBankInfo = "Other Bank Info 05",
                Position = "Position 05"
            }
        };
    }
}

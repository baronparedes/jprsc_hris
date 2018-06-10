using JPRSC.HRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class PagIbigRecordSeed
    {
        internal static PagIbigRecord[] PagIbigRecords = new PagIbigRecord[]
        {
            new PagIbigRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                ApplyToSalary = true,
                Code = "Code 01",
                Description = "Description 01",
                EmployeeAmount = 20,
                EmployeePercentage = 50,
                EmployerAmount = 20,
                EmployerPercentage = 50,
                Id = 2,
                Name = "Name 01"
            },
            new PagIbigRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                ApplyToSalary = true,
                Code = "Code 02",
                Description = "Description 02",
                EmployeeAmount = 20,
                EmployeePercentage = 50,
                EmployerAmount = 20,
                EmployerPercentage = 50,
                Id = 2,
                Name = "Name 02"
            },
            new PagIbigRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                ApplyToSalary = true,
                Code = "Code 03",
                Description = "Description 03",
                EmployeeAmount = 20,
                EmployeePercentage = 50,
                EmployerAmount = 20,
                EmployerPercentage = 50,
                Id = 3,
                Name = "Name 03"
            },
            new PagIbigRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                ApplyToSalary = true,
                Code = "Code 04",
                Description = "Description 04",
                EmployeeAmount = 20,
                EmployeePercentage = 50,
                EmployerAmount = 20,
                EmployerPercentage = 50,
                Id = 4,
                Name = "Name 04"
            },
            new PagIbigRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                ApplyToSalary = true,
                Code = "Code 05",
                Description = "Description 05",
                EmployeeAmount = 20,
                EmployeePercentage = 50,
                EmployerAmount = 20,
                EmployerPercentage = 50,
                Id = 5,
                Name = "Name 05"
            }
        };
    }
}

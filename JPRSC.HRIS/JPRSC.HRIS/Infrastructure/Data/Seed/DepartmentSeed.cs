using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class DepartmentSeed
    {
        internal static Department[] Departments = new Department[]
        {
            new Department
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 1,
                Name = "Department 01"
            },
            new Department
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 2,
                Name = "Department 02"
            },
            new Department
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 3,
                Name = "Department 03"
            },
            new Department
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 4,
                Name = "Department 04"
            },
            new Department
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 5,
                Name = "Department 05"
            }
        };
    }
}
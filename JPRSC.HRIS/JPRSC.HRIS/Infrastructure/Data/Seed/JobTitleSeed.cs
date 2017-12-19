using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class JobTitleSeed
    {
        internal static JobTitle[] JobTitles = new JobTitle[]
        {
            new JobTitle
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 1,
                Name = "Job Title 01"
            },
            new JobTitle
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 2,
                Name = "Job Title 02"
            },
            new JobTitle
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 3,
                Name = "Job Title 03"
            },
            new JobTitle
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 4,
                Name = "Job Title 04"
            },
            new JobTitle
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 5,
                Name = "Job Title 05"
            }
        };
    }
}
using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class PHICRecordSeed
    {
        internal static PhicRecord[] PHICRecords = new PhicRecord[]
        {
            new PhicRecord
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 1,
                Percentage = 2.75,
                EmployeePercentageShare = 50
            }
        };
    }
}
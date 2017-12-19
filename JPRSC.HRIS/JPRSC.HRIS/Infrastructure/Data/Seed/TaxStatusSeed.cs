using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class TaxStatusSeed
    {
        internal static TaxStatus[] TaxStatuses = new TaxStatus[]
        {
            new TaxStatus
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 1,
                Name = "Tax Status 01"
            },
            new TaxStatus
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 2,
                Name = "Tax Status 02"
            },
            new TaxStatus
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 3,
                Name = "Tax Status 03"
            },
            new TaxStatus
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 4,
                Name = "Tax Status 04"
            },
            new TaxStatus
            {
                AddedOn = new DateTime(2017, 9, 1),
                Id = 5,
                Name = "Tax Status 05"
            }
        };
    }
}
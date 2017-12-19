using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class ReligionSeed
    {
        internal static Religion[] Religions = new Religion[]
        {
            new Religion
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "R01",
                Description = "Religion 01",
                Id = 1
            },
            new Religion
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "R02",
                Description = "Religion 02",
                Id = 2
            },
            new Religion
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "R03",
                Description = "Religion 03",
                Id = 3
            },
            new Religion
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "R04",
                Description = "Religion 04",
                Id = 4
            },
            new Religion
            {
                AddedOn = new DateTime(2017, 9, 1),
                Code = "R05",
                Description = "Religion 05",
                Id = 5
            }
        };
    }
}
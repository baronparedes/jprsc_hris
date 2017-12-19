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
                CutOffPeriod = CutOffPeriod.BiMonthly,
                DaysPerWeek = 4,
                HoursPerDay = 8,
                Id = 1,
                Name = "Client 01"
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                CutOffPeriod = CutOffPeriod.Daily,
                DaysPerWeek = 5,
                HoursPerDay = 9,
                Id = 2,
                Name = "Client 02"
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                CutOffPeriod = CutOffPeriod.Monthly,
                DaysPerWeek = 6,
                HoursPerDay = 10,
                Id = 3,
                Name = "Client 03"
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                CutOffPeriod = CutOffPeriod.Monthly,
                DaysPerWeek = 7,
                HoursPerDay = 11,
                Id = 4,
                Name = "Client 04"
            },
            new Client
            {
                AddedOn = new DateTime(2017, 9, 1),
                CutOffPeriod = CutOffPeriod.Daily,
                DaysPerWeek = 8,
                HoursPerDay = 12,
                Id = 5,
                Name = "Client 05"
            }
        };
    }
}
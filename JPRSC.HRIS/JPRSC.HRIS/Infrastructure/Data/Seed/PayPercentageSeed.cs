using JPRSC.HRIS.Models;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class PayPercentageSeed
    {
        internal static PayPercentage[] PayPercentages = new PayPercentage[]
        {
            new PayPercentage
            {
                Id = 1,
                Name = "NDOT",
                Percentage = 137.5
            },
            new PayPercentage
            {
                Id = 2,
                Name = "ROT",
                Percentage = 125
            },
            new PayPercentage
            {
                Id = 3,
                Name = "ND",
                Percentage = 10
            },
            new PayPercentage
            {
                Id = 4,
                Name = "SH",
                Percentage = 130
            },
            new PayPercentage
            {
                Id = 5,
                Name = "NDSH",
                Percentage = 143
            },
            new PayPercentage
            {
                Id = 6,
                Name = "SHOT",
                Percentage = 169
            },
            new PayPercentage
            {
                Id = 7,
                Name = "NDSHOT",
                Percentage = 185.9
            },
            new PayPercentage
            {
                Id = 8,
                Name = "LH",
                Percentage = 200
            },
            new PayPercentage
            {
                Id = 9,
                Name = "NDLH",
                Percentage = 220
            },
            new PayPercentage
            {
                Id = 10,
                Name = "LHOT",
                Percentage = 260
            },
            new PayPercentage
            {
                Id = 11,
                Name = "NDLHOT",
                Percentage = 286
            },
            new PayPercentage
            {
                Id = 12,
                Name = "DOD",
                Percentage = 130
            },
            new PayPercentage
            {
                Id = 13,
                Name = "DODOT",
                Percentage = 169
            },
            new PayPercentage
            {
                Id = 14,
                Name = "SHDOD",
                Percentage = 150
            },
            new PayPercentage
            {
                Id = 15,
                Name = "SHDODOT",
                Percentage = 195
            },
            new PayPercentage
            {
                Id = 16,
                Name = "LHDOD",
                Percentage = 260
            },
            new PayPercentage
            {
                Id = 17,
                Name = "LHDODOT",
                Percentage = 338
            },
        };
    }
}
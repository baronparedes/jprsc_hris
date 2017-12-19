using JPRSC.HRIS.Models;
using System;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class ApprovalLevelSeed
    {
        internal static ApprovalLevel[] ApprovalLevels(ApplicationDbContext context)
        {
            return new ApprovalLevel[]
            {
                new ApprovalLevel
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = 1,
                    Level = 1,
                    UserId = context.Users.SingleOrDefault(u => u.UserName == "admin01@email.com")?.Id
                },
                new ApprovalLevel
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = 2,
                    Level = 2,
                    UserId = context.Users.SingleOrDefault(u => u.UserName == "admin02@email.com")?.Id
                },
                new ApprovalLevel
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = 3,
                    Level = 3,
                    UserId = context.Users.SingleOrDefault(u => u.UserName == "admin03@email.com")?.Id
                },
                new ApprovalLevel
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = 4,
                    Level = 4,
                    UserId = context.Users.SingleOrDefault(u => u.UserName == "admin04@email.com")?.Id
                },
                new ApprovalLevel
                {
                    AddedOn = new DateTime(2017, 9, 1),
                    Id = 5,
                    Level = 5,
                    UserId = context.Users.SingleOrDefault(u => u.UserName == "admin05@email.com")?.Id
                }
            };
        }
    }
}
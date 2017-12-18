using JPRSC.HRIS.Models;
using System;

namespace JPRSC.HRIS.Infrastructure.Data.Seed
{
    public class UserSeed
    {
        internal static User[] Admins = new User[]
        {
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                Email = "admin01@email.com",
                EmailConfirmed = true,
                Id = "6d320082-cec0-4dba-98d7-fc9727ad0d7a",
                UserName = "admin01@email.com"
            }
        };
    }
}
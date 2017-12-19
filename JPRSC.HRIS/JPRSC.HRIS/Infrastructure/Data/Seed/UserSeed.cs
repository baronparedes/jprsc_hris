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
                Name = "Admin 01",
                UserName = "admin01@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                Email = "admin02@email.com",
                EmailConfirmed = true,
                Id = "4b185204-4c17-4696-baba-4e5aa411ff73",
                Name = "Admin 02",
                UserName = "admin02@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                Email = "admin03@email.com",
                EmailConfirmed = true,
                Id = "364006d9-3476-4580-8223-2363a736055e",
                Name = "Admin 03",
                UserName = "admin03@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                Email = "admin04@email.com",
                EmailConfirmed = true,
                Id = "8a68ffdf-9175-4918-8bfc-3e3e1233a16a",
                Name = "Admin 04",
                UserName = "admin04@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                Email = "admin05@email.com",
                EmailConfirmed = true,
                Id = "ca0420f4-be27-45c4-adc3-7f9112d64758",
                Name = "Admin 05",
                UserName = "admin05@email.com"
            }
        };

        internal static User[] DefaultUsers = new User[]
        {
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                CompanyId = 1,
                Email = "user01@email.com",
                EmailConfirmed = true,
                Id = "7acb829d-7351-4aec-86a9-548db30a589b",
                JobTitleId = 1,
                Name = "User 01",
                UserName = "user01@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                CompanyId = 2,
                Email = "user02@email.com",
                EmailConfirmed = true,
                Id = "bc0cb261-af8c-41d3-8a01-f56d3cc95fdc",
                JobTitleId = 2,
                Name = "User 02",
                UserName = "user02@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                CompanyId = 3,
                Email = "user03@email.com",
                EmailConfirmed = true,
                Id = "2958a4e2-fc4b-4372-b3c2-5e46945317eb",
                JobTitleId = 3,
                Name = "User 03",
                UserName = "user03@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                CompanyId = 4,
                Email = "user04@email.com",
                EmailConfirmed = true,
                Id = "3d74b450-beb2-4664-b844-7ae5d4e0543a",
                JobTitleId = 4,
                Name = "User 04",
                UserName = "user04@email.com"
            },
            new User
            {
                AddedOn = new DateTime(2017, 9, 1),
                CompanyId = 5,
                Email = "user05@email.com",
                EmailConfirmed = true,
                Id = "a78fd01a-e3bb-4232-92c3-8e0a53514c9c",
                JobTitleId = 5,
                Name = "User 05",
                UserName = "user05@email.com"
            }
        };
    }
}
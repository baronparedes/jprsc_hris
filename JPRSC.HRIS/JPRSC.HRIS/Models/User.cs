using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Models
{
    public class User : IdentityUser
    {
        public DateTime AddedOn { get; set; }
        public CompanyProfile CompanyProfile { get; set; }
        public int? CompanyProfileId { get; set; }
        public ICollection<CustomRole> CustomRoles { get; set; } = new List<CustomRole>();
        public DateTime? DeletedOn { get; set; }
        public string JobTitle { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }
}
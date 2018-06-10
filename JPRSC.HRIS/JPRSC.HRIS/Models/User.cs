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
        public ICollection<Company> AllowedCompanies { get; set; } = new List<Company>();
        public Company Company { get; set; }
        public int? CompanyId { get; set; }
        public ICollection<CustomRole> CustomRoles { get; set; } = new List<CustomRole>();
        public DateTime? DeletedOn { get; set; }
        public JobTitle JobTitle { get; set; }
        public int? JobTitleId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public TaxRecord TaxRecord { get; set; }
        public int? TaxRecordId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }
}
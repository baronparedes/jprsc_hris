using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Models
{
    public class CustomRole
    {
        public DateTime AddedOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public IEnumerable<Permission> Permissions => GetPermissions();
        public string PermissionsString { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        public void AddPermission(Permission permission)
        {
            PermissionsString = String.IsNullOrWhiteSpace(PermissionsString) ?
                permission.ToString() :
                PermissionsString + $",{permission}";
        }

        public void AddPermissions(IEnumerable<Permission> permissions)
        {
            if (permissions == null) throw new ArgumentNullException(nameof(permissions));

            permissions.ForEach(p => AddPermission(p));
        }

        public void RemovePermission(Permission permission)
        {
            if (String.IsNullOrWhiteSpace(PermissionsString)) return;

            PermissionsString = RemoveStringFromCSV(permission.ToString(), PermissionsString);
        }

        public void RemovePermissions(IEnumerable<Permission> permissions)
        {
            if (permissions == null) throw new ArgumentNullException(nameof(permissions));

            permissions.ForEach(p => RemovePermission(p));
        }

        private IEnumerable<Permission> GetPermissions()
        {
            if (String.IsNullOrWhiteSpace(PermissionsString)) return Enumerable.Empty<Permission>();

            return PermissionsString
                .Split(',')
                .Select(str => (Permission)Enum.Parse(typeof(Permission), str));
        }

        private string RemoveStringFromCSV(string stringToBeRemoved, string csv)
        {
            if (String.IsNullOrWhiteSpace(stringToBeRemoved)) throw new ArgumentNullException(nameof(stringToBeRemoved));
            if (String.IsNullOrWhiteSpace(csv)) throw new ArgumentNullException(nameof(csv));

            return csv
                .Split(',')
                .Remove(stringToBeRemoved)
                .Join(",");
        }
    }
}
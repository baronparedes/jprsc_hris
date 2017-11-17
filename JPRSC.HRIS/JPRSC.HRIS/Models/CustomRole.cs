using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Models
{
    public class CustomRole
    {
        public DateTime AddedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public IEnumerable<Permission> Permissions => GetPermissions();
        public string PermissionsString { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        public void AddPermission(Permission permission)
        {
            var permissionAsInt = (int)permission;

            if (String.IsNullOrWhiteSpace(PermissionsString))
            {
                PermissionsString = permissionAsInt.ToString();
                return;
            }

            if (!HasPermission(permission))
            {
                PermissionsString = PermissionsString + $",{permissionAsInt}";
            }
        }

        public void AddPermissions(IEnumerable<Permission> permissions)
        {
            if (permissions == null) throw new ArgumentNullException(nameof(permissions));

            permissions.ForEach(p => AddPermission(p));
        }

        public bool HasPermission(Permission permission)
        {
            var permissionAsInt = (int)permission;

            return PermissionsString == permissionAsInt.ToString() ||
                PermissionsString.Contains($",{permissionAsInt},") ||
                PermissionsString.StartsWith($"{permissionAsInt},") ||
                PermissionsString.EndsWith($",{permissionAsInt}");
        }

        public void RemovePermission(Permission permission)
        {
            if (String.IsNullOrWhiteSpace(PermissionsString)) return;

            PermissionsString = RemoveStringFromCSV(((int)permission).ToString(), PermissionsString);
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
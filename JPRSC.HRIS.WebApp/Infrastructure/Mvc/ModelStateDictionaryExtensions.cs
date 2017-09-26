using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddErrors(this ModelStateDictionary modelStateDictionary, IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                modelStateDictionary.AddModelError("", error);
            }
        }

        public static IEnumerable<string> GetAllErrors(this ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }
    }
}
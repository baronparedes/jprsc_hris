using JPRSC.HRIS.Models;
using System.Collections.Generic;

namespace JPRSC.HRIS.Domain
{
    public static class PermissionHelper
    {
        private static IList<int> _permissionValuesNotShownInMenu = new List<int> { (int)Permission.OvertimeDefault, (int)Permission.EarningDeductionRecordDefault };

        public static IList<int> PermissionValuesNotShownInMenu
        {
            get
            {
                if (_permissionValuesNotShownInMenu == null)
                {
                    _permissionValuesNotShownInMenu = new List<int> { (int)Permission.OvertimeDefault, (int)Permission.EarningDeductionRecordDefault };
                }

                return _permissionValuesNotShownInMenu;
            }
        }
    }
}
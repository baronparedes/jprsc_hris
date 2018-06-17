using System;
using System.Web.Optimization;

namespace JPRSC.HRIS.WebApp.Infrastructure.Bundling
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddAccountBundles(bundles);
            AddApprovalLevelBundles(bundles);
            AddBankBundles(bundles);
            AddClientBundles(bundles);
            AddCompanyBundles(bundles);
            AddCoreBundles(bundles);
            AddCustomRolesBundles(bundles);
            AddDepartmentBundles(bundles);
            AddEarningDeductionBundles(bundles);
            AddEmployeeBundles(bundles);
            AddJobTitleBundles(bundles);
            AddLoginBundles(bundles);
            AddPagIbigRecordBundles(bundles);
            AddPayPercentageBundles(bundles);
            AddPhicRecordBundles(bundles);
            AddReligionBundles(bundles);
            AddSSSRecordBundles(bundles);
            AddTaxStatusBundles(bundles);
        }

        private static void AddAccountBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/accounts",
                "~/wwwroot/scripts/accounts/add.js",
                "~/wwwroot/scripts/accounts/changePassword.js",
                "~/wwwroot/scripts/accounts/edit.js",
                "~/wwwroot/scripts/accounts/editOwn.js",
                "~/wwwroot/scripts/accounts/index.js");
        }

        private static void AddApprovalLevelBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/approvalLevels",
                "~/wwwroot/scripts/approvalLevels/add.js",
                "~/wwwroot/scripts/approvalLevels/edit.js",
                "~/wwwroot/scripts/approvalLevels/index.js");
        }

        private static void AddBankBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/banks",
                "~/wwwroot/scripts/banks/add.js",
                "~/wwwroot/scripts/banks/edit.js",
                "~/wwwroot/scripts/banks/index.js");
        }

        private static void AddClientBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/clients",
                "~/wwwroot/scripts/clients/add.js",
                "~/wwwroot/scripts/clients/edit.js",
                "~/wwwroot/scripts/clients/index.js");
        }

        private static void AddCompanyBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/companies",
                "~/wwwroot/scripts/companies/add.js",
                "~/wwwroot/scripts/companies/edit.js",
                "~/wwwroot/scripts/companies/index.js");
        }

        private static void AddCoreBundles(BundleCollection bundles)
        {
            bundles.AddStyleBundle("~/stylebundles/metronic-global",
                "~/wwwroot/metronic/global/plugins/font-awesome/css/font-awesome.min.css",
                "~/wwwroot/lib/simple-line-icons/css/simple-line-icons.css",
                "~/wwwroot/metronic/global/plugins/simple-line-icons/simple-line-icons.min.css",
                "~/wwwroot/metronic/global/plugins/bootstrap/css/bootstrap.min.css",
                "~/wwwroot/metronic/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css");

            bundles.AddStyleBundle("~/stylebundles/metronic-theme",
                "~/wwwroot/metronic/global/css/components.min.css",
                "~/wwwroot/metronic/global/css/plugins.min.css",
                "~/wwwroot/metronic/layouts/layout/css/layout.min.css",
                "~/wwwroot/metronic/layouts/layout/css/themes/default.min.css",
                "~/wwwroot/metronic/layouts/layout/css/custom.min.css");

            bundles.AddStyleBundle("~/stylebundles/site",
                "~/wwwroot/styles/shared/app.css");

            bundles.AddScriptBundle("~/scriptbundles/metronic-core",
                "~/wwwroot/lib/jquery/dist/jquery.min.js",
                "~/wwwroot/lib/jquery-validation/dist/jquery.validate.min.js",
                "~/wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js",
                "~/wwwroot/metronic/global/plugins/jquery-ui/jquery-ui.min.js",
                "~/wwwroot/metronic/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/wwwroot/metronic/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                "~/wwwroot/metronic/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/wwwroot/metronic/global/plugins/jquery.blockui.min.js",
                "~/wwwroot/metronic/global/plugins/js.cookie.min.js",
                "~/wwwroot/metronic/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js");

            bundles.AddScriptBundle("~/scriptbundles/angular",
                "~/wwwroot/lib/angular/angular.min.js",
                "~/wwwroot/lib/angular-bootstrap/ui-bootstrap-tpls.min.js");

            bundles.AddScriptBundle("~/scriptbundles/metronic-theme-global",
                "~/wwwroot/metronic/global/scripts/app.min.js");

            bundles.AddScriptBundle("~/scriptbundles/metronic-theme-layout",
                "~/wwwroot/metronic/layouts/layout/scripts/layout.js",
                "~/wwwroot/metronic/layouts/layout/scripts/demo.min.js",
                "~/wwwroot/metronic/layouts/global/scripts/quick-sidebar.min.js",
                "~/wwwroot/metronic/layouts/global/scripts/quick-nav.min.js");
        }

        private static void AddCustomRolesBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/customRoles",
                "~/wwwroot/scripts/customRoles/add.js",
                "~/wwwroot/scripts/customRoles/edit.js",
                "~/wwwroot/scripts/customRoles/index.js");
        }

        private static void AddDepartmentBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/departments",
                "~/wwwroot/scripts/departments/add.js",
                "~/wwwroot/scripts/departments/edit.js",
                "~/wwwroot/scripts/departments/index.js");
        }

        private static void AddEarningDeductionBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/earningDeductions",
                "~/wwwroot/scripts/earningDeductions/add.js",
                "~/wwwroot/scripts/earningDeductions/edit.js",
                "~/wwwroot/scripts/earningDeductions/index.js");
        }

        private static void AddEmployeeBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/employees",
                "~/wwwroot/scripts/employees/add.js",
                "~/wwwroot/scripts/employees/edit.js",
                "~/wwwroot/scripts/employees/index.js");
        }

        private static void AddJobTitleBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/jobTitles",
                "~/wwwroot/scripts/jobTitles/add.js",
                "~/wwwroot/scripts/jobTitles/edit.js",
                "~/wwwroot/scripts/jobTitles/index.js");
        }

        private static void AddLoginBundles(BundleCollection bundles)
        {
            bundles.AddStyleBundle("~/stylebundles/account-login",
                "~/wwwroot/metronic/global/plugins/select2/css/select2.min.css",
                "~/wwwroot/metronic/global/plugins/select2/css/select2-bootstrap.min.css",
                "~/wwwroot/metronic/pages/css/login.min.css");

            bundles.AddScriptBundle("~/scriptbundles/account-login",
                "~/wwwroot/metronic/pages/scripts/login.min.js",
                "~/wwwroot/scripts/account/login.js");
        }

        private static void AddPagIbigRecordBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/pagIbigRecords",
                "~/wwwroot/scripts/pagIbigRecords/add.js",
                "~/wwwroot/scripts/pagIbigRecords/edit.js",
                "~/wwwroot/scripts/pagIbigRecords/index.js");
        }

        private static void AddPayPercentageBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/payPercentages",
                "~/wwwroot/scripts/payPercentages/add.js",
                "~/wwwroot/scripts/payPercentages/edit.js",
                "~/wwwroot/scripts/payPercentages/index.js");
        }

        private static void AddPhicRecordBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/phicRecords",
                "~/wwwroot/scripts/phicRecords/add.js",
                "~/wwwroot/scripts/phicRecords/edit.js",
                "~/wwwroot/scripts/phicRecords/index.js");
        }

        private static void AddReligionBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/religions",
                "~/wwwroot/scripts/religions/add.js",
                "~/wwwroot/scripts/religions/edit.js",
                "~/wwwroot/scripts/religions/index.js");
        }

        private static void AddSSSRecordBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/sssRecords",
                "~/wwwroot/scripts/sssRecords/add.js",
                "~/wwwroot/scripts/sssRecords/edit.js",
                "~/wwwroot/scripts/sssRecords/index.js");
        }

        private static void AddTaxStatusBundles(BundleCollection bundles)
        {
            bundles.AddScriptBundle("~/scriptbundles/taxStatuses",
                "~/wwwroot/scripts/taxStatuses/add.js",
                "~/wwwroot/scripts/taxStatuses/edit.js",
                "~/wwwroot/scripts/taxStatuses/index.js");
        }
    }
}
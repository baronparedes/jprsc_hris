using System;
using System.Web.Optimization;

namespace JPRSC.HRIS.WebApp.Infrastructure.Bundling
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddCompanyBundles(bundles);
            AddCoreBundles(bundles);
            AddLoginBundles(bundles);
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
    }
}
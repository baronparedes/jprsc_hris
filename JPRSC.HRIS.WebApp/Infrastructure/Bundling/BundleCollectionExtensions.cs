using System.Web.Optimization;

namespace JPRSC.HRIS.WebApp.Infrastructure.Bundling
{
    public static class BundleCollectionExtensions
    {
        public static Bundle AddScriptBundle(this BundleCollection bundleCollection, string bundleVirtualPath, params string[] virtualPaths)
        {
            var scriptBundle = new ScriptBundle(bundleVirtualPath)
                .Include(virtualPaths);

            bundleCollection.Add(scriptBundle);

            scriptBundle.Orderer = new NonOrderingBundleOrderer();

            return scriptBundle;
        }

        public static Bundle AddStyleBundle(this BundleCollection bundleCollection, string bundleVirtualPath, params string[] virtualPaths)
        {
            var styleBundle = new StyleBundle(bundleVirtualPath)
                .Include(virtualPaths);

            bundleCollection.Add(styleBundle);

            styleBundle.Orderer = new NonOrderingBundleOrderer();

            return styleBundle;
        }
    }
}
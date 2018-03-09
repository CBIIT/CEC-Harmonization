using System.Web;
using System.Web.Optimization;

namespace CECHarmonization
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                   "~/Scripts/jquery.unobtrusive*",
                   "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/AwesomeMvc").Include(
                                  "~/Scripts/AwesomeMvc.js"));

            bundles.Add(new ScriptBundle("~/bundles/jstree").Include(
                                  "~/Scripts/jstree.min.js",
                                  "~/Scripts/libs/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular")
               .Include("~/Scripts/angular.min.js",
                        "~/Scripts/angular-route.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularUi")
               .Include("~/Scripts/angular-ui/ui-bootstrap-tpls-0.12.1.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/AwesomeMvc").Include(
                      "~/Content/themes/compact/AwesomeMvc.css"));

            bundles.Add(new StyleBundle("~/Content/jstree").Include(
                                  "~/Content/jstree/themes/default/style.min.css"));




            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}

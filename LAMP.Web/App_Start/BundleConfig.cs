using System.Web;
using System.Web.Optimization;

namespace LAMP.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(

                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Javascript files bundle
            bundles.Add(new ScriptBundle("~/bundles/javascripts").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/Default/css/public_lampcss").Include(
                      "~/Content/Default/css/bootstrap.min.css",
                      "~/Content/Default/css/lamp.css",
                      "~/Content/Default/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/Default/css/lampcss").Include(
                      "~/Content/Default/css/bootstrap.min.css",
                      "~/Content/Default/css/jquery.datepick.css",
                       "~/Content/Default/css/bootstrap-datetimepicker.css",
                       "~/Content/Default/css/bootstrap-select.min.css",
                      "~/Content/Default/css/lamp.css",
                      "~/Content/Default/css/style.css"));

            bundles.Add(new ScriptBundle("~/Content/Default/javascripts").Include(
                              "~/Content/Default/js/jquery-3.1.1.min.js",
                              "~/Content/Default/js/bootstrap.min.js",
                              "~/Content/Default/js/jquery.plugin.js",
                              "~/Content/Default/js/jquery.datepick.js"
                              ));

        }
    }
}

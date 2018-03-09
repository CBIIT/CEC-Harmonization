using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;

namespace CECHarmonization
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected override IKernel CreateKernel()
        //{
        //    var kernel = new StandardKernel();
        //    RegisterServices(kernel);
        //    return kernel;
        //}

        ///// <summary>
        ///// Load your modules or register your services here!
        ///// </summary>
        ///// <param name="kernel">The kernel.</param>
        //private void RegisterServices(IKernel kernel)
        //{
        //    // e.g. kernel.Load(Assembly.GetExecutingAssembly());
        //}

        //protected override void OnApplicationStarted()
        //{
        //    base.OnApplicationStarted();

        //    AreaRegistration.RegisterAllAreas();
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //}

        void Application_BeginRequest(Object source, EventArgs e)

        {

            HttpApplication app = (HttpApplication)source;

            HttpContext context = app.Context;

            // Attempt to peform first request initialization

            //HttpContext.Current.Response.Redirect("~\\Views\\Account\\InvalidAccess.cshtml");
            //HttpContext.Current.Response.RedirectToRoute("InvalidAccess");

           

            FirstRequestInitialization.Initialize(context);

        }


        class FirstRequestInitialization

{

    private static bool s_InitializedAlready = false;

        private static Object s_lock = new Object();

        // Initialize only on the first request

        public static void Initialize(HttpContext context)

        {

            if (s_InitializedAlready)

            {

                return;

            }

            lock (s_lock)

            {

                if (s_InitializedAlready)

                {

                    return;

                }

                    // Perform first-request initialization here …
                    if (HttpContext.Current.Request.Url.ToString().Contains("localhost"))
                    {
                        s_InitializedAlready = true;
                    }
                    else
                    {
                        if (HttpContext.Current.Request.QueryString.ToString() != "cectool=1")
                        {
                            HttpContext.Current.RewritePath("Account/InvalidAccess");
                            s_InitializedAlready = false;
                        }
                        else
                        {
                            s_InitializedAlready = true;
                        }
                    }

                }

        }

    }

}
}

using FulaDjur.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FulaDjur
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //CreateBasicAccounts();
        }

        private void CreateBasicAccounts()
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            if (!rm.RoleExists("Admin"))
            {

                rm.Create(new IdentityRole("Admin"));

                var admin = new ApplicationUser()
                {
                    UserName = "admin@fuladjur.se"
                };

                if (um.FindByName(admin.UserName) == null)
                {
                    um.Create(admin, "password");
                    um.AddToRole(admin.Id, "Admin");
                }

            }

        }
    }
}

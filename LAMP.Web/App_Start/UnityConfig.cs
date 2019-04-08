using LAMP.DataAccess;
using LAMP.Service;
using LAMP.Utility;
using LAMP.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using Unity.Mvc5;
namespace LAMP.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountService, AccountService>(new HierarchicalLifetimeManager());                   
            container.RegisterType<ICustomAuthentication, CustomAuthentication>(new HierarchicalLifetimeManager());   
            container.RegisterType<IAdminService, AdminService>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserAdminService, UserAdminService>();
            container.RegisterType<IScheduleService, BatchScheduleService>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        }
    }
}

 
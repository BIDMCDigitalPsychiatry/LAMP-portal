using Microsoft.Practices.Unity;
using System;
using System.Web;
using System.Web.Http;
using LAMP.DataAccess;
using LAMP.Service;
namespace LAMP.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);            
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountService, AccountService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthentication, Authentication>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthenticationContext, AuthenticationContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISurveyService, SurveyService>(new HierarchicalLifetimeManager());            
            container.RegisterType<Func<HttpContextBase>>(new InjectionFactory(a => new Func<HttpContextBase>(() => new HttpContextWrapper(HttpContext.Current))));                 
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}   
using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Web.Mvc;
using ServiceStack.Configuration;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Redis;
using Nuxleus.Data;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ServiceStack.Plugins.ProtoBuf.AppStart), "Start")]
[assembly: WebActivator.PreApplicationStartMethod(typeof(Nuxleus.Web.MvcWebApp.App_Start.AppHost), "Start")]

//IMPORTANT: Add the line below to MvcApplication.RegisterRoutes(RouteCollection) in the Global.asax:
//routes.IgnoreRoute("api/{*pathInfo}"); 

/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace Nuxleus.Web.MvcWebApp.App_Start
{
    //A customizeable typed UserSession that can be extended with your own properties
    //To access ServiceStack's Session, Cache, etc from MVC Controllers inherit from ControllerBase<CustomUserSession>
    public class CustomUserSession : AuthUserSession
    {
        public string CustomProperty { get; set; }
    }

    public class AppHost
      : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services
            : base("Nuxleus Messaging Server API", typeof(Nuxleus.Core.IAgent).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            System.Environment.SetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT", "yes");
            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            //Configure User Defined REST Paths
            //Routes
            //  .Add<Hello>("/hello")
            //  .Add<Hello>("/hello/{Name*}")
            //  .Add<Todo>("/todos")
            //  .Add<Todo>("/todos/{Id}");

            //Permit modern browsers (e.g. Firefox) to allow sending of any REST HTTP Method
             base.SetConfig(new EndpointHostConfig
             {
                 DebugMode = true,
                 GlobalResponseHeaders =
                 {
                     { "Access-Control-Allow-Origin", "*" },
                     { "Access-Control-Allow-Methods", "GET, HEAD, POST, PUT, DELETE, OPTIONS" },
                 },
 
             });
             container.Register<IRedisClientsManager>(c => new PooledRedisClientManager());

            container.Register<IDataRepository>(c => new HybridDataRepository(c.Resolve<IRedisClientsManager>()));
            container.Register<ICacheClient>(c => new RedisClient());

            //Enable Authentication
            //ConfigureAuth(container);

            //Register In-Memory Cache provider. 
            //For Distributed Cache Providers Use: PooledRedisClientManager, BasicRedisClientManager or see: https://github.com/ServiceStack/ServiceStack/wiki/Caching

            container.Register<ISessionFactory>(c =>
              new SessionFactory(c.Resolve<ICacheClient>()));

            //Set MVC to use the same Funq IOC as ServiceStack
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
        }

        // Uncomment to enable ServiceStack Authentication and CustomUserSession
        //private void ConfigureAuth(Funq.Container container)
        //{
        //  var appSettings = new AppSettings();

        //  //Default route: /auth/{provider}
        //  Plugins.Add(new AuthFeature(() => new CustomUserSession(),
        //    new IAuthProvider[] {
        //      new CredentialsAuthProvider(appSettings), 
        //      new FacebookAuthProvider(appSettings), 
        //      new TwitterAuthProvider(appSettings), 
        //      new BasicAuthProvider(appSettings), 
        //    })); 

        //  //Default route: /register
        //  Plugins.Add(new RegistrationFeature()); 

        //  //Requires ConnectionString configured in Web.Config
        //  var connectionString = ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString;
        //  container.Register<IDbConnectionFactory>(c =>
        //    new OrmLiteConnectionFactory(connectionString, SqlServerOrmLiteDialectProvider.Instance));

        //  container.Register<IUserAuthRepository>(c =>
        //    new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

        //  var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
        //  authRepo.CreateMissingTables();
        //}


        public static void Start()
        {
            new AppHost().Init();
        }
    }
}
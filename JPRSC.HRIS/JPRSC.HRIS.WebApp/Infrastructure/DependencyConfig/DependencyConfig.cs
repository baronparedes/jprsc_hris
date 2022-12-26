using FluentValidation;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.Infrastructure.CSV;
using JPRSC.HRIS.Infrastructure.Excel;
using JPRSC.HRIS.WebApp.Infrastructure.Mapping;
using JPRSC.HRIS.Infrastructure.MediatR;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Logging;

namespace JPRSC.HRIS.WebApp.Infrastructure.Dependency
{
    public class DependencyConfig : IServiceProvider
    {
        private static readonly Lazy<DependencyConfig> lazy = new Lazy<DependencyConfig>(() => new DependencyConfig());

        private DependencyConfig()
        {
            Container = ConfigureContainer();
        }

        public static DependencyConfig Instance { get { return lazy.Value; } }

        public Container Container { get; private set; }

        public Container ConfigureContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Register<ApplicationDbContext, ApplicationDbContext>(Lifestyle.Scoped);
            RegisterValidators(container);
            RegisterMediatR(container);
            container.Register<MapperProvider>(Lifestyle.Singleton);
            container.RegisterSingleton(() => GetMapper(container));
            container.Register<SignInManager>(GetSignInManager, Lifestyle.Scoped);
            container.Register<UserManager>(GetUserManager, Lifestyle.Scoped);
            container.Register<IAuthenticationManager>(GetAuthenticationManager, Lifestyle.Scoped);
            container.Register<IMVCLogger, MVCLogger>(Lifestyle.Singleton);
            container.Register<IExcelBuilder, OpenXMLExcelBuilder>(Lifestyle.Scoped);
            container.Register<ICSVBuilder, CSVBuilder>(Lifestyle.Scoped);

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            return container;
        }

        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)Container).GetService(serviceType);
        }

        private static SignInManager GetSignInManager()
        {
            return HttpContext.Current.GetOwinContext().Get<SignInManager>();
        }

        private static UserManager GetUserManager()
        {
            return HttpContext.Current.GetOwinContext().Get<UserManager>();
        }

        private static IAuthenticationManager GetAuthenticationManager()
        {
            return HttpContext.Current.GetOwinContext().Authentication;
        }

        // More info: https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples.SimpleInjector/Program.cs
        private static void RegisterMediatR(Container container)
        {
            var webAssembly = Assembly.GetExecutingAssembly();
            var coreAssembly = typeof(ApplicationDbContext).Assembly;
            var assemblies = new[] { webAssembly, coreAssembly };
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var notificationHandlerTypes = container.GetTypesToRegister(typeof(INotificationHandler<>), assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });
            container.Register(typeof(INotificationHandler<>), notificationHandlerTypes);

            container.Register(() => (TextWriter)(new WrappingWriter(Console.Out)), Lifestyle.Singleton);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
                typeof(GenericPipelineBehavior<,>)
            });
            container.Collection.Register(typeof(IRequestPreProcessor<>), new[] { typeof(GenericRequestPreProcessor<>) });
            container.Collection.Register(typeof(IRequestPostProcessor<,>), new[] { typeof(GenericRequestPostProcessor<,>) });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static void RegisterValidators(Container container)
        {
            var assemblyOfValidationClasses = Assembly.GetExecutingAssembly();
            container.Register(typeof(IValidator<>), new[] { Assembly.GetExecutingAssembly() });
        }

        private AutoMapper.IMapper GetMapper(Container container)
        {
            var mp = container.GetInstance<MapperProvider>();
            return mp.GetMapper();
        }
    }
}
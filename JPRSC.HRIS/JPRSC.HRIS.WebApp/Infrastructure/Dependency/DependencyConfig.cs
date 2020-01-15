﻿using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.Infrastructure.Identity;
using JPRSC.HRIS.WebApp.Infrastructure.Excel;
using JPRSC.HRIS.WebApp.Infrastructure.Logging;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Text;
using JPRSC.HRIS.WebApp.Infrastructure.CSV;

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

            container.Register<ApplicationDbContext, ApplicationDbContext>(Lifestyle.Scoped);
            RegisterValidators(container);
            RegisterMediatR(container);
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
            var assemblyOfMediatRClasses = Assembly.GetExecutingAssembly();
            var assemblies = new[] { assemblyOfMediatRClasses };
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
    }

    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericPipelineBehavior.cs
    public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly TextWriter _writer;

        public GenericPipelineBehavior(TextWriter writer)
        {
            _writer = writer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _writer.WriteLine("-- Handling Request");
            var response = await next();
            _writer.WriteLine("-- Finished Request");
            return response;
        }
    }

    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericRequestPostProcessor.cs
    public class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly TextWriter _writer;

        public GenericRequestPostProcessor(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Process(TRequest request, TResponse response)
        {
            _writer.WriteLine("- All Done");
            return Task.FromResult(0);
        }
    }

    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericRequestPreProcessor.cs
    public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly TextWriter _writer;

        public GenericRequestPreProcessor(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _writer.WriteLine("- Starting Up");
            return Task.FromResult(0);
        }
    }

    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/Runner.cs
    public class WrappingWriter : TextWriter
    {
        private readonly TextWriter _innerWriter;
        private readonly StringBuilder _stringWriter = new StringBuilder();

        public WrappingWriter(TextWriter innerWriter)
        {
            _innerWriter = innerWriter;
        }

        public override void Write(char value)
        {
            _stringWriter.Append(value);
            _innerWriter.Write(value);
        }

        public override Task WriteLineAsync(string value)
        {
            _stringWriter.AppendLine(value);
            return _innerWriter.WriteLineAsync(value);
        }

        public override Encoding Encoding => _innerWriter.Encoding;

        public string Contents => _stringWriter.ToString();
    }
}
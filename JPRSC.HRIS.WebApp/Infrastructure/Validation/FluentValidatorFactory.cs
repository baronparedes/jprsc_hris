using FluentValidation;
using System;

namespace JPRSC.HRIS.WebApp.Infrastructure.Validation
{
    public class FluentValidatorFactory : ValidatorFactoryBase
    {
        private IServiceProvider Injector { get; set; }

        public FluentValidatorFactory(IServiceProvider injector)
        {
            Injector = injector;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return Injector.GetService(validatorType) as IValidator;
        }
    }
}
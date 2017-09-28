using FluentValidation.Mvc;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;

namespace JPRSC.HRIS.WebApp.Infrastructure.Validation
{
    public class ValidationConfig
    {
        public static void Configure()
        {
            var injector = DependencyConfig.Instance;
            FluentValidationModelValidatorProvider.Configure(
                provider =>
                {
                    provider.ValidatorFactory = new FluentValidatorFactory(injector);
                }
            );
        }
    }
}
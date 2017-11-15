using System;

namespace FluentValidation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> MustBeANumber<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .Must(p =>
                {
                    var asString = Convert.ToString(p);

                    // Let another validator handle emptiness
                    if (String.IsNullOrWhiteSpace(asString)) return true;

                    return Int32.TryParse(asString, out int i);
                })
                .WithMessage("Please enter a number.");
        }
    }
}
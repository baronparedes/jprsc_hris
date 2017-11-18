using HtmlTags;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Html
{
    public static class HtmlHelperExtensionscs
    {
        public static IHtmlString PasswordBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null, string placeholder = null)
        {
            return TextBoxHorizontalFormGroup(helper, propertyName, "password", labelText, placeholder);
        }

        public static IHtmlString TextBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null, string placeholder = null)
        {
            return TextBoxHorizontalFormGroup(helper, propertyName, "text", labelText, placeholder);
        }

        private static IHtmlString TextBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string type, string labelText = null, string placeholder = null)
        {
            if (String.IsNullOrWhiteSpace(labelText))
            {
                labelText = propertyName.Humanize();
            }

            var camelCasePropertyName = GetCamelCasePropertyName(propertyName);

            var formGroup = new HtmlTag("div");
            formGroup.AddClass("form-group");
            formGroup.Attr("ng-class", $"{{ 'has-error': vm.validationErrors.{camelCasePropertyName} }}");

            var label = new HtmlTag("label");
            label.AddClasses("col-md-3", "col-lg-2", "control-label");
            label.Text(labelText);

            var inputContainer = new HtmlTag("div");
            inputContainer.AddClasses("col-md-9", "col-lg-10");

            var input = new HtmlTag("input");
            input.Attr("type", type);
            input.AddClass("form-control");
            input.Attr("name", propertyName);
            input.Attr("value", GetPropertyValue(helper.ViewData.Model, propertyName));

            if (!String.IsNullOrWhiteSpace(placeholder))
            {
                input.Attr("placeholder", placeholder);
            }

            var helpBlock = new HtmlTag("span");
            helpBlock.AddClass("help-block");
            helpBlock.Attr("ng-show", $"vm.validationErrors.{camelCasePropertyName}");
            helpBlock.Text($"{{{{vm.validationErrors.{camelCasePropertyName}.join(' ')}}}}");

            inputContainer.Append(input);
            inputContainer.Append(helpBlock);

            formGroup.Append(label);
            formGroup.Append(inputContainer);

            return new MvcHtmlString(formGroup.ToHtmlString());
        }

        private static string GetPropertyValue(object model, string propertyName)
        {
            var property = model.GetType().GetProperty(propertyName);

            var propertyValue = property.GetValue(model);

            var propertyValueAsString = Convert.ToString(propertyValue);

            if (DateTime.TryParse(propertyValueAsString, out DateTime d))
            {
                propertyValueAsString = String.Format("{0:M/d/yyyy}", d);
            }

            return propertyValueAsString;
        }

        private static string GetCamelCasePropertyName(string propertyName)
        {
            var test = new Dictionary<string, string>
            {
                { propertyName, "Test" }
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(test, settings);

            return json.Substring(json.IndexOf("\"") + 1, json.IndexOf(":") - 3);
        }
    }
}
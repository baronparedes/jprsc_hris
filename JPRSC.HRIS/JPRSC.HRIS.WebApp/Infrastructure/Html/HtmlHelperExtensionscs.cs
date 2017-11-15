using HtmlTags;
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
        public static IHtmlString TextBoxHorizontalFormGroup(this HtmlHelper helper, string propertyName, string labelText = null, string placeholder = null)
        {
            if (String.IsNullOrWhiteSpace(labelText))
            {
                labelText = propertyName;
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
            input.Attr("type", "text");
            input.AddClass("form-control");
            input.Attr("name", propertyName);

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
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
        public static IHtmlString CheckboxSelectListItemHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, IList<SelectListItem> items, string propertyName, string labelText = null, string placeholder = null)
        {
            return HorizontalFormGroup(helper, propertyName, "checkboxlist", items, null, labelText, placeholder);
        }

        public static IHtmlString DropdownHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string lookupListName, string propertyName, string labelText = null, string placeholder = null)
        {
            return HorizontalFormGroup(helper, propertyName, "dropdown", null, lookupListName, labelText, placeholder);
        }

        public static IHtmlString PasswordBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null, string placeholder = null)
        {
            return HorizontalFormGroup(helper, propertyName, "password", null, null, labelText, placeholder);
        }

        public static IHtmlString TextBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null, string placeholder = null)
        {
            return HorizontalFormGroup(helper, propertyName, "text", null, null, labelText, placeholder);
        }

        private static HtmlTag CheckboxListControl<TModel>(HtmlHelper<TModel> helper, string propertyName, IList<SelectListItem> items)
        {
            var checkBoxList = new HtmlTag("div");
            checkBoxList.AddClass("mt-checkbox-list");

            for (var i = 0; i < items.Count; i++)
            {
                var hiddenValueInput = new HtmlTag("input");
                hiddenValueInput.Attr("type", "hidden");
                hiddenValueInput.Attr("name", $"{propertyName}[{i}].Value");
                hiddenValueInput.Attr("value", items[i].Value);

                var hiddenTextInput = new HtmlTag("input");
                hiddenTextInput.Attr("type", "hidden");
                hiddenTextInput.Attr("name", $"{propertyName}[{i}].Text");
                hiddenTextInput.Attr("value", items[i].Text);

                var label = new HtmlTag("label");
                label.AddClasses("mt-checkbox", "mt-checkbox-outline");

                var checkbox = new HtmlTag("input");
                checkbox.Attr("type", "checkbox");
                checkbox.Attr("name", $"{propertyName}[{i}].Selected");
                checkbox.Attr("value", "true");
                checkbox.AppendText(items[i].Text);

                if (items[i].Selected)
                {
                    checkbox.Attr("checked", "checked");
                }

                var span = new HtmlTag("span");

                label.Append(checkbox);
                label.Append(span);

                checkBoxList.Append(hiddenValueInput);
                checkBoxList.Append(hiddenTextInput);
                checkBoxList.Append(label);
            }

            return checkBoxList;
        }

        private static HtmlTag DropdownControl<TModel>(HtmlHelper<TModel> helper, string propertyName, string lookupListName)
        {
            /*
            <input type="hidden" name="CutOffPeriod" value="{{vm.cutOffPeriod.value}}" />
            <select class="form-control" ng-model="vm.cutOffPeriod" ng-options="cutOffPeriod.text for cutOffPeriod in vm.lookups.cutOffPeriods track by cutOffPeriod.value"></select>
            <span ng-show="vm.validationErrors.cutOffPeriod" class="help-block">{{vm.validationErrors.cutOffPeriod.join(' ')}}</span>
            */

            var camelCasePropertyName = GetCamelCasePropertyName(propertyName);

            var input = new HtmlTag("input");
            input.Attr("type", "hidden");
            input.Attr("name", propertyName);
            input.Attr("value", $"{{{{vm.{camelCasePropertyName}.value}}}}");

            var select = new HtmlTag("select");
            select.AddClass("form-control");
            select.Attr("ng-model", $"vm.{camelCasePropertyName}");
            select.Attr("ng-options", $"item.text for item in vm.lookups.{lookupListName} track by item.value");

            var helpBlock = HelpBlock(camelCasePropertyName);

            select.After(helpBlock);

            input.After(select);

            return input;
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

        private static IHtmlString HorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string type, IList<SelectListItem> items, string lookupListName, string labelText, string placeholder)
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

            var controlContainer = new HtmlTag("div");
            controlContainer.AddClasses("col-md-9", "col-lg-10");

            HtmlTag control = null;

            if (type == "text" || type == "password")
            {
                control = InputControl(helper, propertyName, type, placeholder, camelCasePropertyName);
            }
            else if (type == "checkboxlist")
            {
                control = CheckboxListControl(helper, propertyName, items);
            }
            else if (type == "dropdown")
            {
                control = DropdownControl(helper, propertyName, lookupListName);
            }

            controlContainer.Append(control);

            formGroup.Append(label);
            formGroup.Append(controlContainer);

            return new MvcHtmlString(formGroup.ToHtmlString());
        }

        private static HtmlTag InputControl<TModel>(HtmlHelper<TModel> helper, string propertyName, string type, string placeholder, string camelCasePropertyName)
        {
            var control = new HtmlTag("input");
            control.Attr("type", type);
            control.AddClass("form-control");
            control.Attr("name", propertyName);
            control.Attr("value", GetPropertyValue(helper.ViewData.Model, propertyName));

            if (!String.IsNullOrWhiteSpace(placeholder))
            {
                control.Attr("placeholder", placeholder);
            }

            var helpBlock = HelpBlock(camelCasePropertyName);

            control.After(helpBlock);

            return control;
        }

        private static HtmlTag HelpBlock(string camelCasePropertyName)
        {
            var helpBlock = new HtmlTag("span");
            helpBlock.AddClass("help-block");
            helpBlock.Attr("ng-show", $"vm.validationErrors.{camelCasePropertyName}");
            helpBlock.Text($"{{{{vm.validationErrors.{camelCasePropertyName}.join(' ')}}}}");

            return helpBlock;
        }
    }
}
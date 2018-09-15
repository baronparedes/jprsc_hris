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
    public static class HtmlHelperExtensions
    {
        public static IHtmlString CheckboxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "checkbox", null, null, labelText, null);
        }

        public static IHtmlString CheckboxSelectListItemHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, IList<SelectListItem> items, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "checkboxlist", items, null, labelText, null);
        }

        public static IHtmlString DropdownHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string lookupListName, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "dropdown", null, lookupListName, labelText, null);
        }

        public static IHtmlString DropdownHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, IList<SelectListItem> items, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "dropdown", items, null, labelText, null);
        }

        public static IHtmlString PasswordBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "password", null, null, labelText, null);
        }

        public static IHtmlString StaticHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null)
        {
            return HorizontalFormGroup(helper, propertyName, "static", null, null, labelText, null);
        }

        public static IHtmlString TextBoxHorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string labelText = null, string placeholder = null)
        {
            return HorizontalFormGroup(helper, propertyName, "text", null, null, labelText, placeholder);
        }

        private static HtmlTag CheckboxControl<TModel>(HtmlHelper<TModel> helper, string propertyName, string labelText)
        {
            var checkBoxList = new HtmlTag("div");
            checkBoxList.AddClass("mt-checkbox-list");
            checkBoxList.Attr("style", "padding-top: 8px;");

            var label = new HtmlTag("label");
            label.AddClasses("mt-checkbox", "mt-checkbox-outline");
            label.Attr("style", "margin-bottom: 0px;");

            var checkbox = new HtmlTag("input");
            checkbox.Attr("type", "checkbox");
            checkbox.Attr("name", propertyName);
            checkbox.Attr("value", "true");
            checkbox.AppendText(String.Empty);

            var propertyValue = GetPropertyValue(helper.ViewData.Model, propertyName);

            if (bool.TryParse(propertyValue, out bool isTrue) && isTrue)
            {
                checkbox.Attr("checked", "checked");
            }

            var span = new HtmlTag("span");

            label.Append(checkbox);
            label.Append(span);

            checkBoxList.Append(label);

            return checkBoxList;
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

        private static HtmlTag DropdownControl<TModel>(HtmlHelper<TModel> helper, string propertyName, IList<SelectListItem> items)
        {
            var camelCasePropertyName = GetCamelCasePropertyName(propertyName);
            var propertyValue = GetPropertyValue(helper.ViewData.Model, propertyName);

            var select = new HtmlTag("select");
            select.AddClass("form-control");
            select.Attr("name", propertyName);

            foreach (var item in items)
            {
                var option = new HtmlTag("option");
                option.Attr("value", item.Value);
                option.AppendText(item.Text);

                if (propertyValue == item.Value)
                {
                    option.Attr("selected", "selected");
                }

                select.Append(option);
            }

            var helpBlock = HelpBlock(camelCasePropertyName);

            select.After(helpBlock);

            return select;
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

            if (property.PropertyType.Name == "DateTime" || (property.PropertyType.Name == "Nullable`1" && Nullable.GetUnderlyingType(property.PropertyType).Name == "DateTime"))
            {
                if (DateTime.TryParse(propertyValueAsString, out DateTime d))
                {
                    propertyValueAsString = String.Format("{0:M/d/yyyy}", d);
                }
            }

            return propertyValueAsString;
        }

        private static IHtmlString HorizontalFormGroup<TModel>(this HtmlHelper<TModel> helper, string propertyName, string type, IList<SelectListItem> items, string lookupListName, string labelText, string placeholder)
        {
            if (String.IsNullOrWhiteSpace(labelText))
            {
                labelText = propertyName.Humanize().Titleize();
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

            switch (type)
            {
                case "text":
                case "password":
                    control = InputControl(helper, propertyName, type, placeholder, camelCasePropertyName);
                    break;
                case "checkbox":
                    control = CheckboxControl(helper, propertyName, labelText);
                    break;
                case "checkboxlist":
                    control = CheckboxListControl(helper, propertyName, items);
                    break;
                case "dropdown":
                    if (!String.IsNullOrWhiteSpace(lookupListName))
                    {
                        control = DropdownControl(helper, propertyName, lookupListName);
                    }
                    else
                    {
                        control = DropdownControl(helper, propertyName, items);
                    }
                    break;
                case "static":
                    control = StaticControl(helper, propertyName, labelText);
                    break;
                default:
                    break;
            }

            controlContainer.Append(control);

            formGroup.Append(label);
            formGroup.Append(controlContainer);

            return new MvcHtmlString(formGroup.ToHtmlString());
        }

        private static HtmlTag StaticControl<TModel>(HtmlHelper<TModel> helper, string propertyName, string labelText)
        {
            var control = new HtmlTag("p");
            control.AddClass("form-control-static");

            var propertyType = helper.ViewData.Model.GetType().GetProperty(propertyName).PropertyType;
            var propertyValue = GetPropertyValue(helper.ViewData.Model, propertyName);

            if (propertyType.Name == "Boolean" || (propertyType.Name == "Nullable`1" && Nullable.GetUnderlyingType(propertyType).Name == "Boolean"))
            {
                if (propertyValue == "True")
                {
                    var icon = new HtmlTag("i");
                    icon.AddClasses("fa", "fa-check");
                    control.Append(icon);
                }
            }
            else
            {
                control.Text(propertyValue);
            }

            return control;
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
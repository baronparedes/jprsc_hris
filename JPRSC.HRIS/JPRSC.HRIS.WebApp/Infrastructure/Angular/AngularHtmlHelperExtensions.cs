using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Angular
{
    public static class AngularHtmlHelperExtensions
    {
        // Usage:
        // 
        // HTML:
        // <div ng-controller="..." ng-init='vm.serverModel = @Html.NgInit(Model)'>
        // 
        // JS:
        // $timeout(function() {
        //     vm.myObject = vm.serverModel;
        // });
        public static IHtmlString NgInit<T>(this HtmlHelper<T> helper, object model)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var jsonEncoded = JsonConvert.SerializeObject(model, jsonSerializerSettings);

            return helper.Raw(jsonEncoded);
        }
    }
}
using Dapper;
using JPRSC.HRIS.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.AuditTrail
{
    public class AuditTrailAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var httpMethodsToLog = new string[] { "POST" };
                var httpMethod = filterContext.RequestContext.HttpContext.Request.HttpMethod;
                if (!httpMethodsToLog.Contains(httpMethod)) return;

                var actionNamesToLog = new string[] { "Add", "Edit", "Delete" };
                var actionName = filterContext.ActionDescriptor.ActionName;
                if (!actionNamesToLog.Contains(actionName)) return;

                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                var username = filterContext.HttpContext.User.Identity.Name;

                var idParameter = GetIdParameter(filterContext);

                using (var connection = new SqlConnection(ConnectionStrings.ApplicationDbContext))
                {
                    var insertAuditTrailCommand =
                        "INSERT INTO [dbo].[AuditTrailEntries] ([Action],[AddedOn],[Module],[RecordId],[UserName]) " +
                        "VALUES(@Action,GETDATE(),@Module,@RecordId,@UserName)";
                    var args = new { Action = actionName, Module = controllerName, RecordId = idParameter, UserName = username };

                    connection.Execute(insertAuditTrailCommand, args);
                }
            }
            catch
            {
            }
        }

        private static int? GetIdParameter(ActionExecutingContext filterContext)
        {
            int? idParameter = null;

            var parameter = filterContext.ActionParameters.SingleOrDefault();

            var command = parameter.Value;

            var myType = command.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                var propValue = prop.GetValue(command, null);

                if (prop.Name == "Id")
                {
                    idParameter = Convert.ToInt32(propValue);
                    break;
                }
            }

            return idParameter;
        }
    }
}
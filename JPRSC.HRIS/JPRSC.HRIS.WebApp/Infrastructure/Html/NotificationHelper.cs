using HtmlTags;
using System;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Html
{
    public static class NotificationHelper
    {
        private const string NotificationMessageKey = "NotificationMessage";
        private const string NotificationTypeKey = "NotificationType";
        private const string NotificatoinTitleKey = "NotificationTitle";

        public static void CreateSuccessNotification(ControllerBase controller, string message, string title = null)
        {
            CreateNotification(controller, NotificationType.Success, message, title);
        }

        public static HtmlTag RenderSuccessNotification(this HtmlHelper helper)
        {
            return Render(helper.ViewContext.TempData, NotificationType.Success, "alert-success");
        }

        private static void CreateNotification(ControllerBase controller, string notificationType, string message, string title)
        {
            controller.TempData[NotificationTypeKey] = notificationType;
            controller.TempData[NotificatoinTitleKey] = title;
            controller.TempData[NotificationMessageKey] = message;
        }

        private static HtmlTag Render(TempDataDictionary tempData, string notificationTypeMatch, string cssClass)
        {
            var notificationType = Convert.ToString(tempData[NotificationTypeKey]);
            if (!String.Equals(notificationType, notificationTypeMatch)) return HtmlTag.Empty();

            var title = Convert.ToString(tempData[NotificatoinTitleKey]);
            var message = Convert.ToString(tempData[NotificationMessageKey]);

            var titleWithFormatting = title;
            if (!String.IsNullOrWhiteSpace(title))
            {
                titleWithFormatting = $"<strong>{title}</strong> ";
            }

            var closeButton = new HtmlTag("button")
                .Attr("type", "button")
                .AddClass("close")
                .Data("dismiss", "alert")
                .Attr("aria-hidden", "true");

            var alertDiv = new HtmlTag("div")
                .AddClasses("alert", cssClass, "alert-disimissable")
                .Append(closeButton)
                .Text($"{titleWithFormatting}{message}")
                .Encoded(false);

            return alertDiv;
        }

        public class NotificationType
        {
            public const string Success = "Success";
        }
    }
}
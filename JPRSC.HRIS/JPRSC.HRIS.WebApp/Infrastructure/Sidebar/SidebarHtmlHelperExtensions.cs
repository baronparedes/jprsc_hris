using HtmlTags;
using Microsoft.AspNet.Identity;
using JPRSC.HRIS.Infrastructure.Data;
using JPRSC.HRIS.WebApp.Infrastructure.Dependency;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JPRSC.HRIS.WebApp.Infrastructure.Sidebar
{
    public static class SidebarHtmlHelperExtensions
    {
        public static MvcHtmlString RenderSidebarMenu<T>(this HtmlHelper<T> helper, params SidebarHeading[] headings)
        {
            if (headings == null) throw new ArgumentNullException(nameof(headings));

            var db = DependencyConfig.Instance.Container.GetInstance<ApplicationDbContext>();
            var currentUserId = HttpContext.Current.User.Identity.GetUserId();

            var currentUser = db.Users
                .Include(u => u.CustomRoles)
                .SingleOrDefault(u => u.Id == currentUserId);

            var itemsStringBuilder = new StringBuilder(128);

            for (var i = 0; i < headings.Count(); i++)
            {
                var heading = headings[i];

                if (!String.IsNullOrWhiteSpace(heading.Text))
                {
                    var headingListTag = new HtmlTag("li");
                    headingListTag.AddClass("heading");

                    var headingTag = new HtmlTag("h3");
                    headingTag.AddClass("uppercase");
                    headingTag.Text(heading.Text);

                    headingListTag.Append(headingTag);

                    itemsStringBuilder.AppendLine(headingListTag.ToHtmlString());
                }

                var items = heading.SubMenus;

                for (var j = 0; j < items.Count(); j++)
                {
                    var item = items[j];

                    var userHasPermission = currentUser != null && currentUser.CustomRoles.Any(cr => cr.HasPermission(item.Permission));
                    if (!userHasPermission)
                    {
                        continue;
                    }

                    var itemTag = new HtmlTag("li");
                    itemTag.AddClasses("nav-item");

                    if (j == 0)
                    {
                        itemTag.AddClass("start");
                    }

                    if (!item.SubMenus.Any())
                    {
                        itemTag.AddClass($"{(HasSelectedMenu(helper, item.Controller, item.Action) ? "active" : String.Empty)}");
                        var finalLevelLinkTag = GetFinalLevelLinkTag(helper, item);

                        itemTag.Append(finalLevelLinkTag);
                    }
                    else
                    {
                        itemTag.AddClass($"{(HasSelectedMenu(helper, item.Controller) ? "active" : String.Empty)}");

                        var linkTag = new HtmlTag("a");
                        linkTag.Attr("href", "javascript:;");
                        linkTag.AddClasses("nav-link", "nav-toggle");

                        var iconTag = new HtmlTag("i");
                        iconTag.AddClass(item.IconClass);
                        linkTag.Append(iconTag);

                        var titleTag = new HtmlTag("span");
                        titleTag.AddClass("title");
                        titleTag.Text(item.Title);
                        linkTag.Append(titleTag);

                        var arrowTag = new HtmlTag("span");
                        arrowTag.AddClass("arrow");
                        linkTag.Append(arrowTag);

                        itemTag.Append(linkTag);

                        var subMenusTag = new HtmlTag("ul");

                        for (var k = 0; k < item.SubMenus.Count; k++)
                        {
                            var finalLevelLinkTag = GetFinalLevelLinkTag(helper, item.SubMenus[k]);

                            if (k == 0)
                            {
                                finalLevelLinkTag.AddClass("start");
                            }

                            subMenusTag.Append(finalLevelLinkTag);
                        }
                    }

                    itemsStringBuilder.AppendLine(itemTag.ToHtmlString());
                }
            }

            return new MvcHtmlString(itemsStringBuilder.ToString());
        }

        private static HtmlTag GetFinalLevelLinkTag<T>(HtmlHelper<T> helper, SidebarMenuItem item)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var finalLevelLinkTag = new HtmlTag("a");
            finalLevelLinkTag.Attr("href", urlHelper.Action(item.Action, item.Controller));
            finalLevelLinkTag.AddClass("nav-link");

            var iconTag = new HtmlTag("i");
            iconTag.AddClass(item.IconClass);
            finalLevelLinkTag.Append(iconTag);

            var titleTag = new HtmlTag("span");
            titleTag.AddClass("title");
            titleTag.Text(item.Title);
            finalLevelLinkTag.Append(titleTag);

            return finalLevelLinkTag;
        }

        private static SelectedSidebarItem GetSelectedSidebarItem<T>(HtmlHelper<T> helper)
        {
            return helper.ViewContext.Controller.ViewBag.SelectedSidebarItem as SelectedSidebarItem;
        }

        private static bool HasSelectedMenu<T>(HtmlHelper<T> helper, string controllerName, string actionName)
        {
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            if (actionName == null) throw new ArgumentNullException(nameof(actionName));
            if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));

            var selectedSidebarItem = GetSelectedSidebarItem(helper);

            if (selectedSidebarItem != null)
            {
                var sameAction = String.Equals(actionName, selectedSidebarItem.ActionName, StringComparison.CurrentCultureIgnoreCase);
                var sameController = String.Equals(controllerName, selectedSidebarItem.ControllerName, StringComparison.CurrentCultureIgnoreCase);

                return (sameAction && sameController) || SpecialCases(actionName, controllerName, selectedSidebarItem);
            }

            return false;

            bool SpecialCases(string actionNameLocal, string controllerNameLocal, SelectedSidebarItem selectedSidebarItemLocal)
            {
                return false;
            }
        }

        private static bool HasSelectedMenu<T>(HtmlHelper<T> helper, string controllerName)
        {
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));

            var selectedSidebarItem = GetSelectedSidebarItem(helper);

            if (selectedSidebarItem != null)
            {
                var sameController = String.Equals(controllerName, selectedSidebarItem.ControllerName, StringComparison.CurrentCultureIgnoreCase);

                return sameController;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CoolMeetingWeb.Extensions
{
    public static class SortableActionLinkExtension
    {
        public static MvcHtmlString SortableActionLink(this HtmlHelper htmlHelper, 
            string actionName, string fieldHeader, string sortFieldName,
            IDictionary<string, object> htmlAttributes = null)
        {
            return new MvcHtmlString(
                GenerateHtml(actionName, fieldHeader, sortFieldName, htmlAttributes));
        }

        public static MvcHtmlString SortableActionLinkFor<TModel, TProperty>(
            this HtmlHelper<IEnumerable<TModel>> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string actionName, IDictionary<string, object> htmlAttributes = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(
                expression, new ViewDataDictionary<TModel>());
            //string areaName = htmlHelper.ViewContext.RouteData.DataTokens["area"].ToString();
            string controllerName = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            //string url = string.Format("/{0}/{1}/{2}?", areaName, controllerName, actionName);
            string url = string.Format("/{0}/{1}?", controllerName, actionName);
            string fieldHeader = metadata.DisplayName;
            string sortFieldName = metadata.PropertyName;
            return new MvcHtmlString(
                GenerateHtml(url, fieldHeader, sortFieldName, htmlAttributes)
                );
        }

        private static string GenerateHtml(string url, 
            string fieldHeader, string sortFieldName, 
            IDictionary<string, object> htmlAttributes)
        {
            string keySortFieldName = "sortFieldName";
            string keyDescending = "descending";

            StringBuilder href = new StringBuilder();
            href.Append(url);
            var parameters = HttpContext.Current.Request.QueryString;
            foreach(string key in parameters.Keys)
            {
                if(key == keySortFieldName || key == keyDescending)
                {
                    continue;
                }
                href.AppendFormat("{0}={1}&", key, parameters[key]);
            }
            href.AppendFormat("{0}={1}&", keySortFieldName, sortFieldName);
            if (parameters[keySortFieldName] == sortFieldName)
            {
                if(parameters[keyDescending] == null || parameters[keyDescending] == "false")
                {
                    href.AppendFormat("{0}=true", keyDescending);
                }
            }
            

            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("target", "_self");
            a.MergeAttribute("href", href.ToString().TrimEnd('&', '?'));
            a.MergeAttributes(htmlAttributes);
            a.SetInnerText(fieldHeader);

            return a.ToString();
        }
    }
}
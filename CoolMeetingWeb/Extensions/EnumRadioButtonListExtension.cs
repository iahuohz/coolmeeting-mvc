using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Mvc.Html;

using CoolMeetingWeb.Models;

namespace CoolMeetingWeb.Extensions
{
    public static class EnumRadioButtonListExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="typeName">在TermDescriptions类中定义的静态属性名称</param>
        /// <param name="isInline"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, string typeName,  bool isInline, IDictionary<string, object> htmlAttributes = null, string selectedValue = null)
        {
            var dataSource = TermDescription.GetDataFor(typeName);
            return GenerateHtml(name, dataSource, isInline, htmlAttributes, selectedValue);
        }
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isInline, IDictionary<string, object> htmlAttributes = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var dataSource = TermDescription.GetDataFor(metadata.PropertyName);

            string name = ExpressionHelper.GetExpressionText(expression);
            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            if (htmlAttributes == null)
            {
                htmlAttributes = attributes;
            }
            else
            {
                foreach (var item in attributes)
                {
                    htmlAttributes.Add(item);
                }
            }

            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            string stateValue = (string)GetModelStateValue(htmlHelper, fullHtmlFieldName, typeof(string));
            if (string.IsNullOrEmpty(stateValue))
            {
                if (metadata.Model != null)
                {
                    // 枚举值直接转换成整数，然后转成字符串
                    stateValue = ((int)metadata.Model).ToString();
                }
            }

            return GenerateHtml(fullHtmlFieldName, dataSource, isInline, htmlAttributes, stateValue);
        }

        internal static object GetModelStateValue<TModel>(HtmlHelper<TModel> htmlHelper, string key, Type destinationType)
        {
            ModelState state;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out state) && (state.Value != null))
            {
                return state.Value.ConvertTo(destinationType, null);
            }
            return null;
        }

        private static MvcHtmlString GenerateHtml(string name, Dictionary<int, string> dataSource,
            bool isInline, IDictionary<string, object> htmlAttributes, string stateValue = null)
        {
            StringBuilder output = new StringBuilder();
            int i = 0;
            foreach (var data in dataSource)
            {
                i++;
                string id = string.Format("{0}_{1}", name, i);
                output.Append(GenerateRadioHtml(name, id, data.Value, data.Key.ToString(),
                    (stateValue != null && stateValue == data.Key.ToString()), isInline, htmlAttributes));
            }
           
            return new MvcHtmlString(output.ToString());
        }

        private static string GenerateRadioHtml(string name, string id, string labelText, string value, bool isChecked, bool isInline, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder sb = new StringBuilder();

            TagBuilder input = new TagBuilder("input");
            input.GenerateId(id);
            input.MergeAttribute("name", name);
            input.MergeAttribute("type", "radio");
            input.MergeAttribute("value", value);
            input.MergeAttributes(htmlAttributes);
            if (isChecked)
            {
                input.MergeAttribute("checked", "checked");
            }

            TagBuilder label = new TagBuilder("label");
            label.MergeAttribute("for", id);
            if (isInline)
            {
                label.MergeAttribute("class", "radio-inline");
            }
            label.InnerHtml = input.ToString() + labelText;

            sb.AppendLine(label.ToString());
            return sb.ToString();
        }
    }
}
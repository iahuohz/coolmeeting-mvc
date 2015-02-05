using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace CoolMeetingWeb.ViewModels
{
    public class PagedNavigatorViewModel
    {
        public const int PageSize = 3;
        public int TotalRows { get; set; }
        public int CurrentPageIndex { get; set; }
        public string SortFieldName { get; set; }
        public bool Descending { get; set; }
        public int TotalPages
        {
            get
            {
                return TotalRows / PageSize + (TotalRows % PageSize == 0 ? 0 : 1);
            }
        }

        private RouteValueDictionary routeValues = new RouteValueDictionary();

        public PagedNavigatorViewModel()
        {
            var parameters = HttpContext.Current.Request.QueryString;
            foreach (string key in parameters)
            {
                routeValues.Add(key, parameters[key]);
            }
        }

        public RouteValueDictionary CurrentPageUrl
        {
            get
            {
                routeValues["pageIndex"] = CurrentPageIndex;
                return routeValues;
            }
        }

        public RouteValueDictionary PreviousPageUrl
        {
            get
            {
                int pageIndex;
                if (CurrentPageIndex == 1)
                {
                    pageIndex = CurrentPageIndex;
                }
                else
                {
                    pageIndex = CurrentPageIndex - 1;
                }
                routeValues["pageIndex"] = pageIndex;
                return routeValues;
            }
        }

        public RouteValueDictionary NextPageUrl
        {
            get
            {
                int pageIndex;
                if (CurrentPageIndex == TotalPages)
                {
                    pageIndex = CurrentPageIndex;
                }
                else
                {
                    pageIndex = CurrentPageIndex + 1;
                }
                routeValues["pageIndex"] = pageIndex;
                return routeValues;
            }
        }

        public RouteValueDictionary FirstPageUrl
        {
            get
            {
                routeValues["pageIndex"] = 1;
                return routeValues;
            }
        }

        public RouteValueDictionary LastPageUrl
        {
            get
            {
                routeValues["pageIndex"] = TotalPages;
                return routeValues;
            }
        }
    }
}
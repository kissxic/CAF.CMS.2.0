﻿using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CAF.WebSite.Application.WebUI.UI
{
    public static class DataListExtensions
    {

        // codehint: sm-edit
        public static IHtmlString DataList<T>(this HtmlHelper helper, IEnumerable<T> items, int columns,
            Func<T, HelperResult> template, int gridColumns = 24)
            where T : class
        {
            if (items == null)
                return new HtmlString("");

            Guard.Against<ArgumentOutOfRangeException>(gridColumns % columns != 0, "Wrong column count. Ensure that gridColumns is divisible by columns.");

            var sb = new StringBuilder();
            sb.Append("<div class='data-list data-list-grid'>");

            int cellIndex = 0;
            string spanClass = String.Format("span{0}", gridColumns / columns);

            foreach (T item in items)
            {
                if (cellIndex == 0)
                    sb.Append("<div class='row product-list'>");

                sb.Append("<div class='{0} data-list-item equalized-column' data-equalized-deep='true'>".FormatInvariant(spanClass));
                sb.Append(template(item).ToHtmlString());
                sb.Append("</div>");

                cellIndex++;

                if (cellIndex == columns)
                {
                    cellIndex = 0;
                    sb.Append("</div>");
                }
            }

            if (cellIndex != 0)
            {
                sb.Append("</div>"); // close .row-fluid
            }

            sb.Append("</div>"); // close .data-list

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString DataList<T>(this HtmlHelper helper, IEnumerable<T> items, int columns,
            Func<T, HelperResult> templateColumsLeft, Func<T, HelperResult> templateColumsRight)
            where T : class
        {
            if (items == null)
                return new HtmlString("");


            var sb = new StringBuilder();


            int cellIndex = 0;



            foreach (T item in items)
            {
                cellIndex++;
                sb.Append("<div class='row margin-bottom-40'>");

                if (cellIndex % columns == 1)
                {
                    sb.Append(templateColumsLeft(item).ToHtmlString());
                    sb.Append(templateColumsRight(item).ToHtmlString());
                }
                else
                {
                    sb.Append(templateColumsRight(item).ToHtmlString());
                    sb.Append(templateColumsLeft(item).ToHtmlString());
                }
                sb.Append("</div>"); // close .data-list

            }

            return new HtmlString(sb.ToString());
        }
    }
}

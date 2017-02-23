using System.Text;
using System.Web.Mvc;


namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// Grid ������չ������
    /// </summary>
    public static class GridFactoryExtensions
    {
        /// <summary>
        /// Bootstrap��
        /// </summary>
        /// <param name="htmlHelper">Html ���֡�</param>
        /// <returns>Grid ������</returns>
        public static GridFacotory GridToolKit(this HtmlHelper htmlHelper)
        {
            return new GridFacotory(htmlHelper);
        }

        /// <summary>
        /// Bootstrap��
        /// </summary>
        /// <typeparam name="TModel">ģ�����͡�</typeparam>
        /// <param name="htmlHelper">Html ���֡�</param>
        /// <returns>Grid ������</returns>
        public static GridFacotory<TModel> GridToolKit<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new GridFacotory<TModel>(htmlHelper);
        }
        public static GridFacotory<TModel> GridToolKit<TModel>(this HtmlHelper htmlHelper)
        {
            return new GridFacotory<TModel>(htmlHelper);
        }

        public static MvcHtmlString GridTable<TModel>(this HtmlHelper helper, GridTable<TModel> gridTable)
        {
            StringBuilder sbHtml = new StringBuilder();

            sbHtml.AppendFormat("<table id='{0}' class='{1}' style='table-layout:fixed'>", gridTable.GetTableId, gridTable.GetDefaultTableClass);
            sbHtml.Append("<thead>");
            sbHtml.Append("<tr>");
            foreach (var column in gridTable.GetColumns)
            {
                if (column.Name == "����")
                {
                    sbHtml.AppendFormat("<th class='{0}'>{1}</th>", column.CssClassHeader, column.DisplayName);
                }
                else if (column.DisplayName == null)
                {
                    sbHtml.AppendFormat("<th class='{0}'><input type='checkbox' class='group-checkable' data-set='checkboxes'></th>", column.CssClassHeader, column.DisplayName);
                }
                else
                {
                    sbHtml.AppendFormat("<th class='{0}'>{1}</th>", column.CssClassHeader, column.DisplayName);
                }
            }

            sbHtml.Append("</tr>");
            sbHtml.Append("</thead>");
            sbHtml.Append("<tbody>");
            sbHtml.Append("</tbody>");
            sbHtml.AppendLine("</table>");

            // Render tag
            return MvcHtmlString.Create(sbHtml.ToString());


        }
    }
}
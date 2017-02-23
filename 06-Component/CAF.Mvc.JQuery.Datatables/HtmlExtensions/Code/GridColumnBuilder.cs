

namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// ����в���ͨ�÷���
    /// </summary>
    public static class GridColumnBuilder
    {
        /// <summary>
        /// �����еĳ�ʼ��ȣ�����pixels�Ͱٷֱ�
        /// </summary>
        /// <param name="width">��ʼ���</param>
        /// <returns></returns>
        public static GridColumn Width(this GridColumn col, int width)
        {
            col.Width = width;
            return col;
        }

        /// <summary>
        /// �����ʼ��ʱ�����Ƿ�����
        /// </summary>
        public static GridColumn Hidden(this GridColumn col, bool hidden = true)
        {
            col.Visible = hidden;
            return col;
        }
        /// <summary>
        /// �����ֶ�����Դ
        /// </summary>
        /// <param name="col"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GridColumn Data(this GridColumn col, string data = null)
        {
            col.Data = data;
            return col;
        }
 
        
        /// <summary>
        /// ���嶨���ֶ��Ƿ�ɱ༭
        /// </summary>
        //public static GridColumn Editable(this GridColumn col, ColumnEdits edittype)
        //{
        //    col.Editable = true;
        //    col.EditType = edittype.ToString().ToLower();
        //    return col;
        //}

        /// <summary>
        /// ���嶨���ֶ��Ƿ�ɱ༭
        /// </summary>
        //public static GridColumn Formatter(this GridColumn col, string cellformater)
        //{
        //    col.Formatter = cellformater;
        //    return col;
        //}

        /// <summary>
        /// �������� 
        /// </summary>
        //public static GridColumn Searchable(this GridColumn col, CellTypes filedType = CellTypes.String, ColumnSearchs columnSearch = ColumnSearchs.Text)
        //{
        //    col.Search = true;
        //    col.SearchFiledType = filedType;
        //    col.SearchType = columnSearch.ToString().ToLower();
        //    return col;
        //}
        public static GridColumn DefaultContent(this GridColumn col, string defaultContent)
        {
            col.Name = string.Empty;
            col.DataProp = string.Empty;
            col.DefaultContent = defaultContent;
            return col;
        }


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="columnSorts">��������</param>
        /// <returns></returns>
        public static GridColumn Sortable(this GridColumn col, ColumnSorts columnSorts = ColumnSorts.Text)
        {
            col.Sortable = true;
            col.SortType = columnSorts.ToString().ToLower();
            return col;
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="columnSorts">��������</param>
        /// <returns></returns>
        public static GridColumn Sortable(this GridColumn col, SortDirection columnSorts = SortDirection.None)
        {
            col.Sortable = true;
            col.SortDirection = columnSorts;
            return col;
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="col"></param>
        /// <param name="cellType"></param>
        /// <returns></returns>
        public static GridColumn CellType(this GridColumn col, CellTypes cellType = CellTypes.String)
        {
            col.sType = cellType.ToString().ToLower();
            return col;
        }
    }
}

using System.ComponentModel;

namespace CAF.Infrastructure.Core.Domain.Cms.Channels
{
	/// <summary>
	/// Represents a product type
	/// </summary>
	public enum CategoryShowType
    {
        /// <summary>
        /// 在页面以左侧导航显示
        /// </summary>
        [Description("左侧导航显示")]
        LeftNavigationt = 5,

        /// <summary>
        /// 在页面以 复选框 显示 
        /// </summary>
        [Description("复选框显示")]
        CheckBox = 10,

        /// <summary>
        /// 在页面以 下拉框显示 
        /// </summary>
        [Description("下拉框显示")]
        DropDownList = 15,
    }
}

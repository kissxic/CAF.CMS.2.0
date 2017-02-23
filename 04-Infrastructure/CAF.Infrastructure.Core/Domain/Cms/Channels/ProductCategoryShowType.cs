
using System.ComponentModel;

namespace CAF.Infrastructure.Core.Domain.Cms.Channels
{
	/// <summary>
	/// Represents a product type
	/// </summary>
	public enum ProductCategoryShowType
    {
        /// <summary>
        /// 在页面不显示
        /// </summary>
        [Description("不显示")]
        No = 5,

        /// <summary>
        /// 在页面以 步骤 显示 
        /// </summary>
        [Description("步骤")]
        Step = 10,

        /// <summary>
        /// 在页面以 下拉框显示 
        /// </summary>
        [Description("下拉框显示")]
        DropDownList = 15,
    }
}

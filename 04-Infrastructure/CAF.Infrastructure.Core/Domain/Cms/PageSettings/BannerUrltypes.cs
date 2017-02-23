
using System.ComponentModel;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
	/// <summary>
	/// Represents a product type
	/// </summary>
	public enum BannerUrltypes
    {
        /// <summary>
        /// 链接
        /// </summary>
        [Description("链接")]
        Link = 5,

        /// <summary>
        /// 全部商品
        /// </summary>
        [Description("全部商品")]
        AllProducts = 10,

        /// <summary>
        /// 商品分类
        /// </summary>
        [Description("商品分类")]
        Category = 15,
        /// <summary>
        /// 店铺简介
        /// </summary>
        [Description("店铺简介")]
        VShopIntroduce = 15,
    }
}

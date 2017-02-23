
using System.ComponentModel;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
	/// <summary>
	/// Represents a product type
	/// </summary>
	public enum SlideAdType
    {
        /// <summary>
        /// 链接
        /// </summary>
        [EnumDescription("首页轮播图")]
        PlatformHome = 5,

        /// <summary>
        /// 全部商品
        /// </summary>
        [EnumDescription("限时购轮播图")]
        PlatformLimitTime = 10,

        /// <summary>
        /// 小图
        /// </summary>
        [EnumDescription("首页小图轮播图")]
        PlatformMiniHome = 15,

        /// <summary>
        /// 区域
        /// </summary>
        [EnumDescription("首页区域轮播图")]
        PlatformAreaHome = 20,

    }
}

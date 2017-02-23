
using CAF.WebSite.Application.WebUI.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    /// <summary>
    /// 步骤属性
    /// </summary>
    public partial class PublicStepOneModel : ModelBase
    {
        public PublicStepOneModel()
        {

        }
        /// <summary>
        /// 商品类型ID
        /// </summary>
        public int? PcategoryId { get; set; }
        /// <summary>
        /// 频道ID
        /// </summary>
        public int? ChannelId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// 栏目分类ID
        /// </summary>
        public int? CategoryId { get; set; }
        /// <summary>
        /// 扩展属性值
        /// </summary>
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
    }
}
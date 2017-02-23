
using System.ComponentModel;
namespace CAF.Infrastructure.Core.Domain.Sellers 
{
    public enum VendorStage
    {
        [Description("许可协议")]
        Agreement=1,
        [Description("公司信息")]
        CompanyInfo=2,
        [Description("财务信息")]
        FinancialInfo=3,
        [Description("店铺信息")]
        ShopInfo=4,
        [Description("上传支付凭证")]
        UploadPayOrder=5,
        [Description("完成")]
        Finish=6
    }
}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Agents
{
    /// <summary>
    /// ������������
    /// </summary>
    public partial class AgencyContent : UserContent
    {
      
        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public int? ArticleId { get; set; }
        public virtual Article Article { get; set; }
        /// <summary>
        /// ��ϵ��
        /// </summary>
        [DataMember]
        public string TrueName { get; set; }
       /// <summary>
       /// �绰
       /// </summary>
        [DataMember]
        public string TelePhone { get; set; }
        /// <summary>
        /// �ֻ�����
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        [DataMember]
        public string QQ { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string ProvinceName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string CityName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string DistributionChannelId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string AgentPropertyId { get; set; }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// �鿴��Ȩ
        /// </summary>
        [DataMember]
        public int ShowAuthId { get; set; }
        [DataMember]
        public ShowAuthType ShowAuthType
        {
            get
            {
                return (ShowAuthType)this.ShowAuthId;
            }
            set
            {
                this.ShowAuthId = (int)value;
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string OtherChannel { get; set; }
        

    }
}

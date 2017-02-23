using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    /// <summary>
    /// Â¥²ãÎÄ±¾ÄÚÈÝ
    /// </summary>
    public class FloorTopicInfo : AuditedBaseEntity
    {
        public FloorTopicInfo()
        {
            CreatedOnUtc = DateTime.UtcNow;
            ModifiedOnUtc = DateTime.UtcNow;
        }
        public int FloorId { get; set; }

        public HomeFloorInfo HomeFloorInfo { get; set; }


        public int? PictureId { get; set; }

        public virtual Picture Picture { get; set; }

        public string Url { get; set; }

        public string TopicName { get; set; }

        public int TopicTypeId { get; set; }

        public Position TopicType
        {
            get
            {
                return (Position)this.TopicTypeId;
            }
            set
            {
                this.TopicTypeId = (int)value;
            }
        }

        

        public int DisplayOrder { get; set; }


    }
}

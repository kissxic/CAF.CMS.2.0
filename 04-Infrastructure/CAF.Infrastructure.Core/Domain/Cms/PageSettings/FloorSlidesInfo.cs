using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    /// <summary>
    /// 楼层幻灯片
    /// </summary>
    public class FloorSlidesInfo : AuditedBaseEntity
    {
        public FloorSlidesInfo()
        {
             CreatedOnUtc = DateTime.UtcNow;
             ModifiedOnUtc = DateTime.UtcNow;
        }

        public string Name { get; set; }

        public int FloorId { get; set; }

        public HomeFloorInfo HomeFloorInfo { get; set; }

        public int DisplayOrder { get; set; }

        private ICollection<FloorSlideDetailsInfo> _floorSlideDetailsInfo;
        /// <summary>
        /// 楼层幻灯片明细
        /// </summary>
        public virtual ICollection<FloorSlideDetailsInfo> FloorSlideDetailsInfos
        {
            get { return _floorSlideDetailsInfo ?? (_floorSlideDetailsInfo = new HashSet<FloorSlideDetailsInfo>()); }
            protected set { _floorSlideDetailsInfo = value; }
        }
    }
    /// <summary>
    /// 楼层幻灯片明细
    /// </summary>
    public class FloorSlideDetailsInfo : AuditedBaseEntity
    {
        public FloorSlideDetailsInfo()
        {
            CreatedOnUtc = DateTime.UtcNow;
            ModifiedOnUtc = DateTime.UtcNow;
        }
        public int? ArticleId { get; set; }

        public Article Article { get; set; }

        public int? PictureId { get; set; }

        public virtual Picture Picture { get; set; }

        public string Url { get; set; }

        public int FloorSlideId { get; set; }

        public FloorSlidesInfo FloorSlidesInfo { get; set; }

        public int DisplayOrder { get; set; }
    }
}

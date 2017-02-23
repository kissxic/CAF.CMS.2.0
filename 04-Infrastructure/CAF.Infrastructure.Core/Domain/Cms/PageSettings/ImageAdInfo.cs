using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Runtime.CompilerServices;

namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class ImageAdInfo : AuditedBaseEntity
    {
        public int PictureId { get; set; }

        public Picture Picture { get; set; }

        public string Url { get; set; }

        public int PostionId { get; set; }

    }
}
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core.Domain.Cms.Favorites
{
    /// <summary>
    ///  ’≤ÿº–
    /// </summary>
    [DataContract]
    public class FavoriteInfo : AuditedBaseEntity
    {
        public FavoriteInfo()
        {

        }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public virtual User User { get; set; }

        [DataMember]
        public int ArticleId { get; set; }

        [DataMember]
        public virtual Article Article { get; set; }


    }
}
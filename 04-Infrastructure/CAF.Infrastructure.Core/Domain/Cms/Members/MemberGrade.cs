using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Domain.Cms.Members
{
    public class MemberGrade : BaseEntity, ISoftDeletable
    {
        /// <summary>
        /// Gets or sets the SystemName
        /// </summary>
        [DataMember]
        public string SystemName { get; set; }
        /// <summary>
		/// Gets or sets the GradeName
		/// </summary>
        [DataMember]
        public string GradeName { get; set; }
        /// <summary>
		/// Gets or sets the Integral
		/// </summary>
        [DataMember]
        public int Integral { get; set; }
        /// <summary>
		/// Gets or sets the Comment
		/// </summary>
        [DataMember]
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets the default quantity unit
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }

        public bool Deleted { get; set; }
    }
}

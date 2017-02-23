using CAF.Infrastructure.Core.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    // [DataContract]
    public abstract partial class BaseEntity : BaseEntity<int>
    {
        /// <summary>
        ///     ��ȡ������ �汾���Ʊ�ʶ�����ڴ�����
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        [DataMember]
        public byte[] Timestamp { get; set; }

        
    }
}

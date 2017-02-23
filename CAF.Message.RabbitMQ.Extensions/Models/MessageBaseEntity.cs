using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
    基础实体
*/
namespace CAF.Message.Distributed.Extensions.Models
{
    /// <summary>
    /// 基类
    /// </summary>
    [CollectionName("BaseEntityCollection")]
    [Serializable]
    public class BaseEntity : IEntity
    {
        [BsonRepresentation(BsonType.String)]
        [DataMember]
        //[JsonProperty("id")]
        public string Id { get; set; }
        [DataMember]
        public string userid { get; set; }
    }
    /// <summary>
    /// 在线用户
    /// </summary>
    [CollectionName("OnlineUserCollection")]
    [Serializable]
    public class OnlineUser : IEntity
    {
        [BsonRepresentation(BsonType.String)]
        [DataMember]
        //[JsonProperty("id")]
        public string Id { get; set; }
        [DataMember]
        public string userid { get; set; }
        [DataMember]
        public string connectionid { get; set; }
        [DataMember]
        public DateTime? onlinetime { get; set; }
    }
    /// <summary>
    /// 申请
    /// </summary>
    [Serializable]
    public class ApplyEntity : BaseEntity
    {
        [DataMember]
        public int applytype { get; set; }
        [DataMember]
        public string targetid { get; set; }
        [DataMember]
        public DateTime applytime { get; set; }
        [DataMember]
        public string other { get; set; }
    }
    /// <summary>
    /// 头像
    /// </summary>
    [Serializable]
    public class AvatarEntity : BaseEntity
    {
        /// <summary>
        /// 我的头像
        /// </summary>
        [DataMember]
        public string avatar { get; set; }
    }

    /// <summary>
    /// 用户
    /// </summary>
    [Serializable]
    public class UserEntity : AvatarEntity
    {
        /// <summary>
        /// 在线状态 online：在线、hide：隐身
        /// </summary>
        [DataMember]
        public string status { get; set; }
        /// <summary>
        /// 我的昵称
        /// </summary>
        [DataMember]
        public string username { get; set; }
        /// <summary>
        /// 我的签名
        /// </summary>
        [DataMember]
        public string sign { get; set; }
    }

    /// <summary>
    /// 好友
    /// </summary>
    [Serializable]
    public class GroupUserEntity : UserEntity
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        [DataMember]
        public int groupid { get; set; }
        [DataMember]
        public string remarkname { get; set; }

    }

    /// <summary>
    ///  群
    /// </summary>
    [Serializable]
    public class GroupEntity : AvatarEntity
    {
        /// <summary>
        /// 好友分组名
        /// </summary>
        [DataMember]
        public string groupname { get; set; }
        /// <summary>
        /// 分组说明
        /// </summary>
        [DataMember]
        public string groupdesc { get; set; }
        /// <summary>
        /// 默认分组
        /// </summary>
        [DataMember]
        public bool isdefault { get; set; }
    }

    /// <summary>
    /// 好友分组
    /// </summary>
    [Serializable]
    public class FriendGroupEntity : GroupEntity
    {
        public FriendGroupEntity()
        {
            //好友分组，该选项不需要
            avatar = "";
            list = new List<GroupUserEntity>();

        }
        /// <summary>
        /// 分组下的好友列表
        /// </summary>
        [DataMember]
        public IList<GroupUserEntity> list { get; set; }
        /// <summary>
        /// 在线数量，可以不传
        /// </summary>
        [DataMember]
        public int online { get; set; }
    }

    /// <summary>
    /// 基础信息json
    /// </summary>
    [Serializable]
    public class BaseListResult : BaseEntity
    {
        public BaseListResult()
        {
            friend = new List<FriendGroupEntity>();
            group = new List<GroupEntity>();
        }
        [DataMember]
        public IList<FriendGroupEntity> friend { get; set; }
        [DataMember]
        public IList<GroupEntity> group { get; set; }
        [DataMember]
        public UserEntity mine { get; set; }
        /// <summary>
        /// 用户设置的皮肤
        /// </summary>
        [DataMember]
        public List<string> skin { get; set; }
    }

    /// <summary>
    /// 群员信息json
    /// </summary>
    [Serializable]
    public class MembersListResult : BaseEntity
    {
        /// <summary>
        /// 群主
        /// </summary>
        [DataMember]
        public UserEntity owner { get; set; }
        /// <summary>
        /// 群成员列表
        /// </summary>
        [DataMember]
        public IList<GroupUserEntity> list { get; set; }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable]
    public class JsonResultModel
    {
        public JsonResultType code { get; set; }
        public object data { get; set; }
        public string msg { get; set; }
    }

    /// <summary>
    /// 成功失败
    /// </summary>
    public enum JsonResultType
    {
        Success = 0,
        Failed = 1
    }
}
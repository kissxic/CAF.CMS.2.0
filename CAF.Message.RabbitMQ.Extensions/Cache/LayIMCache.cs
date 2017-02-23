using CAF.Infrastructure.Core.Utilities.Randomizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoRepository;
using CAF.Message.Distributed.Extensions.Models;
using CAF.WebSite.Application.Services;
using CAF.Message.Distributed.Extensions;
using Newtonsoft.Json;
using CAF.Infrastructure.Core;
using IdGen;
using CAF.Infrastructure.Core.Utilities;

namespace CAF.Message.Distributed.Extensions.Core
{
    public partial class LayIMCache : ILayIMCache
    {
        #region 变量
        private readonly IWorkContext _workContext;
        private static MongoRepository<OnlineUser> onlineUserrepo;
        private static MongoRepository<BaseListResult> userEntityrepo;
        private static MongoRepository<UserFriend> userFriendrepo;
        private static MongoRepository<FriendGroupEntity> friendGrouprepo;
        private static MongoRepository<GroupUserEntity> groupUserEntityrepo;

        private readonly IdGenerator _idGenerator; 
        #endregion
        static LayIMCache()
        {

            onlineUserrepo = new MongoRepository<OnlineUser>(RabbitMqConstants.MongoServerSettings);
            userEntityrepo = new MongoRepository<BaseListResult>(RabbitMqConstants.MongoServerSettings);
            userFriendrepo = new MongoRepository<UserFriend>(RabbitMqConstants.MongoServerSettings);
            friendGrouprepo = new MongoRepository<FriendGroupEntity>(RabbitMqConstants.MongoServerSettings);
            groupUserEntityrepo = new MongoRepository<GroupUserEntity>(RabbitMqConstants.MongoServerSettings);
        }
        public LayIMCache(IWorkContext workContext)
        {
            this._workContext = workContext;
            _idGenerator = IdGenEngineContext.Current.IdGenerator;
        }

        #region 缓存用户的token
        public void CacheUserAfterLogin(string userId)
        {
            // return _workContext.CurrentUser.Id.ToString();

            var token = SomeRandom.String(20);

            bool result = onlineUserrepo.Any(x => x.userid == userId);
            if (result)
            {
                var onlineUser = new OnlineUser()
                {
                    userid = userId
                };
                onlineUserrepo.Add(onlineUser);
            }
            else
            {
                //记录日志
            }
        }
        #endregion

        #region 获取当前登录用户的用户id

        public string GetCurrentUserId()
        {
            return _workContext.CurrentUser.Id.ToString();

        }
        #endregion

        #region 获取用户基本信息

        public BaseListResult GetUserBaseInfo()
        {
            var baseUser = new BaseListResult();
            var userId = GetCurrentUserId();
            if (userEntityrepo.Any(x => x.userid == userId))
            {
                baseUser = userEntityrepo.First(x => x.userid == userId);
            }
            return baseUser;
        }

        #endregion

        #region 在线用户处理
        public void OperateOnlineUser(OnlineUser user, bool isDelete = false)
        {
            bool result = onlineUserrepo.Any(x => x.userid == user.userid);
            if (isDelete)
            {
                if (result)
                {
                    onlineUserrepo.Delete(x => x.userid == user.userid);
                }
            }
            else
            {
                if (!result)
                {
                    var onlineUser = new OnlineUser()
                    {
                        Id = _idGenerator.CreateId().ToString(),
                        userid = user.userid,
                        connectionid = user.connectionid,
                        onlinetime = DateTime.UtcNow
                    };
                    onlineUserrepo.Add(onlineUser);

                }
                else
                {
                    var onlineUser = onlineUserrepo.First(x => x.userid == user.userid);
                    onlineUser.onlinetime = DateTime.UtcNow;
                    onlineUserrepo.Update(onlineUser);
                }
            }
            CreateUser(user.userid);
        }
        /// <summary>
        /// 创建IM新用户
        /// </summary>
        /// <param name="userid"></param>
        private void CreateUser(string userid)
        {

            if (userEntityrepo.Any(x => x.userid == userid)) return;
            var baseUser = new BaseListResult();
            baseUser.userid = userid;
            baseUser.Id = _idGenerator.CreateId().ToString();
            baseUser.mine = new UserEntity()
            {
                Id= _idGenerator.CreateId().ToString(),
                sign = "暂无签名",
                status = OnlineStatus.online.ToString(),
                username = _workContext.CurrentUser.UserName,
                userid = userid,
            };
            var friendGroupEntity = new FriendGroupEntity()
            {
                Id = _idGenerator.CreateId().ToString(),
                groupname = "默认分组",
                groupdesc = "默认分组",
                online = 0,
                userid = userid,
                isdefault = true,
            };
            //添加到用户分组列表
            baseUser.friend.Add(friendGroupEntity);
            userEntityrepo.Add(baseUser);
        }

        public void ApplyToClient(ApplyEntity apply)
        {
            //判断目标用户是否开启聊天，否则返回
            if (!userEntityrepo.Any(x => x.userid == apply.targetid)) return;
            if (!userEntityrepo.Any(x => x.userid == apply.userid))
            {
                CreateUser(apply.userid);
            }
            //获取用户信息
            //添加到好友列表
            var userBaseInfo = userEntityrepo.First(x => x.userid == apply.userid);
            var friend = userBaseInfo.friend.First(x => x.isdefault);
            var targetUser = userEntityrepo.First(x => x.userid == apply.targetid);
            var istargetId = false;
            //判断是否已在好友列表中
            foreach (FriendGroupEntity fg in userBaseInfo.friend)
            {
                if (fg.list.Any(x => x.userid == apply.targetid))
                {
                    istargetId = true;
                    break;
                }
            }
            if (!istargetId)
            {
                friend.list.Add(new GroupUserEntity()
                {
                    Id = _idGenerator.CreateId().ToString(),
                    userid = targetUser.mine.userid,
                    username = targetUser.mine.username,
                    remarkname = targetUser.mine.username,
                    sign = targetUser.mine.sign,
                    avatar = targetUser.mine.avatar,
                });
                userEntityrepo.Update(userBaseInfo);
            }

            //添加到目标好友列表
            var isUserId = false;
            var targetUserBaseInfo = userEntityrepo.First(x => x.userid == apply.targetid);
            var targetFriend = targetUserBaseInfo.friend.First(x => x.isdefault);

            //判断是否已在好友列表中
            foreach (FriendGroupEntity fg in targetUserBaseInfo.friend)
            {
                if (fg.list.Any(x => x.userid == apply.userid))
                {
                    isUserId = true;
                    break;
                }
            }
            if (!isUserId)
            {
                targetFriend.list.Add(new GroupUserEntity()
                {
                    Id = _idGenerator.CreateId().ToString(),
                    userid = userBaseInfo.mine.userid,
                    username = userBaseInfo.mine.username,
                    remarkname = userBaseInfo.mine.username,
                    sign = userBaseInfo.mine.sign,
                    avatar = userBaseInfo.mine.avatar,
                });
                userEntityrepo.Update(targetUserBaseInfo);
            }
        }
        #endregion

        #region 根据用户ID判断某个用户是否在线

        public bool IsOnline(string userId)
        {
            return onlineUserrepo.Any(x => x.Id == userId);

        }
        #endregion



        #region 获取用户好友列表
        public string GetUserFriendList(string userId)
        {
            var baseUser = new BaseListResult();
            var userFriend_str = "";
            if (userEntityrepo.Any(x => x.userid == userId))
            {
                baseUser = userEntityrepo.First(x => x.userid == userId);
            }
            //判断是否已在好友列表中
            foreach (FriendGroupEntity fg in baseUser.friend)
            {
                userFriend_str += string.Join("$LAYIM$", fg.list);

            }
            return userFriend_str;

        }
        #endregion

    }
}
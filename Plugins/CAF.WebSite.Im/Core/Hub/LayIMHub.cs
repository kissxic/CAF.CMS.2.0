
using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Core;
using CAF.Message.Distributed.Extensions.Models;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Im.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Im.Core
{
    [HubName("layimHub")]
    public class LayIMHub : Hub, ILayIMHub
    {
        #region 变量
        private readonly ILayIMCache _layIMCache;
       // private static IBaseMessage _baseMessage;
        #endregion
        public LayIMHub()
        {
          //  _baseMessage = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IBaseMessage>();
            _layIMCache = CAF.Infrastructure.Core.EngineContext.Current.Resolve<ILayIMCache>();
        }


        /// <summary>
        /// 当前的connectionId
        /// </summary>
        public string CurrentConnectId
        {
            get
            {
                return Context.ConnectionId;
            }
        }
        /// <summary>
        /// 当前的用户ID
        /// </summary>
        public string CurrentUserId
        {
            get
            {
                return _layIMCache.GetCurrentUserId();
            }
        }
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        private OnlineUser CurrentOnlineUser
        {
            get
            {
                return new OnlineUser
                {
                    connectionid = CurrentConnectId,
                    userid = CurrentUserId
                };
            }
        }
        /// <summary>
        /// 建立连接
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            UserOnline();
            return Clients.Caller.receiveMessage("连接成功");
        }
        /// <summary>
        /// 失去连接
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            UserOffline();
            return Clients.Caller.receiveMessage("失去连接");
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            UserOnline();
            return Clients.Caller.receiveMessage("重新连接");
        }

        /// <summary>
        /// 单聊连接 1v1
        /// </summary>
        public Task ClientToClient(string fromUserId, string toUserId)
        {

            //放弃组聊天方法，直接用User发送，因为如果不打开聊天窗口没法聊天，而且还有其他很多connectionid变化的问题
            //var groupId =ChatGroupHepler.CreateName(fromUserId, toUserId);
            ////将当前连接人加入到组织内(同理，对方恰好在线的话，那么他们相当于在同一个聊天室内，就可以愉快的聊天了)
            //Groups.Add(CurrentConnectId, groupId);
            var isOneline = _layIMCache.IsOnline(toUserId);
            //将这里的业务逻辑改为判断对方是否在线
            return Clients.Caller.receiveMessage(new ToClientMessageResult
            {
                msg = isOneline ? "" : "对方处在离线状态，给他留言吧",
                msgtype = ChatToClientType.UserIsOnLineToClient,
                other = new { online = isOneline, userid = toUserId }
            });
            // return Clients.User(CurrentUserId).receiveMessage();
            //return Task.Run(() =>
            // {
            //     HubServerHelper.SendMessage(new ToClientMessageResult
            //     {
            //         msg = isOneline ? "" : "对方处在离线状态，给他留言吧",
            //         msgtype = ChatToClientType.UserIsOnLineToClient,
            //         other = new { online = isOneline, userid = toUserId }
            //     });
            // });
        }

        /// <summary>
        /// 群聊
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public Task ClientToGroup(string fromUserId, string toGroupId)
        {
            var groupId = ChatGroupHepler.CreateName(toGroupId);
            return Clients.User(fromUserId).receiveMessage("连接聊天室" + groupId + "成功");
        }
        /// <summary>
        /// 客户端聊天消息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task ClientSendMsgToClient(ClientToClientMessage message)
        {
            //先生成groupid
            var groupId = ChatGroupHepler.CreateName(message.mine.Id, message.to.Id);
            message.roomid = groupId;
            //处理保存消息业务
            Task.Run(() =>
            {
              //  _baseMessage.SendClientToClientMessage(message);
            });
            #region  //发送给客户端

            ClientToClientReceivedMessage tomessage = new ClientToClientReceivedMessage
            {
                fromid = message.mine.Id,
                Id = message.mine.Id,//发送人id
                avatar = message.mine.avatar, //发送人头像
                content = message.mine.content,//发送内容
                type = message.to.type,//类型 这里一定是friend
                username = message.mine.username//发送人姓名
            };
            ToClientMessageResult result = new ToClientMessageResult
            {
                msg = tomessage,
                other = null,
                msgtype = ChatToClientType.ClientToClient
            };
            #endregion
            //取消这种发送消息方式，改为直接向该用户发送
            //return Clients.Group(groupId).receiveMessage(result);
            var toUserId = message.to.Id;
            return Clients.User(toUserId).receiveMessage(result);
        }
        /// <summary>
        /// 群组发送消息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task ClientSendMsgToGroup(ClientToGroupMessage message)
        {
            //先生成groupid
            var groupId = ChatGroupHepler.CreateName(message.to.Id);
            message.roomid = groupId;
            //保存消息
            Task.Run(() =>
            {
               // _baseMessage.SendClientToGroupMessage(message);
            });
            //处理保存消息业务
            //发送给客户端
            ClientToClientReceivedMessage tomessage = new ClientToClientReceivedMessage
            {
                fromid = message.mine.Id,
                Id = message.to.Id,//注意，这里的群组发送人，就应该是群id了
                avatar = message.mine.avatar, //发送人头像
                content = message.mine.content,//发送内容
                type = message.to.type,//类型 这里一定是friend
                username = message.mine.username//发送人姓名
            };
            ToClientMessageResult result = new ToClientMessageResult
            {
                msg = tomessage,
                other = null,
                msgtype = ChatToClientType.ClientToGroup//这里是群组类型
            };
            return Clients.Group(groupId).receiveMessage(result);
        }

        public Task ServerSendGroupCreatedMsgToGroup(UserGroupCreatedMessage message)
        {
            return Clients.All.receiveMessage(message);
        }


        #region 用户上下线操作
        private void UserOnline()
        {
            //将当前用户添加到redis在线用户缓存中
            _layIMCache.OperateOnlineUser(CurrentOnlineUser);
            //发送用户上线消息
            HubServerHelper.SendUserOnOffLineMessage(CurrentUserId);
            //由于用户群一般不多，这里直接将用户全部加入群组中
            //var groups = LayimUserBLL.Instance.GetUserAllGroups(CurrentUserId);
            ////遍历组，该connectionId加入到组中，防止收不到消息
            //foreach (string group in groups)
            //{
            //    var g = GroupHelper.CreateGroup().CreateName(group.ToInt());
            //    Groups.Add(CurrentConnectId, g);
            //}
        }

        private void UserOffline()
        {
            //将当前用户从在线用户列表中剔除
            _layIMCache.OperateOnlineUser(CurrentOnlineUser, isDelete: true);
            //发送用户下线消息
            HubServerHelper.SendUserOnOffLineMessage(CurrentUserId, online: false);
        }
        #endregion


        #region 服务器操作


        #endregion
    }
}
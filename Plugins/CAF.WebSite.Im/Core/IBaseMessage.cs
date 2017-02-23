using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Models;
using CAF.WebSite.Im.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Im.Core
{
    public interface IBaseMessage
    {
        /// <summary>
        /// 发送消息，处理逻辑由详细继承类实现，直接保存数据库，或者走队列，或者走ES
        /// </summary>
        /// <returns></returns>
        SendMessageResult SendClientToClientMessage(ClientToClientMessage message);
        SendMessageResult SendClientToGroupMessage(ClientToGroupMessage message);
        SendMessageResult SendToClientMessageResult(ToClientMessageResult message);
        SendMessageResult SendUserOnOffLineMessage(UserOnOffLineMessage message);
    }
}
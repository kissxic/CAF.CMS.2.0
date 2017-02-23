using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.Message.Job
{
    public class HubClientHelper
    {
        static HubConnection conn = new HubConnection("http://www.virayer.cn/signalr/hubs");
        //获取当前的Hub对象
        static IHubProxy hubProxy
        {
            get
            {
                //成员变量定义，url的www.virayer.cn，可以改成你们的本地服务或者上线服务的
                //如：http://localhost:8080/signalr/hubs

                IHubProxy ihubProxy;


                //和pushHub家里代理，这个名字要和服务端类上面的[HubName("pushHub")]括号里面的要一致
                ihubProxy = conn.CreateHubProxy("layimHub");

                //监听方法，如果有多个参数可以ihubProxy.On<string,string>("sendallclient", (param1,parma2) =>{//接收到推送信息的处理});
                //ihubProxy.On<string>("sendallclient", (str) =>
                //{
                //    //  因为传回来的是Json，我直接用NameValueCollection处理掉，可以直接调用出来，下面附上方法
                //    //  var result = ParseJson(str);
                //    //委托调用 处理listBox1跨线程问题,
                //    // this.Invoke((Action)(() =>
                //    //listBox1.Items.Add(result["ChatUserName"] + "说：" + result["Content"])
                //    //));
                //});
                conn.Start();

                return ihubProxy;

            }
        }
        public static void SendClientToClientMessage(ClientToClientMessage message)
        {
            //调用服务端的Send方法
            hubProxy.Invoke("receiveMessage", null);
        }
        public static void SendClientToGroupMessage(ClientToGroupMessage message)
        {
            //调用服务端的Send方法
            hubProxy.Invoke("receiveMessage", null);

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
            //return Clients.Group(groupId).receiveMessage(result);
        }
    }
}
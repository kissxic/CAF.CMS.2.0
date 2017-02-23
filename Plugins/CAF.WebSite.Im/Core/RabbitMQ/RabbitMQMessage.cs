using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Models;
using CAF.WebSite.Im.Core;
using CAF.WebSite.Im.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.WebSite.Im.RabbitMQ
{
    public class RabbitMQMessage : IBaseMessage
    {
        private readonly IBusControl _bus;
        public RabbitMQMessage()
        {
            this._bus = MessageEngineContext.Current.Bus;
        }

        public SendMessageResult SendClientToClientMessage(ClientToClientMessage message)
        {
            return Publish(message);

        }
        public SendMessageResult SendClientToGroupMessage(ClientToGroupMessage message)
        {
            return Publish(message);

        }
        public SendMessageResult SendToClientMessageResult(ToClientMessageResult message)
        {
            return Publish(message);

        }
        public SendMessageResult SendUserOnOffLineMessage(UserOnOffLineMessage message)
        {

            return Publish(message);
        }

        private SendMessageResult Publish(object message)
        {
            try
            {

                _bus.Publish(message);

                return new SendMessageResult(true);
            }
            catch (Exception)
            {
                return new SendMessageResult(false);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using MassTransit;
using CAF.Message.Distributed.Extensions.Models;

namespace CAF.Message.Job
{
    public class GreetingEventConsumerChat : IConsumer<ClientToClientMessage>
    {
        public async Task Consume(ConsumeContext<ClientToClientMessage> context)
        {
            await Console.Out.WriteLineAsync($"receive greeting eventA: id {context.Message.addtime}");
        }
    }
    public class UserOnOffLineMessageEventConsumer : IConsumer<UserOnOffLineMessage>
    {
        public async Task Consume(ConsumeContext<UserOnOffLineMessage> context)
        {
            await Console.Out.WriteLineAsync($"receive greeting eventA: id {context.Message.userid}");
        }
    }

    public class ToClientMessageResultEventConsumerChat : IConsumer<ToClientMessageResult>
    {
        public async Task Consume(ConsumeContext<ToClientMessageResult> context)
        {
            await Console.Out.WriteLineAsync($"receive greeting eventA:  {context.Message.msg}");
        }
    }
}
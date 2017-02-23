using CAF.Message.Distributed.Extensions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Message.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SubscriberA");
            var bus = BusCreator.CreateBus((cfg, host) => cfg.ReceiveEndpoint(host, RabbitMqConstants.GreetingEventSubscriberChatQueue, e =>
            {
                e.Consumer<GreetingEventConsumerChat>();
                e.Consumer<UserOnOffLineMessageEventConsumer>();
            }));

            bus.Start();

            Console.WriteLine("Listening for Greeting events.. Press enter to exit");
            Console.ReadLine();

            bus.Stop();
        }
    }
}

namespace CAF.Message.Distributed.Extensions
{
    public class RabbitMqConstants
    {
        public const string RabbitMqUri = "rabbitmq://59.46.80.163:5673/";
        public const string UserName = "tang.rb";
        public const string Password = "admin123";
        public const string GreetingQueue = "greeting.service";
        public const string HierarchyMessageSubscriberQueue = "hierarchyMessage.subscriber.service";
        public const string GreetingEventSubscriberChatQueue = "greetingEvent.subscriberChat.service";
 

        public const string RequestClientQueue = "Request.Service";

        public const string MongoServerSettings = "mongodb://localhost:27017/Message";
    }
}
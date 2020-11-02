using Posting.Messaging.Send.Options;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
namespace Posting.Messaging.Send.Sender
{
    public class PostUpdateSender : IPostUpdateSender
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _virtualHost;

        public PostUpdateSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = "lah";//rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            //_virtualHost = rabbitMqOptions.Value.VirtualHost;
        }

        public void SendCustomer(string message)
        {
            var factory = new ConnectionFactory() { HostName = _hostname, UserName = _username, Password = _password, VirtualHost = "/" };

            using (var connection = factory.CreateConnection())
            //ConnectionFactory factory = new ConnectionFactory();
            //factory.UserName = "guest";
            //factory.Password = "guest";
            //factory.VirtualHost = "/";
            //factory.HostName = "localhost";
            //IConnection connection = factory.CreateConnection();

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchange:"message", type: "direct");
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "message", routingKey: "black", basicProperties: null, body: body);
            }
        }
    }
}

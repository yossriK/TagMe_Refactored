using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comment.Messaging.Recieve.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Comment.Messaging.Recieve.Reciever
{
    //. I love .NET Core and it comes really handy here. .NET Core provides the abstract class BackgroundService which provides the method ExecuteAsync. This method can be overriden and is executed regularly in the background.
    public class BlackMessageReciever : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        // this can refer to a handler in your system or something. not necessary atm
        //private readonly ICustomerNameUpdateService _customerNameUpdateService;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public BlackMessageReciever(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = "lah";// rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            // _customerNameUpdateService = customerNameUpdateService; this can be passed in also, and will most likely be done through autofac
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.ExchangeDeclare(exchange: "message", type: "direct");
            _channel.QueueBind(queue: "lah", exchange: "message", routingKey: "black"); // https://www.rabbitmq.com/tutorials/tutorial-four-python.html routing key and exchange, we can tune our routing and what we want to subscribe to: depends on severity and stuff. I guess I can do a dedicated queue for each microservice, and based on content headers I would redirect them( I just want a basic thing running, not much worried about a fine fine detail). wil be adding other mciroserivces later and workign with tehnm 

        }



        // implemented from the interface
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<string>(content);

                /// TODO: for future implementation to pass this to other modules for the update
                 //  HandleMessage(updateCustomerFullNameModel);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }
        private void HandleMessage()
        {
            // _customerNameUpdateService.UpdateCustomerNameInOrders(updateCustomerFullNameModel);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        // I guess this is an event handler that can be fired or somethign
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

    }
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderServices.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderServices.Messaging
{
    public class OrderCreateReceiver : BackgroundService
    {

        // BackgroundService -> using Extensions.Hosting
        private IModel _channel; //AMQP Model
        private IConnection _connection;  //install RabbitMq.Client

       // private readonly IOrderCreateService _orderCreateService;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public OrderCreateReceiver(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            //  _customerNameUpdateService = customerNameUpdateService;
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

            //durable : true (order niet noodzakelijk gestart)
            _connection = factory.CreateConnection();
            //     _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            //declaratie moet overeenstemmen met sender queue: BasicPublish zonder routingkey
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
         // _channel.QueueBind(queue: _queueName, exchange: "", routingKey: "Cart"); 
       
        
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //overriding van de generic Hosted BackgroundService
            stoppingToken.ThrowIfCancellationRequested();

            //using client.Evens
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {

                //byte[] msgContent = new byte[200];
                var msgBody = ea.Body.ToArray(); //type byte<> omzetten naar Array

                //from byte[] to string (byte array)
                var content = Encoding.UTF8.GetString(msgBody);

                
                var cartModel = JsonConvert.DeserializeObject<CartModel>(content);

                HandleMessage(cartModel);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
   
            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(CartModel cartModel)
        {
            //TODO: afwerking RabbitMessage - CreateOrder
            Console.WriteLine("Order nog verwerken en opslaan in de database.");
      
        }
    }
}

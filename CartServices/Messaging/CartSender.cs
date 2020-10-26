using CartServices.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartServices.Messaging
{
    public class CartSender : ICartSender
    {
        private readonly IOptions<RabbitMqConfiguration> rabbitMqOptions;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public CartSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            this.rabbitMqOptions = rabbitMqOptions;
            _hostname = rabbitMqOptions.Value.HostName; //localhost
            _queueName = rabbitMqOptions.Value.QueueName; //CartQueue
            _username = rabbitMqOptions.Value.UserName; //guest
            _password = rabbitMqOptions.Value.Password; //guest
        }

        public void SendCart(Cart cart)
        {
            //using RabbitMQ.Client
            var factory = new ConnectionFactory() { HostName = _hostname, UserName = _username, Password = _password };

            using (var connection = factory.CreateConnection())  //error bij onbereikbare endpoints 
                                                                 //- controleer de Hostname
            using (var channel = connection.CreateModel())
            {
                //durable: true om de booschap te bewaren 
               // channel.ExchangeDeclare(exchange: "direct_carts", type: "direct");
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var json = JsonConvert.SerializeObject(cart); //serialize (met Netonsoft.Json)
                var body = Encoding.UTF8.GetBytes(json);//encodeer naar UTF8)-> byte array als result

             
                //meerdere luisteraars op zelfde routingKey zijn mogelijk
                //ook meeredere routing keys kunnen in de bind uitgelzen met een for lus.
                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);

              //  Console.WriteLine(" Cart details sent with id {0} for {1} item(s)}", cart.Id, cart.CartItems.Count());
            }

        }
    }
}

using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SettlBroker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var exchangeName = "rabbit";
            var queueName = "task";
            var routingKey = "hello";
            var username = "rabbitmq-user";
            var password = "N84q6BwC9qmdgGj";

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://b-c252341c-815d-4df4-8066-489d7e06289e.mq.eu-central-1.amazonaws.com:5671"),
                UserName = username,
                Password = password
            };

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            var response = channel.QueueDeclarePassive(queueName);
            // returns the number of messages in Ready state in the queue
            var messages = response.MessageCount;
            // returns the number of consumers the queue has
            var consumers = response.ConsumerCount;

            //IBasicProperties props = channel.CreateBasicProperties();
            //props.ContentType = "text/plain";
            //props.DeliveryMode = 2;
            //props.Headers = new Dictionary<string, object>();
            //props.Headers.Add("latitude", 51.5252949);
            //props.Headers.Add("longitude", -0.0905493);
            //props.Expiration = "36000000";
            var message = "Hello, world!";
            var json = "{\"DeptId\": 101, \"DepartmentName\": \"IT\"}";
            // byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            byte[] jsonBodyBytes = Encoding.UTF8.GetBytes(json);
            //channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
            //Console.WriteLine(" [x] Sent {0}", message);
            Thread.Sleep(5000);
            channel.BasicPublish(exchangeName, routingKey, null, jsonBodyBytes);
            Console.WriteLine(" [x] Sent {0}", json);

            channel.Close();
            conn.Close();

            Console.ReadLine();
        }
    }
}
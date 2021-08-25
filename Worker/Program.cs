using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var queueName = "task";
            var username = "rabbitmq-user";
            var password = "N84q6BwC9qmdgGj";

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://b-c252341c-815d-4df4-8066-489d7e06289e.mq.eu-central-1.amazonaws.com:5671"),
                UserName = username,
                Password = password
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(0, 1, false);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                var department = JsonConvert.DeserializeObject<Department>(message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
    }

    public class Department
    {
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
    }
}
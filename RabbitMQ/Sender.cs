namespace RabbitMQ
{
    using System;
    using RabbitMQ.Client;
    using System.Text;
    using Newtonsoft.Json;

    public class Producer
    {
        public static void Send<T>(string tipoEvento, T message)
        {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: tipoEvento,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false);

                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: tipoEvento,
                                         body: body);
                }
        }
    }
}
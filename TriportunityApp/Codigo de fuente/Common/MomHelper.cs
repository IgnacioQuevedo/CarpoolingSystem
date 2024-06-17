using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common;

public static class MomHelper
{
    public static void EstablishExchangeAndQueue(string queueName, string exchangeName, string routingKey, IModel channel)
    {
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            
            channel.QueueDeclare(
                queue: queueName, 
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);
        }
    }

    public static void PublishMessage(string exchangeName, string routingKey, object objectToSend, IModel channel)
    {
        try
        {
            string JsonBodyMessage = JsonSerializer.Serialize(objectToSend);
            var body = Encoding.UTF8.GetBytes(JsonBodyMessage);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; 

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }
        catch (Exception exceptionCaught)
        {
            Console.WriteLine(exceptionCaught.Message);
            throw;
        }
 
    }
}
using System.Text.Json;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.DataAccess.Repositories;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.Services;

public class MQRideService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public MQRideService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
        var factory = new ConnectionFactory();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        MomHelper.EstablishExchangeAndQueue(MomConstraints.rideQueueName, MomConstraints.exchangeName,
            MomConstraints.rideQueueRoutingKey, _channel);

        AddRideEvent();
    }

    private void AddRideEvent()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            RideEvent rideEvent = JsonSerializer.Deserialize<RideEvent>(body);

            if (rideEvent is null)
            {
                Console.WriteLine("An error has being produced while trying to get the ride event. Call a programmer.");
            }
            
            // Create a new scope to resolve scoped services
            using (var scope = _scopeFactory.CreateScope())
            {
                var rideEventRepository = scope.ServiceProvider.GetRequiredService<IRideEventRepository>();
                rideEventRepository.AddRideEvent(rideEvent);
            }
        };

        _channel.BasicConsume(queue: MomConstraints.rideQueueName, autoAck: true, consumer: consumer);
        
    }
    
    public void Dispose()
    {
        _connection?.Close();
        _channel?.Close();
    }
}
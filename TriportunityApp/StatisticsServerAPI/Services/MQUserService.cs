using System.Text.Json;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.DataAccess.Repositories;
using StatisticsServerAPI.MqDomain;

namespace StatisticsServerAPI.Services;

public class MQUserService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public MQUserService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
        var factory = new ConnectionFactory();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        MomHelper.EstablishExchangeAndQueue(MomConstraints.userQueueName, MomConstraints.exchangeName,
            MomConstraints.userQueueRoutingKey, _channel);

        AddLoginEvent();
    }

    private void AddLoginEvent()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            LoginEvent LoginEvent = JsonSerializer.Deserialize<LoginEvent>(body);
            
            
            // Create a new scope to resolve scoped services
            using (var scope = _scopeFactory.CreateScope())
            {
                var loginEventRepository = scope.ServiceProvider.GetRequiredService<ILoginEventRepository>();
                loginEventRepository.AddLoginEvent(LoginEvent);
            }
        };

        _channel.BasicConsume(queue: MomConstraints.userQueueName, autoAck: true, consumer: consumer);
        
    }

    private readonly MQUserService _mqUserService;


    public void Dispose()
    {
        _connection?.Close();
        _channel?.Close();
    }
}
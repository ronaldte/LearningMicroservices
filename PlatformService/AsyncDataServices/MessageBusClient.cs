using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory() 
        {
            HostName = _configuration["RabbitMQHost"]!,
            Port = int.Parse(_configuration["RabbitMQPort"]!)
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel?.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to the message bus {ex}");
        }
    }
    
    public void PubilshNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var messageObject = JsonSerializer.Serialize(platformPublishedDto);

        if(_connection is not null && _connection.IsOpen)
        {
            SendMessage(messageObject);
        }
        else
        {
            Console.WriteLine("[RabbitMQ connections closed, not sending...]");
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel?.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"[RabbitMQ connection shutdown]");
    }
}
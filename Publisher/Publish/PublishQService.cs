using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace Publish
{
    public class PublishQService
    {
        private readonly IConnectedQService _connection;
        private readonly ILogger<PublishQService> _logger;
        private readonly int _retryCount;

        public PublishQService(IConnectedQService connection, ILogger<PublishQService> logger, int retryCount = 5)
        {
            _connection = connection;
            _logger = logger;
            _retryCount = retryCount;
        }

        public void Publish(string queueName, IBaseQConfiguration baseEvent)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>().Or<SocketException>()
            .WaitAndRetry(_retryCount, tryAgain => TimeSpan.FromSeconds(Math.Pow(2, tryAgain)), (ex, time, context) =>
            {
                _logger.LogWarning(ex, "Event publishing failed: Event Id {Id} could not be published after {Timeout} seconds. Error: {ExceptionMessage}", baseEvent.Id, $"{time.TotalSeconds:n1}", ex.Message);
            });

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName);
                var message = JsonConvert.SerializeObject(baseEvent);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;

                    channel.ConfirmSelect();
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                    channel.WaitForConfirmsOrDie();

                    channel.BasicAcks += (sender, eventArgs) =>
                    {
                        _logger.LogInformation("Event published successfully to RabbitMQ");
                    };
                });
            }
        }
    }
}

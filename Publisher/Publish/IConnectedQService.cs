using RabbitMQ.Client;

namespace Publish
{
    public interface IConnectedQService : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}

namespace Publish
{
    public abstract class IBaseQConfiguration
    {
        public Guid Id { get; private init; }
        public DateTime Created { get; private init; }

        public IBaseQConfiguration()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }
    }
}

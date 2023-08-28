namespace Plooto.Payments.Domain.Interfaces
{
    public interface IEntity<T>
    {
        T Id { get; }
    }
}

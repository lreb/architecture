namespace Facware.ModularMonolith.SharedKernel.Domain;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        OccurredOnUtc = DateTime.UtcNow;
    }

    public DateTime OccurredOnUtc { get; }
}
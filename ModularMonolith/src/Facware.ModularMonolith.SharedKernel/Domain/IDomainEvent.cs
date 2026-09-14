namespace Facware.ModularMonolith.SharedKernel.Domain;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
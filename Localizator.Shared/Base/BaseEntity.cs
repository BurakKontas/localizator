using Localizator.Shared.Mediator.Interfaces;
using System.Collections.ObjectModel;

namespace Localizator.Shared.Base;

public abstract class BaseEntity
{
    public long Id { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Concurrency
    public byte[] RowVersion { get; set; } = default!;
    public ReadOnlyCollection<IRequest> DomainEvents => _domainEvents.AsReadOnly();

    private List<IRequest> _domainEvents = new();

    public void Arise(IRequest @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
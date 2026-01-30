namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public class NamespaceUserPermission
{
    public string User { get; private set; } = default!;
    private readonly List<NamespacePermission> _permissions = new();

    public IReadOnlyCollection<NamespacePermission> Permissions => _permissions.AsReadOnly();

    private NamespaceUserPermission() { } // EF

    public NamespaceUserPermission(string user, IEnumerable<NamespacePermission> permissions)
    {
        User = user;
        _permissions = permissions.ToList();
    }

    public bool HasPermission(NamespacePermission required)
        => _permissions.Any(p => p.HasPermission(required));

    internal void AddPermission(NamespacePermission permission)
    {
        if (_permissions.Any(p => p.Value == permission.Value))
            return;

        _permissions.Add(permission);
    }

    internal void RemovePermission(string permission)
    {
        _permissions.RemoveAll(p => p.Value == permission.ToUpperInvariant());
    }
}

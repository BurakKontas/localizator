using Localizator.Namespace.Domain.Namespace.Enums;
using Localizator.Namespace.Domain.Namespace.ValueObjects;
using Localizator.Shared.Base;
using Localizator.Shared.Exceptions;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace;

public class Namespace : BaseEntity
{
    private readonly List<NamespaceUserPermission> _permissions = new();


    // Private setters - encapsulation
    public NamespaceName Name { get; private set; }
    public NamespaceSlug Slug { get; private set; }
    public List<SupportedLanguage> SupportedLanguages { get; private set; }
    public IReadOnlyCollection<NamespaceUserPermission> Permissions => _permissions.AsReadOnly(); 
    public NamespaceStatus Status { get; private set; }
    public NamespaceVersion CurrentVersion { get; private set; }
    public PublishedAt? LastPublishedAt { get; private set; }
    public PublishedBy? LastPublishedBy { get; private set; }
    public bool IsPublic { get; private set; }


    // EF Core için parameterless constructor
    private Namespace()
    {
    }

    // Private constructor - sadece factory method'lar kullanılmalı
    private Namespace(string createdBy, NamespaceName name, NamespaceSlug slug, List<SupportedLanguage> supportedLanguages, bool isPublic) : base()
    {
        Name = name;
        Slug = slug;
        SupportedLanguages = supportedLanguages;
        Status = NamespaceStatus.Draft;
        CurrentVersion = NamespaceVersion.Initial();
        IsPublic = isPublic;

        _permissions.Add(
            new NamespaceUserPermission(
                createdBy,
                [new NamespacePermission(NamespacePermission.CREATOR)]
            )
        );

        // Domain Event
        //Arise(new NamespaceCreatedDomainEvent(id, name, slug));
    }

    public static Namespace Create(string createdBy, NamespaceName name, NamespaceSlug slug, List<SupportedLanguage> supportedLanguages, bool isPublic = false)
    {
        if (supportedLanguages.Count == 0)
            throw new BusinessException(Errors.AtLeastOneLanguageRequired);


        return new Namespace(createdBy, name, slug, supportedLanguages, isPublic);
    }

    public void UpdateName(NamespaceName newName)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotUpdateArchivedNamespace);

        if (Name.Equals(newName))
            return;

        Name = newName;
        //Arise(new NamespaceNameUpdatedDomainEvent(Id, newName));
    }

    public void UpdateSlug(NamespaceSlug newSlug)
    {
        if (Status.IsPublished || Status.IsArchived)
            throw new BusinessException(Errors.CannotChangePublishedSlug);

        Slug = newSlug;
    }

    public void Publish(PublishedBy publisher)
    {
        if (Status.IsPublished)
            throw new BusinessException(Errors.NamespaceAlreadyPublished);

        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotPublishArchivedNamespace);

        Status = NamespaceStatus.Published;
        LastPublishedAt = PublishedAt.Now();
        LastPublishedBy = publisher;

        //Arise(new NamespacePublishedDomainEvent(Id, publisher, LastPublishedAt));
    }

    public void Unpublish()
    {
        if (Status.IsDraft)
            throw new BusinessException(Errors.NamespaceAlreadyDraft);

        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotUnpublishArchivedNamespace);

        Status = NamespaceStatus.Draft;
        //Arise(new NamespaceUnpublishedDomainEvent(Id));
    }

    public void Archive()
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.NamespaceAlreadyArchived);

        Status = NamespaceStatus.Archived;
        //Arise(new NamespaceArchivedDomainEvent(Id));
    }

    public void Restore()
    {
        if (!Status.IsArchived)
            throw new BusinessException(Errors.OnlyArchivedCanBeRestored);

        Status = NamespaceStatus.Draft;
        //Arise(new NamespaceRestoredDomainEvent(Id));
    }

    public void AddLanguage(string languageCode)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        if (SupportedLanguages.Any(lang => lang.Value == languageCode))
            return;

        SupportedLanguage newLang = new SupportedLanguage(languageCode.ToLowerInvariant());

        SupportedLanguages.Add(newLang);

        //Arise(new LanguageAddedDomainEvent(Id, languageCode));
    }

    public void RemoveLanguage(string languageCode)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        if (SupportedLanguages.Any(lang => lang.Value == languageCode))
            return;

        if (SupportedLanguages.Count == 1)
            throw new BusinessException(Errors.NamespaceMustSupportOneLanguage);

        SupportedLanguages.RemoveAll(lang => lang.Value != languageCode.ToLowerInvariant());

        //Arise(new LanguageRemovedDomainEvent(Id, languageCode));
    }

    public void AddPermission(string user, string permission)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        var newPermission = new NamespacePermission(permission);

        var userPermissions = _permissions.FirstOrDefault(p => p.User == user);

        if (userPermissions is null)
        {
            _permissions.Add(
                new NamespaceUserPermission(user, new[] { newPermission })
            );
            return;
        }

        if (userPermissions.Permissions.Any(p => p.Value == newPermission.Value))
            return;

        userPermissions.AddPermission(newPermission);
    }

    public void RemovePermission(string user, string permission)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        var userPermissions = _permissions.FirstOrDefault(p => p.User == user);
        if (userPermissions is null)
            return;

        userPermissions.RemovePermission(permission);

        if (!userPermissions.Permissions.Any())
            _permissions.Remove(userPermissions);
    }

    public void RemoveAllPermissions(string user)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        _permissions.RemoveAll(p => p.User == user);
    }

    public void MakePublic()
    {
        if (IsPublic)
            return;

        IsPublic = true;
        //Arise(new NamespaceMadePublicDomainEvent(Id));
    }

    public void MakePrivate()
    {
        if (!IsPublic)
            return;

        IsPublic = false;
        //Arise(new NamespaceMadePrivateDomainEvent(Id));
    }

    public void BumpVersion(VersionBumpType bumpType)
    {
        if (Status.IsArchived)
            throw new BusinessException(Errors.CannotModifyArchivedNamespace);

        CurrentVersion = CurrentVersion.Bump(bumpType);
        //Arise(new NamespaceVersionBumpedDomainEvent(Id, CurrentVersion, bumpType));
    }

    // Query Methods
    public bool CanBePublished()
    {
        return Status.IsDraft && SupportedLanguages.Count > 0;
    }

    public bool CanBeModified()
    {
        return !Status.IsArchived;
    }

    public bool HasPermission(string user, string requiredPermission)
    {
        var permission = new NamespacePermission(requiredPermission);

        var userPermissions = _permissions.FirstOrDefault(p => p.User == user);
        if (userPermissions is null)
            return false;

        return userPermissions.HasPermission(permission);
    }
}
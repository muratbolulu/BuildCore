using BuildCore.SharedKernel.Entities;

namespace BuildCore.Notification.Domain.Entities;

/// <summary>
/// Bildirim şablonu entity'si (Aggregate Root)
/// </summary>
public class NotificationTemplate : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string TemplateType { get; private set; } = string.Empty; // Email, SMS, Push, InApp
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Navigation properties
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();

    // Private constructor for EF Core
    private NotificationTemplate() { }

    public NotificationTemplate(
        string name,
        string templateType,
        string subject,
        string body)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Şablon adı boş olamaz", nameof(name));

        if (string.IsNullOrWhiteSpace(templateType))
            throw new ArgumentException("Şablon tipi boş olamaz", nameof(templateType));

        Name = name;
        TemplateType = templateType;
        Subject = subject;
        Body = body;
        IsActive = true;
    }

    public void Update(string name, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Şablon adı boş olamaz", nameof(name));

        Name = name;
        Subject = subject;
        Body = body;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

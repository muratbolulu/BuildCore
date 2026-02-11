namespace BuildCore.Notification.Domain.ValueObjects;

/// <summary>
/// Bildirim kanalı value object
/// </summary>
public class NotificationChannel
{
    public string ChannelType { get; } // Email, SMS, Push, InApp
    public string? Address { get; } // Email address, phone number, etc.

    public NotificationChannel(string channelType, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(channelType))
            throw new ArgumentException("Kanal tipi boş olamaz", nameof(channelType));

        ChannelType = channelType;
        Address = address;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not NotificationChannel other)
            return false;

        return ChannelType == other.ChannelType && Address == other.Address;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ChannelType, Address);
    }
}

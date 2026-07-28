namespace Aspotus.Notifications.Worker.Data;

/// <summary>
/// Идентификатор успешно обработанного сообщения.
/// </summary>
public sealed class ReceivedMessage
{
    public Guid Id { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}

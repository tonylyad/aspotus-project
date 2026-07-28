namespace Aspotus.Orders.Api.Data.Entities;

/// <summary>
/// Сообщение, ожидающее публикации в брокер.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int Attempts { get; set; }
    public string? Error { get; set; }
}

namespace BookingSystem.Models;
public class Ticket
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow.AddHours(5);
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Pending;
}
public enum TicketPriority
{
    Low,
    Medium,
    High
}
public enum TicketStatus
{
    Pending,
    Processing,
    Completed
}

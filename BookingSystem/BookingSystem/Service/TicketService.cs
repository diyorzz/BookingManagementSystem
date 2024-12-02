using ActualLab.Fusion;
using BookingSystem.Interface;
using BookingSystem.Models;

namespace BookingSystem.Service;

public class TicketService : ITicketService
{
    private List<Ticket> _tickets = new List<Ticket>();

    [ComputeMethod]
    public virtual Task<List<Ticket>> GetTickets()
    {
        var result = _tickets
            .Where(t => t.Status == TicketStatus.Pending || t.Status == TicketStatus.Processing)
            .ToList();

        return Task.FromResult(result);
    }
    public Task AddTicket(int customerId, TicketPriority priority)
    {
        var ticket = new Ticket
        {
            Id = _tickets.Count + 1,
            CustomerId = customerId,
            Priority = priority,
            Status = TicketStatus.Pending,
            CreationTime = DateTime.Now
        };

        _tickets.Add(ticket);

        using (Invalidation.Begin())
            GetTickets();
        return Task.CompletedTask;
    }

    public Task GetNextTicket()
    {
        var ticket = _tickets
        .Where(t => t.Status == TicketStatus.Pending)
        .OrderByDescending(t => t.Priority)
        .ThenBy(t => t.CreationTime)
        .FirstOrDefault();

        if (ticket != null)
        {
            ticket.Status = TicketStatus.Processing;
        }
        using (Invalidation.Begin())
            GetTickets();
        return Task.CompletedTask;
    }

    public Task CompleteTicket(int ticketId)
    {
        var ticket = _tickets.Find(t => t.Id == ticketId);

        if (ticket != null && ticket.Status == TicketStatus.Processing)
        {
            ticket.Status = TicketStatus.Completed;
            _tickets.Remove(ticket);
        }
        using (Invalidation.Begin())
            GetTickets();
        return Task.CompletedTask;
    }
}
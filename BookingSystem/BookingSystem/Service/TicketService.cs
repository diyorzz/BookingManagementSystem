using ActualLab.CommandR;
using ActualLab.CommandR.Configuration;
using ActualLab.Fusion;
using BookingSystem.Interface;
using BookingSystem.Models;
using System.Reactive;

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

    [CommandHandler]
    public virtual Task AddTicket(Add_Command command, CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket
        {
            Id = _tickets.Count + 1,
            CustomerId = command.customerId,
            Priority = command.TicketPriority,
        };

        _tickets.Add(ticket);

        using (Invalidation.Begin())
            GetTickets();
        return Task.CompletedTask;
    }

    [CommandHandler]
    public virtual Task CompleteTicket(Revome_Command command, CancellationToken cancellationToken = default)
    {
        var ticket = _tickets.Find(t => t.Id == command.ticketId);

        if (ticket != null && ticket.Status == TicketStatus.Processing)
        {
            ticket.Status = TicketStatus.Completed;
            _tickets.Remove(ticket);
        }
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
}

public record Add_Command(int customerId, TicketPriority TicketPriority) : ICommand<Unit>;
public record Revome_Command(int ticketId) : ICommand<Unit>;
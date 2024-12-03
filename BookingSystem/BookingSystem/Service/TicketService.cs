using ActualLab.CommandR;
using ActualLab.CommandR.Configuration;
using ActualLab.Fusion;
using ActualLab.Fusion.EntityFramework;
using BookingSystem.Database;
using BookingSystem.Interface;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reactive;

namespace BookingSystem.Service;

public class TicketService : DbServiceBase<BookingManagementDbContext>, ITicketService
{
    public TicketService(IServiceProvider service) : base(service) { }

    [ComputeMethod]
    public virtual async Task<List<Ticket>> GetTickets(CancellationToken cancellation = default)
    {
        var dbContext = await DbHub.CreateDbContext(cancellation).ConfigureAwait(false);

        if (dbContext == null)
        {
            throw new Exception("DbContext yaratishda xato yuz berdi.");
        }

        var result = await dbContext.Tickets
            .Where(t => t.Status == TicketStatus.Pending || t.Status == TicketStatus.Processing)
            .ToListAsync(cancellation);

        return result;
    }

    [CommandHandler]
    public virtual async Task AddTicket(Add_Command command, CancellationToken cancellationToken = default)
    {
        var dbContext = await DbHub.CreateOperationDbContext(cancellationToken).ConfigureAwait(false);
        await using var _1 = dbContext.ConfigureAwait(false);

        var ticket = new Ticket
        {
            CustomerId = command.customerId,
            Priority = command.TicketPriority
        };

        await dbContext.Tickets.AddAsync(ticket, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        using (Invalidation.Begin())
            await GetTickets(cancellationToken);
    }

    [CommandHandler]
    public virtual async Task CompleteTicket(Revome_Command command, CancellationToken cancellationToken = default)
    {
        var dbContext = await DbHub.CreateOperationDbContext(cancellationToken).ConfigureAwait(false);
        await using var _1 = dbContext.ConfigureAwait(false);

        var ticket = await dbContext.Tickets.FindAsync(new object[] { command.ticketId }, cancellationToken).ConfigureAwait(false);

        if (ticket != null && ticket.Status == TicketStatus.Processing)
        {
            ticket.Status = TicketStatus.Completed;
            dbContext.Tickets.Remove(ticket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        using (Invalidation.Begin())
        {
            _ = GetTickets(cancellationToken);
        }
    }

    [CommandHandler]
    public virtual async Task GetNextTicket(Update_Status command, CancellationToken cancellationToken = default)
    {
        var dbContext = await DbHub.CreateOperationDbContext(cancellationToken).ConfigureAwait(false);
        await using var _1 = dbContext.ConfigureAwait(false);

        var ticket = await dbContext.Tickets
            .Where(t => t.Status == TicketStatus.Pending)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreationTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (ticket != null)
        {
            ticket.Status = TicketStatus.Processing;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        using (Invalidation.Begin())
            await GetTickets(cancellationToken);
    }
}

//Commands
public record Add_Command(int customerId, TicketPriority TicketPriority) : ICommand<Unit>;
public record Revome_Command(int ticketId) : ICommand<Unit>;
public record Update_Status() : ICommand<Unit>;
using ActualLab.Fusion;
using BookingSystem.Models;

namespace BookingSystem.Interface
{
    public interface ITicketService : IComputeService
    {
        [ComputeMethod]
        Task<List<Ticket>> GetTickets();
        Task GetNextTicket();
        Task CompleteTicket(int ticketId);
        Task AddTicket(int customerId, TicketPriority ticketPriority);
    }
}

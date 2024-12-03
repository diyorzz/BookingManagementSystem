using ActualLab.CommandR.Configuration;
using ActualLab.Fusion;
using BookingSystem.Models;
using BookingSystem.Service;

namespace BookingSystem.Interface
{
    public interface ITicketService : IComputeService
    {
        [ComputeMethod]
        Task<List<Ticket>> GetTickets(CancellationToken cancellation = default);

        [CommandHandler]
        Task CompleteTicket(Revome_Command revome_Command, CancellationToken cancellation = default);

        [CommandHandler]
        Task AddTicket(Add_Command add_Command, CancellationToken cancellation = default);

        [CommandHandler]
        Task GetNextTicket(Update_Status update_Command, CancellationToken cancellation = default);
    }
}
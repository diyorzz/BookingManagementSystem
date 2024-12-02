using ActualLab.CommandR.Configuration;
using ActualLab.Fusion;
using BookingSystem.Models;
using BookingSystem.Service;

namespace BookingSystem.Interface
{
    public interface ITicketService : IComputeService
    {
        [ComputeMethod]
        Task<List<Ticket>> GetTickets();

        [CommandHandler]
        Task CompleteTicket(Revome_Command revome_Command, CancellationToken cancellation);

        [CommandHandler]
        Task AddTicket(Add_Command add_Comand, CancellationToken cancellation);

        Task GetNextTicket();
    }
}

using Acceloka.Abstractions;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.RevokeTicket
{
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand>
    {
        private readonly ITicketRepository _repo;

        public RevokeTicketHandler(ITicketRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            await _repo.RevokeTicketAsync(request.BookedTicketId, request.TicketCode, request.Quantity);
        }
    }
}
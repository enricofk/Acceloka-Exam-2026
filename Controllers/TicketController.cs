using Acceloka.Entities.TicketEntities.Commands.BookTicket;
using Acceloka.Entities.TicketEntities.Commands.EditBookedTicket;
using Acceloka.Entities.TicketEntities.Commands.RevokeTicket;
using Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets;
using Acceloka.Entities.TicketEntities.Queries.GetBookedTickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets([FromQuery] GetAvailableTicketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTickets([FromBody] BookTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTicket(string bookedTicketId)
        {
            var query = new GetBookedTicketsQuery { BookedTicketId = bookedTicketId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("revoke-ticket")]
        public async Task<IActionResult> RevokeTicket([FromBody] RevokeTicketCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Ticket revoked and quota refunded successfully." });
        }

        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(string bookedTicketId, [FromBody] EditBookedTicketCommand command)
        {
            if (bookedTicketId != command.BookedTicketId)
            {
                return BadRequest("BookedTicketId mismatch between URL and Body.");
            }

            await _mediator.Send(command);
            return Ok(new { message = "Booking updated successfully." });
        }
    }
}
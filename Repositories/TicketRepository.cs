using Acceloka.Abstractions;
using Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets;
using Acceloka.Entities.TicketEntities.Commands.BookTicket;
using Acceloka.Models;
using Dapper;
using Npgsql;
using System.Text;
namespace Acceloka.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IConfiguration _configuration;

        public TicketRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync(GetAvailableTicketsQuery request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);

            var sql = new StringBuilder("SELECT * FROM Tickets WHERE Quota > 0");

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                sql.Append(" AND CategoryName ILIKE @CategoryName");
            }

            if (!string.IsNullOrEmpty(request.TicketCode))
            {
                sql.Append(" AND TicketCode ILIKE @TicketCode");
            }

            if (!string.IsNullOrEmpty(request.TicketName))
            {
                sql.Append(" AND TicketName ILIKE @TicketName");
            }

            if (request.Price.HasValue)
            {
                sql.Append(" AND Price <= @Price");
            }

            if (request.MinEventDate.HasValue)
            {
                sql.Append(" AND EventDate >= @MinEventDate");
            }

            if (request.MaxEventDate.HasValue)
            {
                sql.Append(" AND EventDate <= @MaxEventDate");
            }

            string orderByColumn = "TicketCode";
            string direction = "ASC";

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                orderByColumn = request.OrderBy.ToLower() switch
                {
                    "ticketname" => "TicketName",
                    "categoryname" => "CategoryName",
                    "price" => "Price",
                    "eventdate" => "EventDate",
                    _ => "TicketCode"
                };
            }

            if (!string.IsNullOrEmpty(request.OrderState) && request.OrderState.ToLower() == "desc")
            {
                direction = "DESC";
            }

            sql.Append($" ORDER BY {orderByColumn} {direction}");

            var parameters = new
            {
                CategoryName = string.IsNullOrEmpty(request.CategoryName) ? null : $"%{request.CategoryName}%",
                TicketCode = string.IsNullOrEmpty(request.TicketCode) ? null : $"%{request.TicketCode}%",
                TicketName = string.IsNullOrEmpty(request.TicketName) ? null : $"%{request.TicketName}%",
                Price = request.Price,
                MinEventDate = request.MinEventDate,
                MaxEventDate = request.MaxEventDate
            };

            return await connection.QueryAsync<Ticket>(sql.ToString(), parameters);
        }

        public async Task<List<BookedTicket>> BookTicketsAsync(List<BookTicketRequest> tickets)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var bookedTicketsResponse = new List<BookedTicket>();
                string transactionId = Guid.NewGuid().ToString();

                foreach (var ticketReq in tickets)
                {
                    var sqlCheck = @"SELECT * FROM Tickets WHERE TicketCode = @TicketCode LIMIT 1";

                    var ticketDb = await connection.QueryFirstOrDefaultAsync<Ticket>(sqlCheck, new { ticketReq.TicketCode }, transaction);

                    if (ticketDb == null)
                    {
                        throw new Exception($"Ticket with code '{ticketReq.TicketCode}' not found.");
                    }

                    if (ticketDb.Quota < ticketReq.Quantity)
                    {
                        throw new Exception($"Quota for '{ticketDb.TicketName}' is not enough. Available: {ticketDb.Quota}, Requested: {ticketReq.Quantity}");
                    }

                    if (ticketDb.EventDate < DateTime.Now)
                    {
                        throw new Exception($"Event '{ticketDb.TicketName}' has already ended.");
                    }

                    var sqlUpdate = "UPDATE Tickets SET Quota = Quota - @Quantity WHERE TicketCode = @TicketCode";
                    await connection.ExecuteAsync(sqlUpdate, new { Quantity = ticketReq.Quantity, TicketCode = ticketReq.TicketCode }, transaction);

                    var sqlInsert = @"INSERT INTO BookedTickets (BookedTicketId, TicketCode, Quantity) 
                                      VALUES (@BookedTicketId, @TicketCode, @Quantity)
                                      RETURNING Id, BookedTicketId, TicketCode, Quantity;";

                    var newBooking = await connection.QuerySingleAsync<BookedTicket>(sqlInsert, new
                    {
                        BookedTicketId = transactionId,
                        TicketCode = ticketReq.TicketCode,
                        Quantity = ticketReq.Quantity
                    }, transaction);

                    bookedTicketsResponse.Add(newBooking);
                }

                await transaction.CommitAsync();

                return bookedTicketsResponse;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<BookedTicket>> GetBookedTicketsAsync(string bookedTicketId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);

            var sql = "SELECT * FROM BookedTickets WHERE BookedTicketId = @bookedTicketId";

            return await connection.QueryAsync<BookedTicket>(sql, new { bookedTicketId });
        }

        public async Task RevokeTicketAsync(string bookedTicketId, string ticketCode, int quantity)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sqlDelete = @"DELETE FROM BookedTickets 
                          WHERE BookedTicketId = @bookedTicketId AND TicketCode = @ticketCode";

                var rowsAffected = await connection.ExecuteAsync(sqlDelete, new { bookedTicketId, ticketCode }, transaction);

                if (rowsAffected == 0)
                {
                    throw new Exception("Booking not found or already deleted.");
                }

                var sqlRefund = @"UPDATE Tickets SET Quota = Quota + @quantity 
                          WHERE TicketCode = @ticketCode";

                await connection.ExecuteAsync(sqlRefund, new { quantity, ticketCode }, transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task EditBookedTicketAsync(string bookedTicketId, string ticketCode, int newQuantity)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sqlGetOld = @"SELECT Quantity FROM BookedTickets 
                                  WHERE BookedTicketId = @bookedTicketId AND TicketCode = @ticketCode";

                var oldQuantity = await connection.QueryFirstOrDefaultAsync<int?>(sqlGetOld, new { bookedTicketId, ticketCode }, transaction);

                if (oldQuantity == null)
                {
                    throw new Exception("Booking not found.");
                }

                int difference = newQuantity - oldQuantity.Value;

                if (difference != 0)
                {
                    var sqlUpdateQuota = @"UPDATE Tickets SET Quota = Quota - @difference 
                                           WHERE TicketCode = @ticketCode AND Quota >= @difference";

                    var rowsAffected = await connection.ExecuteAsync(sqlUpdateQuota, new { difference, ticketCode }, transaction);

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Not enough quota to update ticket. Need {difference} more.");
                    }
                }

                var sqlUpdateBooking = @"UPDATE BookedTickets SET Quantity = @newQuantity 
                                         WHERE BookedTicketId = @bookedTicketId AND TicketCode = @ticketCode";

                await connection.ExecuteAsync(sqlUpdateBooking, new { newQuantity, bookedTicketId, ticketCode }, transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

using MediatR;
using Neighborhood.Services.Application.Transactions.DTOs;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Transactions.Queries.GetTransactionByType
{
    public class GetTransactionByTypeQuery : IRequest<IEnumerable<TransactionResponseDto>>
    {
        public TransactionType Type { get; set; }
    }
}
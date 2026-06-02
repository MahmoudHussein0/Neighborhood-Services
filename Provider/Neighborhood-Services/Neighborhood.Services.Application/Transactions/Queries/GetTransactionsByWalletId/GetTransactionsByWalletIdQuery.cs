using MediatR;
using Neighborhood.Services.Application.Transactions.DTOs;
namespace Neighborhood.Services.Application.Transactions.Queries.GetTransactionsByWalletId
{
    public class GetTransactionsByWalletIdQuery : IRequest<IEnumerable<TransactionResponseDto>>
    {
        public int WalletId { get; set; }
    }
}
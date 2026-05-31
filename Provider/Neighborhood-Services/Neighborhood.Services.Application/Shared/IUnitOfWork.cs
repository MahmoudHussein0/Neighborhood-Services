namespace Neighborhood.Services.Application.Shared
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    }
}

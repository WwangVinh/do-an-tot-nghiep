using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OtoContext _context;

        public UnitOfWork(OtoContext context)
        {
            _context = context;
        }

        public async Task RunInTransactionAsync(Func<Task> action)
        {
            await using IDbContextTransaction tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}


using System;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    // Unit of Work chỉ để chạy transaction xuyên nhiều repository.
    // LogicBusiness không phụ thuộc EF/DbContext; implementation nằm ở layer SqlServer.
    public interface IUnitOfWork
    {
        Task RunInTransactionAsync(Func<Task> action);
    }
}


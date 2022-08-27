using Ordering.Domain.Entities;

namespace Ordering.Application.Contracts.Persistences;

public interface IOrderRepository : IAsyncRepository<Order>
{
    Task<IEnumerable<Order>> GetOrderByUserName(string userName);
}
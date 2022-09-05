using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistences;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Order>> GetOrderByUserName(string userName)
    {
        var orderList = await _dbContext.Orders.Where(x => x.UserName == userName).ToListAsync();
        return orderList;
    }
}

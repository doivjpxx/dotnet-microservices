using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistences;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private IOrderRepository _orderRepository;
    private IMapper _mapper;
    private ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(IOrderRepository repository, IMapper mapper,
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _orderRepository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var existOrder = await _orderRepository.GetByIdAsync(request.Id);

        if (existOrder == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        _mapper.Map(request, existOrder, typeof(UpdateOrderCommand), typeof(Order));

        await _orderRepository.UpdateAsync(existOrder);

        _logger.LogInformation($"Order {existOrder.Id} is updated successfully!");
        
        return Unit.Value;
    }
}
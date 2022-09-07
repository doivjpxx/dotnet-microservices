using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly ILogger<BasketController> _logger;
    private readonly DiscountGrpcService _discountGrpcService;
    private readonly IMapper _mapper;
    private IPublishEndpoint _publishEndpoint;

    public BasketController(IBasketRepository repository, ILogger<BasketController> logger,
        DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    // [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await _repository.GetBasket(userName);
        return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
        }

        return Ok(await _repository.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> DeleteBasket(string userName)
    {
        await _repository.DeleteBasket(userName);
        return Ok();
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        // get existing basket with total price
        var basket = await _repository.GetBasket(basketCheckout.UserName);
        if (basket == null)
        {
            return BadRequest();
        }
        
        // map to event
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        
        // send checkout event to rabbitmq
        await _publishEndpoint.Publish(eventMessage);
        
        // remove the basket
        await _repository.DeleteBasket(basket.UserName);

        return Accepted();
    }
}
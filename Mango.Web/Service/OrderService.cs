using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        private string orderAPI = "/api/order";

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIBase + orderAPI + "/CreateOrder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = SD.OrderAPIBase + orderAPI + "/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Data = userID,
                Url = SD.OrderAPIBase + orderAPI + "/GetOrders"
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Data = orderID,
                Url = SD.OrderAPIBase + orderAPI + "/GetOrder/" + orderID
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderID, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPIBase + orderAPI + "/UpdateOrderStatus/" + orderID
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderID,
                Url = SD.OrderAPIBase + orderAPI + "/ValidateStripeSession"
            });
        }
    }
}

using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderID);
        Task<ResponseDto?> GetAllOrder(string? userID);
        Task<ResponseDto?> GetOrder(int orderID);
        Task<ResponseDto?> UpdateOrderStatus(int orderID, string newStatus);


    }
}

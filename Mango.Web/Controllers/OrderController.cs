using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> OrderIndex()
        {
            return View();
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderID)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderID,SD.Status_ReadyForPickup);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order status updated";
                return RedirectToAction(nameof(OrderDetails), new {OrderID = orderID});
            }

            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderID)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderID, SD.Status_Cancelled);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order status updated";
                return RedirectToAction(nameof(OrderDetails), new { OrderID = orderID });
            }

            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderID)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderID, SD.Status_Completed);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order status updated";
                return RedirectToAction(nameof(OrderDetails), new { OrderID = orderID });
            }

            return View();
        }

        public async Task<IActionResult> OrderDetails(int orderID)
		{
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            string userID = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderID);

            if (response != null && response.IsSuccess)
            {
				orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
			}

			if (!User.IsInRole(SD.RoleAdmin) && userID != orderHeaderDto.UserID)
			{
                return NotFound();
			}

			return View(orderHeaderDto);
		}

		[HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> orderList;
            string userID = "";
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userID = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }

            ResponseDto response = _orderService.GetAllOrder(userID).GetAwaiter().GetResult();

            if (response != null && response.IsSuccess)
            {
                orderList = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        orderList = orderList.Where(u => u.Status == SD.Status_Approved);
                        break;
                    case "cancelled":
						orderList = orderList.Where(u => u.Status == SD.Status_Cancelled);
						break;
					case "readyforpickup":
						orderList = orderList.Where(u => u.Status == SD.Status_ReadyForPickup);
						break;
                    default:
                        break;
				}
            }
            else
            {
                orderList = new List<OrderHeaderDto>();
            }

            return Json(new {data = orderList});

        }
    }
}

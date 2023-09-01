using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderDetailsDto
    {
        public int OrderDetailsID { get; set; }
        public int OrderHeaderID { get; set; }
        public int ProductID { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

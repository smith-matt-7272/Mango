
namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderID { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public string? Email { get; set;}
        public string? Phone { get; set;}
    }
}


using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CartHeaderDto
    {
        public int CartHeaderID { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        [Required(ErrorMessage = "Please enter a first name...")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Please enter a last name...")]
        public string? LastName { get; set;}
        [Required(ErrorMessage = "Please enter an email address...")]
        public string? Email { get; set;}
        [Required(ErrorMessage = "Please enter a phone number...")]
        public string? Phone { get; set;}
    }
}

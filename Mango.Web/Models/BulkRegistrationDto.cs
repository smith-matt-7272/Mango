using Mango.Web.Utility;

namespace Mango.Web.Models
{
    public class BulkRegistrationDto
    {
        [AllowedExtensions (new string[]{".csv"})]
        public IFormFile LoadFile { get; set; }
    }
}

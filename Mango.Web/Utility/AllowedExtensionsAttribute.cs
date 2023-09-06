using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _allowedExtensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_allowedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult("This image extensions is not allowed. Image should be jpg, jpeg, or png.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IProductService
	{
		Task<ResponseDto?> GetAllProductsAsync();
		Task<ResponseDto?> GetProductByIdAsync(int id);
		Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
		Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
		Task<ResponseDto?> DeleteProductsAsync(int id);
	}
}

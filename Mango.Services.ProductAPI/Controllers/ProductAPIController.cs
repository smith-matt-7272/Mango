using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Mango.Services.ProductAPI.Controllers
{
	[Route("api/productAPI")]
	[ApiController]
	public class ProductAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private IMapper _mapper;

		public ProductAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
			_response = new ResponseDto();
		}

		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				// We have a productDTO, we should not be returning a product
				IEnumerable<Product> objList = _db.Products.ToList();
				_response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				// We have a productDTO, we should not be returning a product
				Product obj = _db.Products.First(u => u.ProductID == id);
				// Should be converting to the productDto
				// However, this is ugly and tedious...we are using the
				// automapper package to help with this
				//productDto dto = new productDto()
				//{
				//    productID = obj.productID,
				//    productCode = obj.productCode,
				//    DiscountAmount = obj.DiscountAmount,
				//    MinAmount = obj.MinAmount,
				//};

				// This auto converts the object into a productDTO
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}
		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post(ProductDto productDto)
		{
			try
			{
				Product product = _mapper.Map<Product>(productDto);

				_db.Products.Add(product);
				_db.SaveChanges();

				if(productDto.Image != null)
				{
					// Rename the file to be the product ID, but we also want to append the existing
					// file extension from the dto
					string fileName = product.ProductID + Path.GetExtension(productDto.Image.FileName);
					string filePath = @"wwwroot/ProductImages/" + fileName;
					var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
					using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
					{
						productDto.Image.CopyTo(fileStream);
					}

					var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
					product.ImageUrl = baseURL + "/ProductImages/" + fileName;
					product.ImageLocalPath = filePath;
				}
				else
				{
					product.ImageUrl = "https://placehold.co/600x400";
				}

				_db.Products.Update(product);
				_db.SaveChanges();

				_response.Result = productDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Put(ProductDto productDto)
		{
			try
			{
				Product product = _mapper.Map<Product>(productDto);

                if (productDto.Image != null)
                {
					// Delete the existing image
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        // Get the file path
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);

                        // Get the file
                        FileInfo file = new FileInfo(oldFilePathDirectory);

                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // Rename the file to be the product ID, but we also want to append the existing
                    // file extension from the dto
                    string fileName = product.ProductID + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot/ProductImages/" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseURL + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }

				_db.Products.Update(product);
               _db.SaveChanges();

				_response.Result = productDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Delete(int id)
		{
			try
			{
				Product obj = _db.Products.First(u => u.ProductID == id);

				// If a picture is stored, we want to delete it as well
				if(!string.IsNullOrEmpty(obj.ImageLocalPath))
				{
					// Get the file path
					var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);

					// Get the file
					FileInfo file = new FileInfo(oldFilePathDirectory);

					if(file.Exists)
					{
						file.Delete();
					}
				}

				_db.Products.Remove(obj);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

	}
}

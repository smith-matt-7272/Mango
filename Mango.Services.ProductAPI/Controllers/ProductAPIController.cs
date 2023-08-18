using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		public ResponseDto Post([FromBody] ProductDto productDto)
		{
			try
			{
				_db.Products.Add(_mapper.Map<Product>(productDto));
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
		public ResponseDto Put([FromBody] ProductDto productDto)
		{
			try
			{
				_db.Products.Update(_mapper.Map<Product>(productDto));
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

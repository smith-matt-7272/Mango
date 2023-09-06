using AutoMapper;
using Mango.MessageBus;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Utility;
using Mango.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Mango.Services.AuthAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthAPIController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IMessageBus _messageBus;
		private readonly IMapper _mapper;
		private IConfiguration _configuration;
        private readonly AppDbContext _db;
        protected ResponseDto _response;

		public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration, AppDbContext db, IMapper mapper)
		{
			_authService = authService;
			_messageBus = messageBus;
			_configuration = configuration;
			_mapper = mapper;
			_db = db;
            _response = new();
		}

        [HttpGet]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Get()
        {
            try
            {
                // We have a productDTO, we should not be returning a product
                IEnumerable<ApplicationUser> appUsers = _db.ApplicationUsers.ToList();
                _response.Result = _mapper.Map<IEnumerable<UserDto>>(appUsers);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
		{
			var errorMessage = await _authService.Register(model);

			if (!string.IsNullOrEmpty(errorMessage))
			{
				_response.IsSuccess = false;
				_response.Message = errorMessage;
				return BadRequest(_response);
			}

			await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

			return Ok(_response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
		{
			var loginResponse = await _authService.Login(model);

			if (loginResponse.User == null)
			{
				_response.IsSuccess = false;
				_response.Message = "Username or password is incorrect";
				return BadRequest(_response);
			}

			_response.Result = loginResponse;
			return Ok(_response);
		}

		[HttpPost("bulkregister")]
		public async Task<IActionResult> BulkRegister(BulkRegistrationDto bulkRegistrationDto)
		{
			int newRegistrations = 0;

			try
			{
				if(bulkRegistrationDto.LoadFile != null)
				{
					string[] headers = new string[] { "name", "phonenumber", "email" };
					List<string[]> registrationRecords = FileUtilities.ProcessBulkRegistrationFile(bulkRegistrationDto.LoadFile, headers);

					foreach (string[] regRecord in registrationRecords)
					{
						RegistrationRequestDto registrationRequestDto = new()
						{
							Name = regRecord[0],
							PhoneNumber = regRecord[1],
							Email = regRecord[2],
							Password = "Password123!",
							Role = SD.RoleCustomer
						};

						var errorMessage = await _authService.Register(registrationRequestDto);
						if (string.IsNullOrEmpty(errorMessage))
						{
							newRegistrations++;
						}
					}
				}

				_response.IsSuccess = true;
				_response.Message = string.Format("{0} new registrations created.", newRegistrations);
			} catch
			{
				_response.IsSuccess = false;
				_response.Message = "Bad file";
				return BadRequest(_response);
			}


			return Ok(_response);
		}

		[HttpPost("assignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
		{
			var assignedRoleSuccessful = await _authService.AssignRole(model.Email,model.Role.ToUpper());

			if (!assignedRoleSuccessful)
			{
				_response.IsSuccess = false;
				_response.Message = "Username or password is incorrect";
				return BadRequest(_response);
			}

			return Ok(_response);
		}
	}
}

using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
	{
		private readonly IBaseService _baseService;
		private string authApi = "/api/auth";

		public AuthService(IBaseService baseService) 
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = registrationRequestDto,
				Url = SD.AuthAPIBase + authApi + "/assignRole"
			});
		}

        public async Task<ResponseDto?> BulkRegisterAsync(BulkRegistrationDto bulkRegistrationDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = bulkRegistrationDto,
                Url = SD.AuthAPIBase + authApi + "/bulkregister",
				ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> GetAllUsersAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.AuthAPIBase + authApi
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = loginRequestDto,
				Url = SD.AuthAPIBase + authApi + "/login"
			}, withBearer: false);
		}

		public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = registrationRequestDto,
				Url = SD.AuthAPIBase + authApi + "/register"
			}, withBearer: false);
		}
	}
}

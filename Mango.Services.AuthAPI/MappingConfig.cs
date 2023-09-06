using AutoMapper;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserDto, ApplicationUser>()
                    .ForMember(dest => dest.Id, u => u.MapFrom(src => src.ID)).ReverseMap();
            });

            return mappingConfig;
        }
    }
}
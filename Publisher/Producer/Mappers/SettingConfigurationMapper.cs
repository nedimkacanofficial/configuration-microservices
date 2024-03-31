using AutoMapper;
using Producer.Domain;
using Producer.Dtos.Requests;
using Producer.Dtos.Responses;

namespace Producer.Mappers
{
    public class SettingConfigurationMapper : Profile
    {
        public SettingConfigurationMapper()
        {
            CreateMap<SettingConfiguration, SettingConfigurationResponseDto>()
                .ReverseMap();
            CreateMap<SettingConfiguration, SettingConfigurationRequestDto>()
                .ReverseMap();
        }
    }
}

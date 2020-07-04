using api.models;
using AutoMapper;
using service.DTOs.stberry;

namespace web.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Record, HighestRecordDto>();
            
        }
    }
}

using api.models;
using AutoMapper;
using dao.Interfaces;
using service.DTOs.stberry;
using service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace service.Services
{
    public class RecordsService : IRecordsService
    {
        private readonly IRecordsRepo _recordsRepo;
        private readonly IMapper _mapper;
        public RecordsService(
            IRecordsRepo recordsRepo
            , IMapper mapper)
        {
            _recordsRepo = recordsRepo;
            _mapper = mapper;
        }

        public async Task<Record> CreateAsync(Record model)
        {
            return await _recordsRepo.AddRecordAsync(model);
        }

        public async Task<HighestRecordDto> GetHighestRecordByUserIdAsync(string userId)
        {
            var dto = await _recordsRepo.GetHighestRecordByUserIdAsync(userId);
            return _mapper.Map<Record, HighestRecordDto>(dto);
        }
    }
}

using api.models;
using service.DTOs.stberry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace service.Interfaces
{
    public interface IRecordsService
    {
        Task<Record> CreateAsync(Record model);
        Task<HighestRecordDto> GetHighestRecordByUserIdAsync(string userId);
    }
}

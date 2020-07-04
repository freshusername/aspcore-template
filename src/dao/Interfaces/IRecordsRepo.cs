using api.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dao.Interfaces
{
    public interface IRecordsRepo
    {
        Task<Record> AddRecordAsync(Record model);
        Task<Record> GetHighestRecordByUserIdAsync(string userId);
    }
}

using api.models;
using dao.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dao.Repos
{
    public class RecordsRepo : IRecordsRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public RecordsRepo(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Record> AddRecordAsync(Record model)
        {
            await _dbContext.Records.AddAsync(model);
            return model;
        }

        public async Task<Record> GetHighestRecordByUserIdAsync(string userId)
        {
            return await _dbContext.Records
                .Include(u => u.User)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
}

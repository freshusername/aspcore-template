using api.models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace web.Controllers
{
    [ApiVersion("0")]
    [Route("api/v0/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        #region private props
        private readonly IRecordsService _recordsService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RecordsController> _logger;
        private readonly IMapper _mapper;
        #endregion
        #region constructor
        public RecordsController(
            IRecordsService recordsService
            , UserManager<User> userManager
            , ILogger<RecordsController> logger
            , IMapper mapper)
        {
            _recordsService = recordsService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        [HttpGet("getHighestRecordForCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetHighestRecordForCurrentUser()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var record = await _recordsService.GetHighestRecordByUserIdAsync(userId);
            return Ok(record);
        }
    }
}

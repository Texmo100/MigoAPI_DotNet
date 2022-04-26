using Microsoft.AspNetCore.Mvc;
using MigoAPI.Models;
using MigoAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigoAPI.Controllers
{
    [ApiController]
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("2.0")]
    public class UsersControllerV2 : Controller
    {
        private readonly IRepository<User> _userRepo;

        public UsersControllerV2(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Get the users (V2)
        /// </summary>
        /// <returns>An Actionresult with all the Users</returns>
        /// <response code="200">Returns the list of Users</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<User>))]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok(await _userRepo.GetAllMembersAsync());
        }
    }
}

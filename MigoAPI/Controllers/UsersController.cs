using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MigoAPI.Data;
using MigoAPI.Models;
using MigoAPI.Repository.IRepository;

namespace MigoAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsersController : Controller
    {
        private readonly IRepository<User> _userRepo;

        public UsersController(IRepository<User> userRepo)
        {
           _userRepo = userRepo;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<User>))]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok(await _userRepo.GetAllMembersAsync());
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(ModelState);
            }

            if (await _userRepo.MemberExistsAsync(user.UserName))
            {
                ModelState.AddModelError("", "This user already exists");
                return StatusCode(404, ModelState);
            }

            if (!await _userRepo.CreateMemberAsync(user))
            {
                ModelState.AddModelError("", "Something wrong ocurred during the process");
                return StatusCode(500, ModelState);
            }

            return StatusCode(201, user);
        }


    }
}

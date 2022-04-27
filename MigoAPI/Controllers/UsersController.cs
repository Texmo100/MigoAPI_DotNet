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
    [Consumes("application/json")]
    [Produces("application/json", "application/xml")]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class UsersController : Controller
    {
        private readonly IRepository<User> _userRepo;

        public UsersController(IRepository<User> userRepo)
        {
           _userRepo = userRepo;
        }

        [HttpGet(Name = "GetAllUsersAsync")]
        [ProducesResponseType(200, Type = typeof(List<User>))]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok(await _userRepo.GetAllMembersAsync());
        }

        [HttpPost(Name = "CreateUserAsync")]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                ModelState.AddModelError("", "Something wrong occurred during the process");
                return StatusCode(500, ModelState);
            }

            return StatusCode(201, user);
        }

        /// <summary>
        /// Get an User by his/her id
        /// </summary>
        /// <param name="userId">The id of the user you want to get</param>
        /// <returns> An ActionResult type of User</returns>
        /// <response code="200">Returns the requested User</response>
        [HttpGet("{userId:int}", Name = "GetUserAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserAsync(int userId)
        {
            var user = await _userRepo.GetMemberAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        /// <summary>
        /// Update an User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns>An ActionResult with no content</returns>
        /// <remarks> Sample request (this request updates the entire user record)
        ///     PUT /users/userId
        ///     [
        ///         {
        ///             "UserName": "",
        ///             "Password": "",
        ///             "FirstName": "",
        ///             "LastName": "",
        ///             "Age": 0
        ///         }
        ///     ]
        /// </remarks>
        /// <response code="204">Returns no content</response>
        [HttpPut("{userId:int}", Name = "UpdateUserAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] User user)
        {
            if (user == null && userId <= 0)
            {
                return BadRequest(ModelState);
            }

            if (!await _userRepo.MemberExistsAsync(userId))
            {
                return NotFound();
            }

            await _userRepo.UpdateMemberAsync(user);

            return NoContent();
        }

        [HttpDelete("{userId:int}", Name = "DeleteUserAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            if (!await _userRepo.MemberExistsAsync(userId))
            {
                return NotFound();
            }

            var user = await _userRepo.GetMemberAsync(userId);

            if (!await _userRepo.DeleteMemberAsync(user))
            {
                ModelState.AddModelError("", "Something wrong occurred during the process");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

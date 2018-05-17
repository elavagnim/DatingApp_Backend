using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }
        [HttpGet("{Id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user = await _repo.GetUser(Id);

            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }
          //api/user/1 PUT:
          [HttpPut("{Id}")]
          public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto) 
          {
            if(!ModelState.IsValid)
                return BadRequest(ModelState); 
            
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(id);

            if(userFromRepo== null)
                return NotFound($"Could not find user with ID: {id}");

            if(currentUserId != userFromRepo.Id)
                return Unauthorized();
            
            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"Updating user with ID {id} failed on saved");

          }

      
    }
}
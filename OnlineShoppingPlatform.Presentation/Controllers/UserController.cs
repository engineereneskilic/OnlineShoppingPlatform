using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using OnlineShoppingPlatform.Business.Operations.User;
using OnlineShoppingPlatform.Business.Operations.User.Dtos;
using OnlineShoppingPlatform.Business.Types;
using OnlineShoppingPlatform.Presentation.Models.User;
using OnlineShoppingPlatform.Business.DataProtection;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDataProtection _dataProtector;
        public UserController(IUserService userService , IDataProtection dataProtector)
        {
            _userService = userService;
            _dataProtector = dataProtector;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz kullanıcı ID'si."
                });
            }

            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);


        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateUserDto = new UpdateUserDto
            {
                UserId = updateUserRequest.UserId,
                UserName = updateUserRequest.UserName,
                FirstName = updateUserRequest.FirstName,
                LastName = updateUserRequest.LastName,
                Email = updateUserRequest.Email,
                Password = _dataProtector.UnProtect(updateUserRequest.Password),
                BirthDate = updateUserRequest.BirthDate,
                PhoneNumber = updateUserRequest.PhoneNumber
            };

           

            var result = await _userService.UpdateUserAsync(updateUserDto);

            if (result.IsSucceed)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz kullanıcı ID'si."
                });
            }

            var result = await _userService.DeleteUserAsync(id);

            if (result.IsSucceed)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }



    }
}


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

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"Kullanıcılar alınırken bir hata oluştu: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
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

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Kullanıcı bulunamadı."
                    });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"Kullanıcı bilgileri alınırken bir hata oluştu: {ex.Message}"
                });
            }


        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz veri girişi."
                });
            }

            var result = await _userService.UpdateUserAsync(updateUserDto);

            if (result.IsSucceed)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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


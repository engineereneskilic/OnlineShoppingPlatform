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
using OnlineShoppingPlatform.Business.Operations.Product.Dtos;
using OnlineShoppingPlatform.Business.Operations.Product;

namespace OnlineShoppingPlatform.Presentation.Controllers
{
    //[Authorize(Roles = "Admin")]
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
        public async Task<ActionResult<List<ProductInfoDto>>> GetAllUsers()
        {
            var usersList = await  _userService.GetAllUsersAsync();

            var userInfoDtoList = usersList.Select(user => new UserInfoDto
            {
               UserId = user.UserId,
               Email = user.Email,
               UserName = user.UserName,
               FirstName = user.FirstName,
               LastName = user.LastName,
               BirthDate = user.BirthDate,
               PhoneNumber = user.PhoneNumber,
               UserType = user.UserType

            }).ToList();

            return Ok(userInfoDtoList!);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUserById(int id)
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

            var userInfoDto = new UserInfoDto
            {
                UserId = user.UserId,
                Email = user.Email,
                UserName = user.UserName,
                Password = _dataProtector.UnProtect(user.Password),
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType
            };

            return Ok(userInfoDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest updateUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Geçersiz user ID'si."
                });
            }

            var updateUserDto = new UpdateUserDto
            {
                UserId = id,
                UserName = updateUserRequest.UserName,
                FirstName = updateUserRequest.FirstName,
                LastName = updateUserRequest.LastName,
                Email = updateUserRequest.Email,
                Password = updateUserRequest.Password,
                BirthDate = updateUserRequest.BirthDate,
                PhoneNumber = updateUserRequest.PhoneNumber
            };

            if (updateUserDto == null)
            {
                return BadRequest("User data cannot be null.");
            }

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


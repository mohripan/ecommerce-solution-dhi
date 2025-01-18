using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;

namespace UserService.Application.Services
{
    public interface IUserAppService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<IReadOnlyList<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UserRequestDto userDto);
        Task<bool> DeleteUserAsync(int id);
    }
}

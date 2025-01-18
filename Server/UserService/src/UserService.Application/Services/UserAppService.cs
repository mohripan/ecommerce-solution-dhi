using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Factories;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.UnitOfWork;

namespace UserService.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;
        private readonly IUnitOfWork _unitOfWork;

        public UserAppService(
            IUserRepository userRepository,
            IUserFactory userFactory,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            // Use the factory to create a user (this includes domain validations)
            var user = _userFactory.CreateUser(userDto.Username, userDto.Email, userDto.RoleId);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // In a real system, you might publish a domain event here 
            // e.g. UserRegisteredEvent

            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return null;

            // domain method for updating
            existingUser.UpdateUser(userDto.Username, userDto.Email, userDto.RoleId);

            _userRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return false;

            _userRepository.Delete(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Helper method
        private UserDto MapToDto(MstrUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName
            };
        }
    }
}

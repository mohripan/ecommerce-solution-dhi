using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Factories;
using UserService.Infrastructure.Helper;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.UnitOfWork;

namespace UserService.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserAppService(
            IUserRepository userRepository,
            IUserFactory userFactory,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
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
            if (await CheckExistingUserByEmail(userDto.Email))
                throw new GlobalException("The email is already in use.");

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);
            var user = _userFactory.CreateUser(userDto.Username, userDto.Email, passwordHash, userDto.RoleId);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new ArgumentException("User doesn't exist");

            var newPasswordHash = _passwordHasher.HashPassword(userDto.Password);
            existingUser.UpdateUser(userDto.Username, userDto.Email, newPasswordHash, userDto.RoleId);

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
                Password = user.Password,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName
            };
        }

        private async Task<bool> CheckExistingUserByEmail(string email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser == null) return false;
            return true;
        }
    }
}

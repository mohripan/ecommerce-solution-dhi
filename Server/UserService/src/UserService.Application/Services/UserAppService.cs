using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;
using UserService.Application.Exceptions;
using UserService.Contracts.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Factories;

namespace UserService.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;
        private readonly IBlacklistService _blacklistService;
        private readonly IJwtService _jwtService;

        public UserAppService(
            IUserRepository userRepository,
            IUserFactory userFactory,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IAuthService authService,
            IBlacklistService blacklistService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _blacklistService = blacklistService;
            _jwtService = jwtService;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<PaginatedResponse<UserResponseDto>> GetAllUsersAsync(int page, int sizePerPage, int? roleId)
        {
            var users = await _userRepository.GetAllAsync(page, sizePerPage, roleId);
            var totalCount = await _userRepository.GetTotalCountAsync(roleId);

            var totalPage = (int)Math.Ceiling((double)totalCount / sizePerPage);

            return new PaginatedResponse<UserResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = users.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto)
        {
            if (await CheckExistingUserByEmail(userDto.Email))
                throw new GlobalException("The email is already in use.");

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);
            var user = _userFactory.CreateUser(userDto.Username, userDto.Email, passwordHash, userDto.RoleId);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var savedUser = await _userRepository.GetByIdAsync(user.Id);
            return MapToResponseDto(savedUser);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UserRequestDto userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new GlobalException("User doesn't exist.");

            var newPasswordHash = _passwordHasher.HashPassword(userDto.Password);
            existingUser.UpdateUser(userDto.Username, userDto.Email, newPasswordHash, userDto.RoleId);

            _userRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDto(existingUser);
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

        public async Task<(bool IsAuthenticated, string Token, DateTime Expiration)> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            var isValidUser = await _authService.ValidateUserAsync(email, password);
            Console.WriteLine("This is user: " + user);
            if (!isValidUser || user == null)
                return (false, string.Empty, DateTime.MinValue);

            var token = await _authService.GenerateJwtTokenAsync(user.Id);
            var expiration = DateTime.UtcNow.AddHours(1);

            return (true, token, expiration);
        }

        public async Task LogoutAsync(string token)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _authService.ValidateToken(token, validateLifetime: false);
            }
            catch
            {
                return;
            }

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            user.RefreshSecurityStamp();
            await _unitOfWork.SaveChangesAsync();
        }

        //---------------------------------------------------------------------------------
        // Helper
        private UserResponseDto MapToResponseDto(MstrUser user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName
            };
        }

        private async Task<bool> CheckExistingUserByEmail(string email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            return existingUser != null;
        }
    }
}

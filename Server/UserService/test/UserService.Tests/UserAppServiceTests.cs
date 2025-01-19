using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Requests;
using UserService.Application.Exceptions;
using UserService.Application.Services;
using UserService.Contracts.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Factories;

namespace UserService.Tests
{
    [TestClass]
    public class UserAppServiceTests
    {
        private IUserAppService _service;
        private FakeUserRepository _userRepo;
        private FakeUserFactory _userFactory;
        private FakeUnitOfWork _unitOfWork;
        private FakePasswordHasher _passwordHasher;
        private FakeAuthService _authService;
        private FakeBlacklistService _blacklistService;
        private FakeJwtService _jwtService;

        [TestInitialize]
        public void Setup()
        {
            _userRepo = new FakeUserRepository();
            _userFactory = new FakeUserFactory();
            _unitOfWork = new FakeUnitOfWork();
            _passwordHasher = new FakePasswordHasher();
            _authService = new FakeAuthService();
            _blacklistService = new FakeBlacklistService();
            _jwtService = new FakeJwtService();

            _service = new UserAppService(
                _userRepo,
                _userFactory,
                _unitOfWork,
                _passwordHasher,
                _authService,
                _blacklistService,
                _jwtService
            );
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldCreateNewUser_WhenEmailNotExist()
        {
            // Arrange
            var userDto = new UserRequestDto
            {
                Username = "John",
                Email = "john@example.com",
                Password = "secret",
                RoleId = 1
            };

            // Act
            var result = await _service.CreateUserAsync(userDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.Username);
            Assert.AreEqual("john@example.com", result.Email);
            Assert.IsTrue(_userRepo.Users.Any(u => u.Email == "john@example.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(GlobalException))]
        public async Task CreateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new MstrUser("OldUser", "dup@example.com", "hashed!", 1);
            _userRepo.Users.Add(existingUser);

            var userDto = new UserRequestDto
            {
                Username = "Anyone",
                Email = "dup@example.com",
                Password = "secret",
                RoleId = 1
            };

            // Act
            // Should throw GlobalException
            await _service.CreateUserAsync(userDto);
        }

        [TestMethod]
        public async Task AuthenticateUserAsync_ShouldReturnValidToken_WhenCredentialsAreCorrect()
        {
            // Arrange
            var user = new MstrUser("Alice", "alice@example.com", "hashed!", 2);
            _userRepo.Users.Add(user);
            // FakeAuthService always returns "FakeToken" if password is "correct"
            _authService.ValidUserEmail = "alice@example.com";
            _authService.ValidUserPassword = "correct";

            // Act
            var (isAuthenticated, token, expiration) = await _service.AuthenticateUserAsync("alice@example.com", "correct");

            // Assert
            Assert.IsTrue(isAuthenticated);
            Assert.AreEqual("FakeToken", token);
            Assert.IsTrue(expiration > DateTime.UtcNow);
        }

        [TestMethod]
        public async Task AuthenticateUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            _authService.ValidUserEmail = "doesnotexist@example.com";

            // Act
            var (isAuthenticated, token, _) = await _service.AuthenticateUserAsync("doesnotexist@example.com", "whatever");

            // Assert
            Assert.IsFalse(isAuthenticated);
            Assert.AreEqual(string.Empty, token);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = new MstrUser("Bob", "bob@example.com", "hashed!", 1) { Id = 123 };
            _userRepo.Users.Add(user);

            // Act
            var result = await _service.GetUserByIdAsync(123);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Bob", result.Username);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var user = new MstrUser("Initial", "init@example.com", "hashed!", 1) { Id = 50 };
            _userRepo.Users.Add(user);

            var newDto = new UserRequestDto
            {
                Username = "Updated",
                Email = "updated@example.com",
                Password = "newpass",
                RoleId = 2
            };

            // Act
            var updated = await _service.UpdateUserAsync(50, newDto);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated.Username);
            Assert.AreEqual("updated@example.com", updated.Email);
            Assert.AreEqual(2, updated.RoleId);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldThrow_WhenUserNotExist()
        {
            // Arrange
            var newDto = new UserRequestDto
            {
                Username = "NoOne",
                Email = "noone@example.com",
                Password = "nope",
                RoleId = 1
            };

            try
            {
                // Act
                await _service.UpdateUserAsync(999, newDto);
                Assert.Fail("Expected GlobalException for non-existing user");
            }
            catch (GlobalException ex)
            {
                // Assert
                Assert.AreEqual("User doesn't exist.", ex.Message);
            }
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = new MstrUser("Temp", "temp@example.com", "hashed!", 1) { Id = 1 };
            _userRepo.Users.Add(user);

            // Act
            var result = await _service.DeleteUserAsync(1);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_userRepo.Users.Any(u => u.Id == 1));
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotExists()
        {
            // Act
            var result = await _service.DeleteUserAsync(999);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LogoutAsync_ShouldRefreshSecurityStamp()
        {
            // Arrange
            var user = new MstrUser("LogoutUser", "logout@example.com", "hashed", 1) { Id = 2 };
            _userRepo.Users.Add(user);
            var oldStamp = user.SecurityStamp;

            _authService.ValidateTokenUserId = 2;

            // Act
            await _service.LogoutAsync("LogoutToken");

            // Assert
            var updatedUser = _userRepo.Users.First(u => u.Id == 2);
            Assert.AreNotEqual(oldStamp, updatedUser.SecurityStamp);
        }
    }

    public class FakeUserRepository : IUserRepository
    {
        public List<MstrUser> Users { get; set; } = new List<MstrUser>();

        public Task AddAsync(MstrUser user)
        {
            if (user.Id == 0)
            {
                user.GetType().GetProperty(nameof(MstrUser.Id))!
                    .SetValue(user, Users.Count + 100);
            }
            Users.Add(user);
            return Task.CompletedTask;
        }

        public void Delete(MstrUser user)
        {
            Users.Remove(user);
        }

        public async Task<IReadOnlyList<MstrUser>> GetAllAsync(int page, int sizePerPage, int? roleId)
        {
            var q = Users.AsQueryable();
            if (roleId.HasValue) q = q.Where(u => u.RoleId == roleId.Value);
            return q.Skip((page - 1) * sizePerPage).Take(sizePerPage).ToList();
        }

        public async Task<MstrUser?> GetByEmailAsync(string email)
        {
            return Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<MstrUser?> GetByIdAsync(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<int> GetTotalCountAsync(int? roleId)
        {
            if (roleId == null) return Users.Count;
            return Users.Count(u => u.RoleId == roleId.Value);
        }

        public void Update(MstrUser user)
        {

        }
    }

    public class FakeUserFactory : IUserFactory
    {
        public MstrUser CreateUser(string username, string email, string password, int roleId)
        {
            return new MstrUser(username, email, password, roleId);
        }
    }

    public class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }

    public class FakePasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return "FAKE_HASH_" + password;
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            return hashedPassword.EndsWith(inputPassword);
        }
    }

    public class FakeAuthService : IAuthService
    {
        public string ValidUserEmail { get; set; } = "none";
        public string ValidUserPassword { get; set; } = "none";

        // For ValidateToken
        public int ValidateTokenUserId { get; set; } = -1;

        public Task<bool> ValidateUserAsync(string email, string password)
        {
            var valid = (email == ValidUserEmail && password == ValidUserPassword);
            return Task.FromResult(valid);
        }

        public Task<string> GenerateJwtTokenAsync(int userId)
        {
            return Task.FromResult("FakeToken");
        }

        public ClaimsPrincipal ValidateToken(string tokenString, bool validateLifetime = false)
        {
            // If tokenString=="LogoutToken", parse out userId=ValidateTokenUserId
            if (tokenString == "LogoutToken" && ValidateTokenUserId > 0)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, ValidateTokenUserId.ToString()));
                return new ClaimsPrincipal(identity);
            }

            throw new Exception("Invalid token");
        }
    }

    public class FakeBlacklistService : IBlacklistService
    {
        private readonly HashSet<string> _tokens = new HashSet<string>();

        public void AddToBlacklist(string token)
        {
            _tokens.Add(token);
        }

        public bool IsBlacklisted(string token)
        {
            return _tokens.Contains(token);
        }
    }

    public class FakeJwtService : IJwtService
    {
        public int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("No user ID claim");
            return int.Parse(claim.Value);
        }

        public string ExtractToken(string authorizationHeader)
        {
            return authorizationHeader;
        }
    }
}

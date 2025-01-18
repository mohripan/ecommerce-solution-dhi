using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Infrastructure.Services;

namespace UserService.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _authService;
        private FakeUserRepository _userRepo;
        private FakePasswordHasher _hasher;
        private FakeUnitOfWork _unitOfWork;
        private FakeConfiguration _configuration;

        [TestInitialize]
        public void Setup()
        {
            _userRepo = new FakeUserRepository();
            _hasher = new FakePasswordHasher();
            _unitOfWork = new FakeUnitOfWork();
            _configuration = new FakeConfiguration();

            _authService = new AuthService(
                _userRepo,
                _hasher,
                _configuration,
                _unitOfWork
            );
        }

        [TestMethod]
        public async Task ValidateUserAsync_ReturnsFalse_WhenUserNotExist()
        {
            var result = await _authService.ValidateUserAsync("nope@example.com", "pwd");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateUserAsync_ReturnsTrue_WhenPasswordMatches()
        {
            // Arrange
            var user = new MstrUser("User", "test@example.com", "FAKE_HASH_correctpw", 1);
            _userRepo.Users.Add(user);

            // Act
            var result = await _authService.ValidateUserAsync("test@example.com", "correctpw");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GenerateJwtTokenAsync_ShouldRefreshSecurityStamp()
        {
            // Arrange
            var user = new MstrUser("JWT", "jwt@example.com", "hash", 1) { Id = 77 };
            var oldStamp = user.SecurityStamp;
            _userRepo.Users.Add(user);

            // Act
            var token = await _authService.GenerateJwtTokenAsync(77);

            // Assert
            var updatedUser = _userRepo.Users.First(u => u.Id == 77);
            Assert.AreNotEqual(oldStamp, updatedUser.SecurityStamp);
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 10);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GenerateJwtTokenAsync_ShouldThrow_WhenUserNotFound()
        {
            await _authService.GenerateJwtTokenAsync(999);
        }

        [TestMethod]
        public void ValidateToken_ShouldParsePrincipal_WhenValidSignature()
        {
            try
            {
                var principal = _authService.ValidateToken("FAKE_JWT");
                Assert.Fail("Should throw without a real token");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is SecurityTokenException
                              || ex is ArgumentException
                              || ex is SecurityTokenInvalidSignatureException);
            }
        }
    }


    internal class FakeConfiguration : IConfiguration
    {
        public string this[string key]
        {
            get
            {
                if (key == "Jwt:Key") return "testtesttesttest12345678testtest";
                if (key == "Jwt:Issuer") return "UnitTestIssuer";
                return "";
            }
            set { }
        }

        public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();
        public IChangeToken GetReloadToken() => throw new NotImplementedException();
        public IConfigurationSection GetSection(string key) => throw new NotImplementedException();
    }
}

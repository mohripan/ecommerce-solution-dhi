using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Services.Impls;
using UserService.Application.Services.Interfaces;

namespace UserService.Tests
{
    [TestClass]
    public class JwtServiceTests
    {
        private IJwtService _jwtService;

        [TestInitialize]
        public void Setup()
        {
            _jwtService = new JwtService();
        }

        [TestMethod]
        public void ExtractToken_ShouldReturnToken_WhenHeaderIsValid()
        {
            var header = "Bearer abc123";
            var token = _jwtService.ExtractToken(header);
            Assert.AreEqual("abc123", token);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractToken_ShouldThrow_WhenHeaderIsInvalid()
        {
            _jwtService.ExtractToken("SomethingInvalid");
        }

        [TestMethod]
        public void GetUserIdFromToken_ShouldReturnId_WhenClaimExists()
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "42"));
            var principal = new ClaimsPrincipal(identity);

            var userId = _jwtService.GetUserIdFromToken(principal);
            Assert.AreEqual(42, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void GetUserIdFromToken_ShouldThrow_WhenNoClaim()
        {
            var principal = new ClaimsPrincipal();
            _jwtService.GetUserIdFromToken(principal);
        }
    }
}

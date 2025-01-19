using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Services;

namespace UserService.Tests
{
    [TestClass]
    public class BlacklistServiceTests
    {
        [TestMethod]
        public void AddToBlacklistAndIsBlacklisted_ShouldWork()
        {
            var service = new BlacklistService();
            var token = "1921j2ok1jm2ojk1n21i20d11lmxakos";

            Assert.IsFalse(service.IsBlacklisted(token));

            service.AddToBlacklist(token);

            Assert.IsTrue(service.IsBlacklisted(token));
        }
    }
}

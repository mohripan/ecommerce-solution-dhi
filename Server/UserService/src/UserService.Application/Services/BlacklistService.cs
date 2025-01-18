using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Services
{
    public class BlacklistService : IBlacklistService
    {
        private readonly ConcurrentDictionary<string, bool> _blacklistedTokens = new();

        public void AddToBlacklist(string token)
        {
            _blacklistedTokens[token] = true;
        }

        public bool IsBlacklisted(string token)
        {
            return _blacklistedTokens.ContainsKey(token);
        }
    }
}

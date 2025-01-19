using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Application.Exceptions
{
    public class GlobalException : Exception
    {
        public GlobalException(string message) : base(message) { }
    }
}

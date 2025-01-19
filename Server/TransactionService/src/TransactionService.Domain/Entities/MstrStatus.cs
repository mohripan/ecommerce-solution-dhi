using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Domain.Entities
{
    public class MstrStatus
    {
        public int Id { get; private set; }
        public string StatusName { get; private set; } = string.Empty;
        public DateTime CreatedOn { get; private set; }
        public string CreatedBy { get; private set; } = "SYS";

        private MstrStatus() { }

        public MstrStatus(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
                throw new ArgumentException("StatusName cannot be null or empty.", nameof(statusName));

            StatusName = statusName;
            CreatedOn = new DateTime(2025, 1, 1);
            CreatedBy = "SYS";
        }
    }
}

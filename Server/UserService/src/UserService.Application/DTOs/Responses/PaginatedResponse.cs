using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Responses
{
    public class PaginatedResponse<T>
    {
        public PaginationMetadata Search { get; set; } = default!;
        public IReadOnlyList<T> Values { get; set; } = new List<T>();
    }

    public class PaginationMetadata
    {
        public int Total { get; set; }
        public int TotalPage { get; set; }
        public int SizePerPage { get; set; }
        public int PageAt { get; set; }
    }
}

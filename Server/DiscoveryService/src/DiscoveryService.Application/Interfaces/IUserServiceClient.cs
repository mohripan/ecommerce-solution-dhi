using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<(HttpStatusCode StatusCode, string Content)> GetAllUsersAsync(int page, int sizePerPage, int? roleId);
        Task<(HttpStatusCode StatusCode, string Content)> GetUserByIdAsync(int id);
        Task<(HttpStatusCode StatusCode, string Content)> LoginAsync(object loginRequestDto);
        Task<(HttpStatusCode StatusCode, string Content)> CreateUserAsync(object userCreateDto);
        Task<(HttpStatusCode StatusCode, string Content)> UpdateUserAsync(object userUpdateDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> DeleteUserAsync(string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> LogoutUserAsync(string authorizationHeader);
    }
}

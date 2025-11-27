using Application.DTO.Common;
using Application.DTO.Users;

namespace Application.Interfaces.Users;

public interface IUserService
{
    Task<UserDetailDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserSummaryDto>> GetAllAsync();
    Task UpdateNameAsync(Guid userId, NameDto name);
    Task UpdateEmailAsync(Guid userId, string email);
    Task UpdatePhoneAsync(Guid userId, string phone);
    Task DisableAsync(Guid userId);
}

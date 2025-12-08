using Application.DTO.Common;
using Application.DTO.Users;
using Application.Interfaces.Users;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public UserService(IUserRepository userRepo,IUnitOfWork uow)
    {
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<UserDetailDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepo.GetById(id);
        if (user is null)
            return null;

        return ToDetailDto(user);
    }

    public async Task<IEnumerable<UserSummaryDto>> GetAllAsync()
    {
        var users = await _userRepo.GetAllAsync();

        return users.Select(u => new UserSummaryDto(
            u.Id,
            new NameDto(u.Name.FirstName, u.Name.LastName),
            u.Email.Address,
            u.Role,
            u.IsActive,
            u.CreatedAt
        ));
    }

    public async Task UpdateNameAsync(Guid userId, NameDto nameDto)
    {
        var user = await _userRepo.GetById(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        var name = new Name(nameDto.FirstName, nameDto.LastName);

        user.ChangeName(name);

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }

    public async Task UpdateEmailAsync(Guid userId, string email)
    {
        var user = await _userRepo.GetById(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        var existing = await _userRepo.GetByEmailAsync(email);
        if (existing is not null && existing.Id != userId)
            throw new InvalidOperationException("Este e-mail já está em uso.");

        var emailVo = new Email(email);
        user.ChangeEmail(emailVo);

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }

    public async Task UpdatePhoneAsync(Guid userId, string phone)
    {
        var user = await _userRepo.GetById(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        user.ChangePhone(phone);

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }

    public async Task DisableAsync(Guid userId)
    {
        var user = await _userRepo.GetById(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        user.DisableAccount();

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }


    private static UserDetailDto ToDetailDto(User user)
    {
        var nameDto = new NameDto(user.Name.FirstName, user.Name.LastName);

        ProfessionalProfileDto? profProfileDto = null;

        return new UserDetailDto(
            user.Id,
            nameDto,
            user.Email.Address,
            user.Role,
            user.IsActive,
            user.CreatedAt,
            profProfileDto
        );
    }
}

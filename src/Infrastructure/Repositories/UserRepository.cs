using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(NeuroFlowContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.Address == email);
    }
    public async Task<User?> GetById(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(i=> i.Id == id);
    }
}

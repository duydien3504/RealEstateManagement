using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entities;
using RealEstateSystem.Infrastructure.Data;

namespace RealEstateSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RealEstateDbContext _context;

        public UserRepository(RealEstateDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email) 
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}

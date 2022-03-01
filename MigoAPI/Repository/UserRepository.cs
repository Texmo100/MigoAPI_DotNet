using Microsoft.EntityFrameworkCore;
using MigoAPI.Data;
using MigoAPI.Models;
using MigoAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MigoAPI.Repository
{
    public class UserRepository : IRepository<User>
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateMemberAsync(User member)
        {
            _context.Users.Add(member);
            return await SaveSync();
        }

        public async Task<bool> DeleteMemberAsync(User member)
        {
            _context.Users.Remove(member);
            return await SaveSync();
        }

        public async Task<ICollection<User>> GetAllMembersAsync()
        {
            return await _context.Users.OrderBy(a => a.UserName).ToListAsync();
        }

        public async Task<User> GetMemberAsync(int memberId)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.Id == memberId);
        }

        public async Task<bool> MemberExistsAsync(string name)
        {
            return await _context.Users.AnyAsync(a => a.UserName.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task<bool> MemberExistsAsync(int memberId)
        {
            return await _context.Users.AnyAsync(a => a.Id == memberId);
        }

        public async Task<bool> SaveSync()
        {
            return await _context.SaveChangesAsync() >= 0 ? true : false;
        }

        public async Task<bool> UpdateMemberAsync(User member)
        {
            _context.Users.Update(member);
            return await SaveSync();
        }
    }
}

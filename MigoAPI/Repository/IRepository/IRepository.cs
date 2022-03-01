using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigoAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<ICollection<T>> GetAllMembersAsync();
        Task<T> GetMemberAsync(int memberId);
        Task<bool> MemberExistsAsync(string name);
        Task<bool> MemberExistsAsync(int memberId);
        Task<bool> CreateMemberAsync(T member);
        Task<bool> UpdateMemberAsync(T member);
        Task<bool> DeleteMemberAsync(T member);
        Task<bool> SaveSync();
    }
}
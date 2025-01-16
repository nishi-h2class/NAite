using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        PagedList<User> GetUsers(UserParams param);
        User? GetUser(string id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool CheckDuplicateLoginId(string id, string loginId);
        bool CheckUserId(string userId);
    }
}

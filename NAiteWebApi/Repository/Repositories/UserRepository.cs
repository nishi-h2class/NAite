using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private ISortHelper<User> _sortHelper;

        public UserRepository(NAiteContext context,
            ISortHelper<User> sortHelper)
            : base(context)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<User> GetUsers(UserParams param)
        {
            var users = FindByCondition(a => a.Deleted == null && a.LoginId != NAiteSettings.GetSystemAdminLoginId());

            SearchByName(ref users, param.Keyword);

            var sortedUsers = _sortHelper.ApplySort(users, param.OrderBy);

            return PagedList<User>.ToPagedList(
                sortedUsers.ToList(),
                param.PageNumber,
                param.PageSize
                );
        }

        private void SearchByName(ref IQueryable<User> users, string name)
        {
            if (!users.Any() || string.IsNullOrEmpty(name))
                return;

            users = users.Where(o => o.LoginId!.ToLower().Contains(name.Trim().ToLower()) || o.FirstName!.ToLower().Contains(name.Trim().ToLower()) || o.LastName!.ToLower().Contains(name.Trim().ToLower()));
        }

        public User? GetUser(string id)
        {
            return FindByCondition(a => a.Id == id).FirstOrDefault();
        }

        public void CreateUser(User user)
        {
            Create(user);
        }

        public void UpdateUser(User user)
        {
            Update(user);
        }

        public void DeleteUser(User user)
        {
            user.Deleted = DateTime.Now;
            Update(user);
        }

        public bool CheckDuplicateLoginId(string id, string loginId)
        {
            var users = FindByCondition(a => a.Id != id);

            return users.Any(a => a.LoginId.Equals(loginId) && a.Deleted == null);
        }

        public bool CheckUserId(string userId)
        {
            return NAiteContext.Users.Any(a => a.Id.Equals(userId));
        }
    }
}

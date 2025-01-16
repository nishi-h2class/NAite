using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using NAiteWebApi.Libs;

namespace NAiteWebApi.Repository.Repositories
{
    public class AuthRepository : RepositoryBase<User>, IAuthRepository
    {
        public AuthRepository(NAiteContext context)
            : base(context)
        {
            
        }

        public User? Auth(AuthParams param)
        {
            var user = FindByCondition(
                u => u.LoginId.Equals(param.LoginId) && u.Deleted == null).FirstOrDefault();

            if (user is null)
                return null;
            else
                return user;
        }
    }
}

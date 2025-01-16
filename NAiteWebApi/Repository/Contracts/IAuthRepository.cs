using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IAuthRepository
    {
        User? Auth(AuthParams param);
    }
}

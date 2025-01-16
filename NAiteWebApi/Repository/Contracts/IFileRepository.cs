using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IFileRepository : IRepositoryBase<NAiteEntities.Models.File>
    {
        PagedList<NAiteEntities.Models.File> GetFiles(FileParams param);
        NAiteEntities.Models.File[] GetAllFiles();
        NAiteEntities.Models.File? GetFile(string id);
        NAiteEntities.Models.File? GetFileByName(string name);
        string[] CheckFileName(string[] names);
        void CreateFile(NAiteEntities.Models.File item);
        void UpdateFile(NAiteEntities.Models.File item);
        void DeleteFile(NAiteEntities.Models.File item);
    }
}

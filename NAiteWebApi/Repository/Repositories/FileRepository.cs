using Microsoft.EntityFrameworkCore;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class FileRepository : RepositoryBase<NAiteEntities.Models.File>, IFileRepository
    {
        private ISortHelper<NAiteEntities.Models.File> _sortHelper;

        public FileRepository(NAiteContext context,
            ISortHelper<NAiteEntities.Models.File> sortHelper)
            : base(context)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<NAiteEntities.Models.File> GetFiles(FileParams param)
        {
            var files = FindByCondition(a => a.Deleted == null);

            SearchByName(ref files, param.Keyword);

            var sortedFiles = _sortHelper.ApplySort(files, param.OrderBy);

            return PagedList<NAiteEntities.Models.File>.ToPagedList(
                sortedFiles.ToList(),
                param.PageNumber,
                param.PageSize
                );
        }

        private void SearchByName(ref IQueryable<NAiteEntities.Models.File> files, string name)
        {
            if (!files.Any() || string.IsNullOrEmpty(name))
                return;

            files = files.Where(o => o.Name!.ToLower().Contains(name.Trim().ToLower()));
        }

        public NAiteEntities.Models.File[] GetAllFiles()
        {
            var files = FindByCondition(a => a.Deleted == null).OrderBy(a => a.Name).ToArray();
            return files;
        }

        public NAiteEntities.Models.File? GetFile(string id)
        {
            return FindByCondition(a => a.Id == id).FirstOrDefault();
        }

        public NAiteEntities.Models.File? GetFileByName(string name)
        {
            return FindByCondition(a => a.Deleted == null && a.Name == name).FirstOrDefault();
        }

        public string[] CheckFileName(string[] names)
        {
            var errorFileNames = new List<string>();

            foreach (var name in names)
            {
                var count = FindByCondition(a => a.Deleted == null).Where(a => a.Name == name).Count();
                if (count > 0)
                {
                    errorFileNames.Add(name);
                }
            }

            return errorFileNames.ToArray();
        }

        public void CreateFile(NAiteEntities.Models.File file)
        {
            Create(file);
        }

        public void UpdateFile(NAiteEntities.Models.File file)
        {
            Update(file);
        }

        public void DeleteFile(NAiteEntities.Models.File file)
        {
            Delete(file);
        }
    }
}

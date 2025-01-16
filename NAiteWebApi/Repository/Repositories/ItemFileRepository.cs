using NAiteEntities.Models;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemFileRepository : RepositoryBase<ItemFile>, IItemFileRepository
    {
        public ItemFileRepository(NAiteContext context)
            : base(context)
        {

        }

        public ItemFile[] GetItemFilesByItem(string id)
        {
            var files = FindByCondition(a => a.ItemId == id).OrderBy(a => a.File.Name).ToArray();
            return files;
        }

        public ItemFile[] GetItemFilesByFile(string id)
        {
            var files = FindByCondition(a => a.FileId == id).ToArray();
            return files;
        }

        public void CreateItemFile(ItemFile file)
        {
            Create(file);
        }

        public void DeleteItemFile(ItemFile file)
        {
            Delete(file);
        }
    }
}

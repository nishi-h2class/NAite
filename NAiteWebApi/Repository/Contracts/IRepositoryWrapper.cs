namespace NAiteWebApi.Repository.Contracts
{
    public interface IRepositoryWrapper
    {
        IAuthRepository Auth { get; }
        IUserRepository User { get; }
        IItemFieldRepository ItemField { get; }
        IItemRepository Item { get; }
        IItemRowRepository ItemRow { get; }
        IFileRepository File { get; }
        IItemFileRepository ItemFile { get; }
        IItemDataImportRepository ItemDataImport { get; }
        IItemDataImportFieldRepository ItemDataImportField { get; }
        IItemDataRepository ItemData { get; }

        void Save();
    }
}

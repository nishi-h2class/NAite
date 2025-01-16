using NAiteWebApi.Repository.Contracts;
using NAiteEntities.Models;
using AutoMapper;
using NAiteWebApi.Helpers;
using NAiteWebApi.Libs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NAiteWebApi.Repository.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private NAiteContext _context = null!;

        private IAuthRepository _auth = null!;
        private IUserRepository _user = null!;
        private IItemFieldRepository _itemField = null!;
        private IItemRepository _item = null!;
        private IItemRowRepository _itemRow = null!;
        private IFileRepository _file = null!;
        private IItemFileRepository _itemFile = null!;
        private IItemDataImportRepository _itemDataImport = null!;
        private IItemDataImportFieldRepository _itemDataImportField = null!;
        private IItemDataRepository _itemData = null!;

        private ISortHelper<User> _userSortHelper;
        private ISortHelper<ItemRow> _itemRowSortHelper;
        private ISortHelper<NAiteEntities.Models.File> _fileSortHelper;
        private ISortHelper<ItemDataImport> _itemDataImportSortHelper;
        private IMapper _mapper;

        public RepositoryWrapper(
            NAiteContext context,
            ISortHelper<User> userSortHelper,
            ISortHelper<ItemRow> itemRowSortHelper,
            ISortHelper<NAiteEntities.Models.File> fileSortHelper,
            ISortHelper<ItemDataImport> itemDataImportSortHelper,
            IMapper mapper)
        {
            _context = context;

            _userSortHelper = userSortHelper;
            _itemRowSortHelper = itemRowSortHelper;
            _fileSortHelper = fileSortHelper;
            _itemDataImportSortHelper = itemDataImportSortHelper;

            _mapper = mapper;
        }

        public IAuthRepository Auth
        {
            get
            {
                if (_auth == null)
                    _auth = new AuthRepository(_context);
                return _auth;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_context, _userSortHelper);
                }
                return _user;
            }
        }

        public IItemFieldRepository ItemField
        {
            get
            {
                if (_itemField == null)
                {
                    _itemField = new ItemFieldRepository(_context);
                }
                return _itemField;
            }
        }

        public IItemRepository Item
        {
            get
            {
                if (_item == null)
                {
                    _item = new ItemRepository(_context);
                }
                return _item;
            }
        }

        public IItemRowRepository ItemRow
        {
            get
            {
                if (_itemRow == null)
                {
                    _itemRow = new ItemRowRepository(_context, _itemRowSortHelper);
                }
                return _itemRow;
            }
        }

        public IFileRepository File
        {
            get
            {
                if (_file == null)
                {
                    _file = new FileRepository(_context, _fileSortHelper);
                }
                return _file;
            }
        }

        public IItemFileRepository ItemFile
        {
            get
            {
                if (_itemFile == null)
                {
                    _itemFile = new ItemFileRepository(_context);
                }
                return _itemFile;
            }
        }

        public IItemDataImportRepository ItemDataImport
        {
            get
            {
                if (_itemDataImport == null)
                {
                    _itemDataImport = new ItemDataImportRepository(_context, _itemDataImportSortHelper);
                }
                return _itemDataImport;
            }
        }

        public IItemDataImportFieldRepository ItemDataImportField
        {
            get
            {
                if (_itemDataImportField == null)
                {
                    _itemDataImportField = new ItemDataImportFieldRepository(_context);
                }
                return _itemDataImportField;
            }
        }

        public IItemDataRepository ItemData
        {
            get
            {
                if (_itemData == null)
                {
                    _itemData = new ItemDataRepository(_context);
                }
                return _itemData;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

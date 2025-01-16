using AutoMapper;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;

namespace NAiteWebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // User
            CreateMap<User, UserRes>()
                .ForMember(
                    dest => dest.AuthorityName,
                    opt => opt.MapFrom(src => src.Authority == "admin" ? "管理者" : "ユーザ")
                );
            CreateMap<CreateUserParams, User>();
            CreateMap<UpdateUserParams, User>();

            // Auth
            CreateMap<User, AuthRes>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.LastName + " " + src.FirstName)
                );

            // ItemField
            CreateMap<ItemField, ItemFieldRes>();
            CreateMap<CreateItemFieldParams, ItemField>();
            CreateMap<UpdateItemFieldParams, ItemField>();

            // ItemRow
            CreateMap<ItemRow, ItemRowRes>()
                .ForMember(
                    dest => dest.Items,
                    opt => opt.MapFrom(src => src.Items)
                )
                .ForMember(
                    dest => dest.Code,
                    opt => opt.MapFrom(src => src.Items.Where(a => a.ItemField.FixedFieldType == "Code").FirstOrDefault() == null ? null : src.Items.Where(a => a.ItemField.FixedFieldType == "Code").FirstOrDefault()!.ValueText)
                );

            // Item
            CreateMap<Item, ItemRes>()
                 .ForMember(
                    dest => dest.Files,
                    opt => opt.MapFrom(src => src.ItemFiles.Select(a => a.File))
                )
                 .ForMember(
                    dest => dest.ValueDateTime,
                    opt => opt.MapFrom(src => src.ValueDateTime == null ? null : src.ItemField.Type == "date" ? Convert.ToDateTime(src.ValueDateTime).ToString("yyyy-MM-dd") : src.ItemField.Type == "datetime" ? Convert.ToDateTime(src.ValueDateTime).ToString("yyyy-MM-dd HH:mm") : null)
                );

            CreateMap<UpdateItemParams, Item>();

            // File
            CreateMap<NAiteEntities.Models.File, FileRes>()
                .ForMember(
                    dest => dest.URL,
                    opt => opt.MapFrom(src => "/" + NAiteSettings.GetFileDirectory().Replace(Path.DirectorySeparatorChar, '/') + "/" + src.Name)
                );

            // ItemDataImport
            CreateMap<ItemDataImport, ItemDataImportRes>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId == null ? null : src.User.Id)
                )
                .ForMember(
                    dest => dest.UserName,
                    opt => opt.MapFrom(src => src.UserId == null ? null : src.User.LastName + " " + src.User.FirstName)
                );
            CreateMap<UpdateItemDataImportParams, ItemDataImport>();

            // StoreImportField
            CreateMap<ItemDataImportField, ItemDataImportFieldRes>();
            CreateMap<CreateItemDataImportFieldParams, ItemDataImportField>();
            CreateMap<UpdateItemDataImportFieldParams, ItemDataImportField>();
        }

    }
}

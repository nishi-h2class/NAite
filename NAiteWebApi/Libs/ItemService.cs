using NAiteEntities.Models;
using NAiteWebApi.Repository.Contracts;
using NAiteWebApi.Repository.Repositories;

namespace NAiteWebApi.Libs
{
    public class ItemService
    {
        public static void SetItemValue(string? value, string fieldType, ref Item itemEntity, IRepositoryWrapper _repository)
        {
            switch (fieldType)
            {
                case "int":
                    int valueInt;
                    if (string.IsNullOrEmpty(value))
                    {
                        itemEntity.ValueInt = null;
                    }
                    else
                    {
                        if (int.TryParse(value, out valueInt))
                        {
                            itemEntity.ValueInt = valueInt;
                        }
                    }
                    break;
                case "decimal":
                    decimal valueDecimal;
                    if (string.IsNullOrEmpty(value))
                    {
                        itemEntity.ValueDecimal = null;
                    }
                    else
                    {
                        if (decimal.TryParse(value, out valueDecimal))
                        {
                            itemEntity.ValueDecimal = valueDecimal;
                        }
                    }
                    break;
                case "date":
                case "datetime":
                    DateTime valueDateTime;
                    if (string.IsNullOrEmpty(value))
                    {
                        itemEntity.ValueDateTime = null;
                    }
                    else
                    {
                        var date = value.Replace("-", "/");
                        if (DateTime.TryParse(date, out valueDateTime))
                        {
                            itemEntity.ValueDateTime = valueDateTime;
                        }
                    }
                    break;
                case "file":
                    // 現在の紐づけを削除
                    foreach (var itemFile in itemEntity!.ItemFiles)
                    {
                        _repository.ItemFile.DeleteItemFile(itemFile);
                    }
                    if (!string.IsNullOrEmpty(value))
                    {
                        var fileNames = value.Split(',');
                        foreach (var fn in fileNames)
                        {
                            var fileEntity = _repository.File.GetFileByName(fn.Trim());
                            if (fileEntity != null)
                            {
                                var itemFile = new ItemFile
                                {
                                    ItemId = itemEntity.Id,
                                    FileId = fileEntity.Id
                                };
                                _repository.ItemFile.CreateItemFile(itemFile);
                            }
                        }
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(value))
                    {
                        itemEntity.ValueText = null;
                    }
                    else
                    {
                        itemEntity.ValueText = value;
                    }
                    break;
            }
        }
    }
}

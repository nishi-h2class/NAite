using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemsController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult UpdateItems([FromBody] UpdateItemParams[] param)
        {
            Logs.Logger.Debug("商品情報の更新");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("Item object sent from client is null.");
                return BadRequest("Item object is null");
            }

            foreach (var item in param)
            {
                if (!ModelState.IsValid)
                {
                    string msg = "対象の商品情報が編集中です";
                    Logs.Logger.Error(msg);
                    return BadRequest(msg);
                }

                var itemEntity = _repository.Item.GetItem(item.Id);
                if (itemEntity is null)
                {
                    string msg = $"商品ID:{item.Id}は存在しません";
                    Logs.Logger.Error(msg);
                    return NotFound(msg);
                }
            }
            
            foreach(var item in param)
            {
                var itemEntity = _repository.Item.GetItem(item.Id);
                _mapper.Map(item, itemEntity);
                _repository.Item.UpdateItem(itemEntity!);
                if (item.Files != null){
                    // 現在の紐づけを削除
                    foreach (var itemFile in itemEntity!.ItemFiles)
                    {
                        _repository.ItemFile.DeleteItemFile(itemFile);
                    }
                    if (item.Files.Count() > 0)
                    {
                        // 紐づけを追加
                        foreach (var file in item.Files)
                        {
                            var itemFile = new ItemFile
                            {
                                ItemId = item.Id,
                                FileId = file.Id
                            };
                            _repository.ItemFile.CreateItemFile(itemFile);
                        }
                    }
                }

                _repository.Save();
            }

            return NoContent();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemFieldsController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemFieldsController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetItemFields()
        {
            Logs.Logger.Debug("商品フィールド一覧の取得");

            var fields = _repository.ItemField.GetItemFields();

            var fieldsResult = _mapper.Map<IEnumerable<ItemFieldRes>>(fields);

            return Ok(fieldsResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemField(string id)
        {
            Logs.Logger.Debug("商品フィールド情報の取得 Id:" + id);

            var field = _repository.ItemField.GetItemField(id);

            if (field is null)
            {
                string msg = $"商品フィールドID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var fieldsResult = _mapper.Map<ItemFieldRes>(field);

            return Ok(fieldsResult);
        }

        [HttpPost]
        public IActionResult CreateItemField([FromBody] CreateItemFieldParams param)
        {
            Logs.Logger.Debug("商品フィールド情報の新規登録");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("ItemField object sent from client is null.");
                return BadRequest("ItemField object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品フィールド情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var fieldEntity = _mapper.Map<ItemField>(param);

            _repository.ItemField.CreateItemField(fieldEntity);

            var rows = _repository.ItemRow.GetAllItemRows();
            foreach (var row in rows)
            {
                var itemEntity = new Item
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ItemRowId = row.Id,
                    ItemFieldId = fieldEntity.Id,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                _repository.Item.CreateItem(itemEntity);
            }
            _repository.Save();

            var createdItemField = _mapper.Map<ItemFieldRes>(fieldEntity);

            return CreatedAtAction("GetItemField", new { id = createdItemField.Id }, createdItemField);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItemField(string id, [FromBody] UpdateItemFieldParams param)
        {
            Logs.Logger.Debug("ユーザ情報の更新");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("ItemField object sent from client is null.");
                return BadRequest("ItemField object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品フィールド情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var fieldEntity = _repository.ItemField.GetItemField(id);
            if (fieldEntity is null)
            {
                string msg = $"商品フィールドID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            _mapper.Map(param, fieldEntity);
            _repository.ItemField.UpdateItemField(fieldEntity);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemField(string id)
        {
            Logs.Logger.Debug("商品フィールド情報の削除 Id:" + id);

            var field = _repository.ItemField.GetItemField(id);
            if (field is null)
            {
                string msg = $"商品フィールドID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            _repository.ItemField.DeleteItemField(field);
            _repository.Save();

            return NoContent();
        }

        [HttpPut("{id}/sort")]
        public IActionResult UpdateItemFieldOrder(string id, [FromBody] UpdateItemFieldOrderParams param)
        {
            Logs.Logger.Debug("商品フィールドの並び替え");
            Logs.Logger.Debug($"商品フィールドID:{id}、商品フィールドID:{param.ReplaceItemFieldId}");

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品フィールド情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var fieldEntity = _repository.ItemField.GetItemField(id);
            if (fieldEntity is null)
            {
                string msg = $"商品フィールドID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var replaceEntity = _repository.ItemField.GetItemField(param.ReplaceItemFieldId);
            if (replaceEntity is null)
            {
                string msg = $"商品フィールドID:{param.ReplaceItemFieldId}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var myOrder = fieldEntity.Order;
            var replaceOrder = replaceEntity.Order;

            fieldEntity.Order = replaceOrder;
            replaceEntity.Order = myOrder;
            _repository.ItemField.UpdateItemField(fieldEntity);
            _repository.ItemField.UpdateItemField(replaceEntity);
            _repository.Save();

            return NoContent();
        }
    }
}

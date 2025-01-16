using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata.Ecma335;
using ClosedXML.Excel;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.VisualBasic.FileIO;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemRowsController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemRowsController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public IActionResult GetItemRows([FromBody] ItemRowParams param)
        {
            Logs.Logger.Debug("商品行一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            string? type = null;
            if (param.SortKey != null)
            {
                type = _repository.ItemField.GetItemField(param.SortKey)?.Type;
            }

            var rows = _repository.ItemRow.GetItemRows(param, type);

            var metadata = new
            {
                rows.TotalCount,
                rows.PageSize,
                rows.CurrentPage,
                rows.TotalPages,
                rows.HasNext,
                rows.HasPrevious,
            };
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var rowsResult = _mapper.Map<IEnumerable<ItemRowRes>>(rows);

            return Ok(rowsResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemRow(string id)
        {
            Logs.Logger.Debug("商品行情報の取得 Id:" + id);

            var row = _repository.ItemRow.GetItemRow(id);

            if (row is null)
            {
                string msg = $"商品行ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var rowsResult = _mapper.Map<ItemRowRes>(row);

            return Ok(rowsResult);
        }

        [HttpPost]
        public IActionResult CreateItemRow()
        {
            Logs.Logger.Debug("商品行情報の新規登録");

            var order = _repository.ItemRow.GetItemRowCount();
            var rowEntity = new ItemRow
            {
                Id = Guid.NewGuid().ToString("N"),
                DefaultOrder = order,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            _repository.ItemRow.CreateItemRow(rowEntity);
            //_repository.Save();

            var fields = _repository.ItemField.GetItemFields();
            foreach ( var field in fields )
            {
                var itemEntity = new Item
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ItemRowId = rowEntity.Id,
                    ItemFieldId = field.Id,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };
                _repository.Item.CreateItem(itemEntity);
            }
            _repository.Save();

            var createdItemRow = _mapper.Map<ItemRowRes>(rowEntity);

            return CreatedAtAction("GetItemRow", new { id = createdItemRow.Id }, createdItemRow);
        }

        [HttpPost("import")]
        public IActionResult ImportItemRows([FromForm] FileUtil.FileUploadRequest request, [FromQuery] bool isHeader)
        {
            Logs.Logger.Debug("商品取込");

            var fields = _repository.ItemField.GetItemFields();

            // 商品コードフィールドチェック
            var itemCodeField = fields.Where(a => a.FixedFieldType == "Code").FirstOrDefault();

            if (itemCodeField is null)
            {
                string msg = $"固定フィールド種別で商品コードが設定されていません";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            if (string.IsNullOrEmpty(itemCodeField.ExcelColumnName))
            {
                string msg = $"商品コードのインポート時列アルファベットが設定されていません";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            try
            {
                var file = request.File;
                if (file.Length > 0)
                {
                    var now = DateTime.Now;
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"');
                    var savePath = NAiteSettings.GetFilePath();

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    var fullPath = Path.Combine(savePath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // 先頭６行だけ読み込み
                    var rows = new List<string[]>();
                    int num = 0;


                    using (var workbook = new XLWorkbook(fullPath))
                    {
                        var worksheet = workbook.Worksheet(1); // 1番目のシートを取得
                        var range = worksheet.RangeUsed();
                        if (range is null)
                            return BadRequest(); // todo

                        var dataRows = range.RowsUsed(); // 使用されている行を取得

                        foreach (var row in dataRows)
                        {
                            if (isHeader == true && num == 0)
                            {
                                num++;
                                continue;
                            }

                            var itemCode = row.Cell(itemCodeField.ExcelColumnName.ToUpper()).GetValue<string>();
                            var itemRow = _repository.ItemRow.GetItemRowByCode(itemCode);

                            if (itemRow is null)
                            {
                                // 新規追加
                                var order = _repository.ItemRow.GetItemRowCount();
                                var rowEntity = new ItemRow
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    DefaultOrder = order,
                                    Created = DateTime.Now,
                                    Modified = DateTime.Now
                                };

                                _repository.ItemRow.CreateItemRow(rowEntity);

                                foreach (var field in fields)
                                {
                                    var itemEntity = new Item
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        ItemRowId = rowEntity.Id,
                                        ItemFieldId = field.Id,
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now
                                    };
                                    if (!string.IsNullOrEmpty(field.ExcelColumnName))
                                    {
                                        var value = row.Cell(field.ExcelColumnName.ToUpper()).GetValue<string>();
                                        ItemService.SetItemValue(value, field.Type, ref itemEntity!, _repository);
                                        _repository.Item.CreateItem(itemEntity);
                                    }
                                }
                            }
                            else
                            {
                                // 更新
                                foreach (var field in fields)
                                {
                                    var itemEntity = _repository.Item.GetItemByRowField(itemRow.Id, field.Id);                                    
                                    if (!string.IsNullOrEmpty(field.ExcelColumnName))
                                    {
                                        var value = row.Cell(field.ExcelColumnName.ToUpper()).GetValue<string>();
                                        ItemService.SetItemValue(value, field.Type, ref itemEntity!, _repository);
                                        _repository.Item.UpdateItem(itemEntity);
                                    }
                                }
                            }
                        }

                        _repository.Save();
                    }
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemRow(string id)
        {
            Logs.Logger.Debug("商品行情報の削除 Id:" + id);

            var row = _repository.ItemRow.GetItemRow(id);
            if (row is null)
            {
                string msg = $"商品フ行ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            _repository.ItemRow.DeleteItemRow(row);
            _repository.Save();

            return NoContent();
        }
    }
}

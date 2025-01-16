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

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemDataImportsController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemDataImportsController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetItemDataImports([FromQuery] ItemDataImportParams param)
        {
            Logs.Logger.Debug("商品在庫データ取込一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            var itemDataImports = _repository.ItemDataImport.GetItemDataImports(param);

            var metaItemDataImport = new
            {
                itemDataImports.TotalCount,
                itemDataImports.PageSize,
                itemDataImports.CurrentPage,
                itemDataImports.TotalPages,
                itemDataImports.HasNext,
                itemDataImports.HasPrevious,
            };
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaItemDataImport));

            var itemDataImportsResult = _mapper.Map<IEnumerable<ItemDataImportRes>>(itemDataImports);

            return Ok(itemDataImportsResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemDataImport(string id)
        {
            Logs.Logger.Debug("商品在庫データ取込情報の取得 Id:" + id);

            var itemDataImport = _repository.ItemDataImport.GetItemDataImport(id);

            if (itemDataImport is null)
            {
                string msg = $"商品在庫データ取込ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            try
            {
                var fullPath = Path.Combine(NAiteSettings.GetItemDataImportFilePath(), itemDataImport.FileName);

                // 先頭５行だけ読み込み
                var rows = new List<string[]>();
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    int num = 0;
                    while (!reader.EndOfStream)
                    {
                        if ((itemDataImport.IsHeader == true && num == 6) || (itemDataImport.IsHeader == false && num == 5))
                            break;

                        string line = reader.ReadLine()!;
                        if (line == null)
                            break;

                        if (itemDataImport.IsHeader == true && num == 0)
                        {
                            num++;
                            continue;
                        }

                        string[] values = line.Split(',');
                        rows.Add(values);
                        num++;
                    }
                }

                var result = new ItemDataImportCreateRes
                {
                    Id = itemDataImport.Id,
                    FileType = itemDataImport.FileType,
                    Rows = rows
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public IActionResult CreateItemDataImport([FromForm] FileUtil.FileUploadRequest request)
        {
            Logs.Logger.Debug("商品在庫データ取込情報の新規登録");

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品在庫データ取込情報が編集中です";
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
                    var savePath = Path.Combine(NAiteSettings.GetItemDataImportFilePath(), now.ToString("yyyy"));

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    var saveFileName = FileUtil.DuplicateFileName(savePath, fileName);
                    var fullPath = Path.Combine(savePath, saveFileName);

                    var ItemDataImport = new ItemDataImport
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        FileName = saveFileName,
                        OriginalFileName = fileName,
                        Created = now,
                        Modified = now
                    };

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _repository.ItemDataImport.Create(ItemDataImport);
                    _repository.Save();

                    // 先頭６行だけ読み込み
                    var rows = new List<string[]>();
                    using (StreamReader reader = new StreamReader(fullPath))
                    {
                        int num = 0;
                        while (!reader.EndOfStream)
                        {
                            if (num == 6)
                                break;

                            string line = reader.ReadLine()!;
                            if (line == null)
                                break;

                            string[] values = line.Split(',');
                            rows.Add(values);
                            num++;
                        }
                    }

                    var result = new ItemDataImportCreateRes
                    {
                        Id = ItemDataImport.Id,
                        FileType = ItemDataImport.FileType,
                        Rows = rows
                    };
                    return Ok(result);
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

        [HttpPut("{id}")]
        public IActionResult UpdateItemDataImport(string id, [FromBody] UpdateItemDataImportParams param)
        {
            Logs.Logger.Debug("商品在庫データ取込情報の更新");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("ItemDataImport object sent from client is null.");
                return BadRequest("ItemDataImport object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品在庫データ取込情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var ItemDataImportEntity = _repository.ItemDataImport.GetItemDataImport(id);
            if (ItemDataImportEntity is null)
            {
                string msg = $"商品在庫データ取込ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            _mapper.Map(param, ItemDataImportEntity);
            _repository.ItemDataImport.UpdateItemDataImport(ItemDataImportEntity);

            int order = 1;
            foreach (var field in param.Fields)
            {
                field.ItemDataImportId = ItemDataImportEntity.Id;
                field.Order = order;
                var ItemDataImportFieldEntity = _mapper.Map<ItemDataImportField>(field);
                _repository.ItemDataImportField.CreateItemDataImportField(ItemDataImportFieldEntity);
                order++;
            }
            _repository.Save();

            return NoContent();
        }

        [HttpPut("{id}/fields")]
        public IActionResult UpdateItemDataImportFields(string id, [FromBody] UpdateItemDataImportFielsParams param)
        {
            Logs.Logger.Debug("商品在庫データ取込情報の更新");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("ItemDataImport object sent from client is null.");
                return BadRequest("ItemDataImport object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象の商品在庫データ取込情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var ItemDataImportEntity = _repository.ItemDataImport.GetItemDataImport(id);
            if (ItemDataImportEntity is null)
            {
                string msg = $"商品在庫データ取込ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            // フィールドの削除
            var fieldParam = new ItemDataImportFieldParams
            {
                ItemDataImportId = id
            };
            var datas = _repository.ItemDataImportField.GetItemDataImportFields(fieldParam);
            foreach (var data in datas)
            {
                _repository.ItemDataImportField.Delete(data);
            }

            // フィールドの追加
            int order = 1;
            foreach (var field in param.Fields)
            {
                field.ItemDataImportId = ItemDataImportEntity.Id;
                field.Order = order;
                var ItemDataImportFieldEntity = _mapper.Map<ItemDataImportField>(field);
                _repository.ItemDataImportField.CreateItemDataImportField(ItemDataImportFieldEntity);
                order++;
            }
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemDataImport(string id)
        {
            Logs.Logger.Debug("商品在庫データ取込情報の削除 Id:" + id);

            var ItemDataImport = _repository.ItemDataImport.GetItemDataImport(id);
            if (ItemDataImport is null)
            {
                string msg = $"商品在庫データ取込ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            _repository.ItemDataImport.DeleteItemDataImport(ItemDataImport);
            _repository.Save();

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public IActionResult Download(string id)
        {
            Logs.Logger.Debug("商品在庫データ取込情報のダウンロード Id:" + id);

            var ItemDataImport = _repository.ItemDataImport.GetItemDataImport(id);

            if (ItemDataImport is null)
            {
                string msg = $"商品在庫データ取込ID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var filePath = Path.Combine(NAiteSettings.GetItemDataImportFilePath(), ItemDataImport.FileName);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", ItemDataImport.OriginalFileName);
        }
    }
}

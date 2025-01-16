using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using System.ComponentModel.Design;
using System.IO;
using System;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public FilesController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetFiles([FromQuery] FileParams param)
        {
            Logs.Logger.Debug("ファイル一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            var files = _repository.File.GetFiles(param);

            var metadata = new
            {
                files.TotalCount,
                files.PageSize,
                files.CurrentPage,
                files.TotalPages,
                files.HasNext,
                files.HasPrevious,
            };
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var filesResult = _mapper.Map<IEnumerable<FileRes>>(files);

            return Ok(filesResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetFile(string id)
        {
            Logs.Logger.Debug("ファイル情報の取得 Id:" + id);

            var file = _repository.File.GetFile(id);

            if (file is null)
            {
                string msg = $"ファイルID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var filesResult = _mapper.Map<FileRes>(file);

            return Ok(filesResult);
        }

        [HttpPost("check")]
        public IActionResult CheckFile([FromBody] string[] names)
        {
            Logs.Logger.Debug("ファイル名のチェック");
            Logs.Logger.Debug(JsonConvert.SerializeObject(names));

            if (names is null)
            {
                Logs.Logger.Error("File object sent from client is null.");
                return BadRequest("File object is null");
            }

            var res = _repository.File.CheckFileName(names);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult CreateFile([FromForm] FileUtil.FileUploadRequest request)
        {
            Logs.Logger.Debug("ファイル情報の新規登録");

            if (!ModelState.IsValid)
            {
                string msg = "対象のファイル情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            try
            {
                var file = request.File;
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"');
                    var savePath = NAiteSettings.GetFilePath();

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    var fullPath = Path.Combine(savePath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var data = _repository.File.GetFileByName(fileName);
                    if (data != null)
                    {
                        data.Modified = DateTime.Now;
                        _repository.File.UpdateFile(data);
                    }
                    else
                    {
                        data = new NAiteEntities.Models.File
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Name = fileName,
                            Type = file.ContentType,
                            Created = DateTime.Now,
                            Modified = DateTime.Now
                        };

                        _repository.File.CreateFile(data);
                    }
                    _repository.Save();

                    var fileResult = _mapper.Map<FileRes>(data);

                    return Ok(fileResult);
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
        public IActionResult DeleteFile(string id)
        {
            Logs.Logger.Debug("ファイル情報の削除 Id:" + id);

            var file = _repository.File.GetFile(id);
            if (file is null)
            {
                string msg = $"ファイルID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            // 現在の紐づけを削除
            var itemFiles = _repository.ItemFile.GetItemFilesByFile(id);
            foreach (var itemFile in itemFiles)
            {
                _repository.ItemFile.DeleteItemFile(itemFile);
            }

            // ファイルを削除
            try
            {
                var fullPath = Path.Combine(NAiteSettings.GetFilePath(), file.Name);
                // ファイルが存在するかを確認
                if (System.IO.File.Exists(fullPath))
                {
                    // ファイルを削除
                    System.IO.File.Delete(fullPath);
                    Logs.Logger.Info("ファイルを削除しました（" + fullPath + "）");
                }
                else
                {
                    Logs.Logger.Info("ファイルが存在しません（" + fullPath + "）");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
            _repository.File.DeleteFile(file);
            _repository.Save();

            return NoContent();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemDataImportFieldsController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemDataImportFieldsController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetItemDataImportFields([FromQuery] ItemDataImportFieldParams param)
        {
            Logs.Logger.Debug("商品在庫データインポートフィールド一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            var datas = _repository.ItemDataImportField.GetItemDataImportFields(param);
            var datasResult = _mapper.Map<IEnumerable<ItemDataImportFieldRes>>(datas);

            return Ok(datasResult);
        }
    }
}

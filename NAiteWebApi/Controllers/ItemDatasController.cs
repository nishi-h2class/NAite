using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;
using System.Net;
using System.Reflection.Emit;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemDatasController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public ItemDatasController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetItemDatas([FromQuery] ItemDataParams param)
        {
            Logs.Logger.Debug("商品在庫データ一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            var itemRow = _repository.ItemRow.GetItemRowByCode(param.Code);

            if (itemRow is null)
            {
                string msg = $"商品コード:{param.Code}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var inventoryDateItem = itemRow.Items.Where(a => a.ItemField.FixedFieldType == "InventoryDate").FirstOrDefault();
            var _inventoryDate = inventoryDateItem == null ? null : inventoryDateItem.ValueDateTime;

            if (_inventoryDate is null)
            {
                string msg = $"棚卸日が設定されていません";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);
            if (_inventoryDate > today)
            {
                string msg = $"棚卸日が今日よりも後に設定されています";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }
            var inventoryDate = (DateTime)_inventoryDate;

            // 棚卸数量
            var inventoryQuantityItem = itemRow.Items.Where(a => a.ItemField.FixedFieldType == "InventoryQuantity").FirstOrDefault();
            var inventoryQuantity = inventoryQuantityItem == null ? 0 : inventoryQuantityItem.ValueInt == null ? 0 : (int)inventoryQuantityItem.ValueInt!;

            // 在庫閾値
            var stockThresholdItem = itemRow.Items.Where(a => a.ItemField.FixedFieldType == "StockThreshold").FirstOrDefault();
            var stockThreshold = stockThresholdItem == null ? null : stockThresholdItem.ValueInt;

            var startDate = param.StartDate;
            var endDate = param.EndDate;

            if (startDate > inventoryDate)
                param.StartDate = inventoryDate;

            if (endDate < inventoryDate) 
                param.EndDate = inventoryDate;

            if (endDate > today)
                param.EndDate = today.AddDays(-1);

            // 入荷取得
            param.Type = "Receving";
            var recevings = _repository.ItemData.GetItemDatas(param);

            // 出荷取得
            param.Type = "Shipping";
            var shippings = _repository.ItemData.GetItemDatas(param);

            // 棚卸日のデータ追加
            var itemDataList = new List<ItemDataRes>();
            var inventoryItemData = new ItemDataRes
            {
                Date = inventoryDate,
                Quantity = inventoryQuantity
            };
            itemDataList.Add(inventoryItemData);

            // 棚卸日以降のデータ追加
            if (param.EndDate > inventoryDate)
            {
                int count = 0;
                for (DateTime date = inventoryDate.AddDays(1); date <= param.EndDate; date = date.AddDays(1))
                {
                    var receving = recevings.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();
                    var shipping = shippings.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();

                    var itemData = new ItemDataRes
                    {
                        Date = date,
                        Quantity = itemDataList[count].Quantity + receving - shipping,
                    };
                    itemDataList.Add(itemData);

                    count++;
                }
            }

            // 棚卸日以前のデータ追加
            if (param.StartDate < inventoryDate)
            {
                for (DateTime date = inventoryDate; date >= param.StartDate; date = date.AddDays(-1))
                {
                    var receving = recevings.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();
                    var shipping = shippings.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();

                    var itemData = new ItemDataRes
                    {
                        Date = date.AddDays(-1),
                        Quantity = itemDataList[0].Quantity - receving + shipping,
                    };
                    itemDataList.Insert(0, itemData);
                }
            }

            // 予定の処理
            if (endDate > today)
            {
                param.StartDate = today;
                param.EndDate = endDate;

                // 入荷予定取得
                param.Type = "ScheduledReceving";
                var scheduledRecevings = _repository.ItemData.GetItemDatas(param);

                // 出荷予定取得
                param.Type = "scheduledShipping";
                var scheduledShipping = _repository.ItemData.GetItemDatas(param);

                int count = itemDataList.Count() - 1;
                for (DateTime date = today; date <= endDate; date = date.AddDays(1))
                {
                    var receving = scheduledRecevings.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();
                    var shipping = scheduledShipping.Where(a => a.Date >= date).Where(a => a.Date < date.AddDays(1)).Select(a => a.Quantity).Sum();

                    var itemData = new ItemDataRes
                    {
                        Date = date,
                        Quantity = itemDataList[count].Quantity + receving - shipping,
                    };
                    itemDataList.Add(itemData);

                    count++;
                }
            }            

            var labels = itemDataList.Where(a => a.Date >= startDate).Where(a => a.Date <= endDate).Select(a => a.Date.ToString("MM/dd")).ToArray();
            var datas = itemDataList.Where(a => a.Date >= startDate).Where(a => a.Date <= endDate).Select(a => a.Quantity).ToArray();

            var graphDatas = new List<GraphData>
                {
                    new GraphData { Id = "data", Data = datas },
                    new GraphData { Id = "scheduleData", Data = [] }
                };

            var graphRes = new GraphRes
            {
                GraphLabels = labels.ToArray(),
                GraphDatas = graphDatas,
                StockThreshold = stockThreshold
            };

            if (datas.Count() > 0)
            {
                if (graphRes.StockThreshold != null)
                {
                    graphRes.MaxValue = (datas.Max() > graphRes.StockThreshold ? datas.Max() + 5 : graphRes.StockThreshold + 5);
                    graphRes.MinValue = (datas.Min() < graphRes.StockThreshold ? datas.Min() - 5 : graphRes.StockThreshold - 5);
                }
                else
                {
                    graphRes.MaxValue = datas.Max() == null ? null : (datas.Max() + 5);
                    graphRes.MinValue = datas.Min() == null ? null : (datas.Min() - 5);
                }
            }
            else
            {
                if (graphRes.StockThreshold != null)
                {
                    graphRes.MaxValue = graphRes.StockThreshold + 5;
                    graphRes.MinValue = graphRes.StockThreshold - 5;
                }
            }

            if (graphRes.GraphLabels.Contains(inventoryDate.ToString("MM/dd")))
            {
                graphRes.InventoryDate = inventoryDate.ToString("MM/dd");
            }

            if (graphRes.GraphLabels.Contains(DateTime.Now.ToString("MM/dd")))
            {
                graphRes.Today = DateTime.Now.ToString("MM/dd");
            }

            return Ok(graphRes);
        }
    }
}

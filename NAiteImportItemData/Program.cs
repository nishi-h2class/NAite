using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NAiteImportItemData;
using static System.Formats.Asn1.AsnWriter;
using System.Text;
using NAiteEntities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace NAite.NAiteImportItemData
{
    class Program
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Debug("Application Start!!!");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //shift-jisのファイル取り扱いに必要な宣言

            // 設定ファイルの読込
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            AppSettings.SetConfig(config);

            try
            {
                // MySQLのバージョン
                MySqlServerVersion serverVersion = new(new Version(8, 0, 36));

                using (var db = new NAiteContext(AppSettings.ConnectionStrings, serverVersion))
                {
                    // インポートするデータを取得
                    var datas = db.ItemDataImports.Where(a => a.Reserved != null).Where(a => a.Imported == null).Where(a => a.Deleted == null).ToArray();

                    if (datas.Count() > 0)
                    {
                        logger.Info("新着データあり[{0}件]", datas.Count());

                        foreach (var data in datas)
                        {

                            // fieldsを取得
                            var fields = db.ItemDataImportFields.Where(a => a.ItemDataImportId == data.Id).Where(a => a.Deleted == null).ToArray();
                            int codeIndex = 0;
                            int dateIndex = 0;
                            int quantityIndex = 0;
                            int index = 0;
                            foreach (var field in fields)
                            {
                                if (field.Tag == "code")
                                {
                                    codeIndex = index;
                                }
                                if (field.Tag == "date")
                                {
                                    dateIndex = index;
                                }
                                if (field.Tag == "quantity")
                                {
                                    quantityIndex = index;
                                }
                                index++;
                            }

                            // データを投入する
                            int num = 0;
                            var fullPath = Path.Combine(AppSettings.ItemDataImportFilePath, data.Created.ToString("yyyy"), data.FileName);
                            using (StreamReader reader = new StreamReader(fullPath))
                            {
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine()!;
                                    if (line == null)
                                        break;

                                    if (data.IsHeader == true && num == 0)
                                    {
                                        num++;
                                        continue;
                                    }

                                    string[] values = line.Split(',');
                                    logger.Info(num + 1 + ": " + line);

                                    // 新規登録
                                    var entity = new ItemData
                                    {
                                        Type = data.FileType!,
                                        Code = values[codeIndex].ToString(),
                                        Date = values[dateIndex] != null ? Convert.ToDateTime(values[dateIndex]) : DateTime.MinValue,
                                        Quantity = values[quantityIndex] != null ? Convert.ToInt32(values[quantityIndex]) : 0,
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now
                                    };
                                    db.ItemDatas.Add(entity);
                                    num++;
                                }
                            }

                            // 行数・インポート日時を保存
                            data.Number = num;
                            data.Imported = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        logger.Info("新着データなし");
                    }


                }
            }
            catch (Exception ex)
            {
                logger.Error($"例外発生：{ex.Message}");
                logger.Error($"{ex.StackTrace}");
            }
            finally
            {
                logger.Debug("Application End!!!");
            }
        }
    }
}
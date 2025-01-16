using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteImportItemData
{
    public class AppSettings
    {
        private static IConfiguration? configuration { get; set; }

        public static void SetConfig(IConfiguration config)
        {
            configuration = config;
        }

        public static string ConnectionStrings
        {
            get
            {
                return configuration!.GetValue<string>("ConnectionStrings:NAiteContext") ?? throw new Exception("connectionStringsの設定がありません");
            }
        }

        public static string PID
        {
            get
            {
                return configuration!.GetValue<string>("Configs:PID") ?? "999";
            }
        }

        public static string ItemDataImportFilePath
        {
            get
            {
                return configuration!.GetValue<string>("Configs:ItemDataImportFilePath") ?? throw new Exception("ItemDataImportFilePathの設定がありません");
            }
        }
    }
}

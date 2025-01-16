namespace NAiteWebApi
{
    public class NAiteSettings
    {
        public static IConfiguration? Configuration { get; set; }

        public static string GetFileDirectory()
        {
            return Configuration!.GetValue<string>("Configs:FileDirectory") ?? throw new Exception("FileDirectoryの設定がありません");
        }

        public static string GetFilePath()
        {
            return Configuration!.GetValue<string>("Configs:FilePath") ?? throw new Exception("FilePathの設定がありません");
        }

        public static string GetTokenSecretKey()
        {
            return Configuration!.GetValue<string>("TokenValidation:SecretKey") ?? throw new Exception("SecretKeyの設定がありません");
        }

        public static string GetTokenIssuer()
        {
            return Configuration!.GetValue<string>("TokenValidation:Issuer") ?? throw new Exception("Issuerの設定がありません");
        }

        public static string GetTokenAudience()
        {
            return Configuration!.GetValue<string>("TokenValidation:Audience") ?? throw new Exception("Audienceの設定がありません");
        }

        public static string GetDynamoDBURL()
        {
            return Configuration!.GetValue<string>("Configs:DynamoDBURL") ?? throw new Exception("DynamoDBURLの設定がありません");
        }

        public static string GetDynamoDBTableName()
        {
            return Configuration!.GetValue<string>("Configs:DynamoDBTableName") ?? throw new Exception("DynamoDBTableNameの設定がありません");
        }

        public static string GetItemImportFilePath()
        {
            return Configuration!.GetValue<string>("Configs:ItemImportFilePath") ?? throw new Exception("ItemImportFilePathの設定がありません");
        }

        public static string GetItemDataImportFilePath()
        {
            return Configuration!.GetValue<string>("Configs:ItemDataImportFilePath") ?? throw new Exception("ItemDataImportFilePathの設定がありません");
        }

        public static string GetSystemAdminLoginId()
        {
            return Configuration!.GetValue<string>("Configs:SystemAdminLoginId") ?? throw new Exception("SystemAdminLoginIdの設定がありません");
        }

        public static string GetSystemAdminLoginPassword()
        {
            return Configuration!.GetValue<string>("Configs:SystemAdminLoginPassword") ?? throw new Exception("SystemAdminLoginPasswordの設定がありません");
        }

        public static string GetAdServerAddress()
        {
            return Configuration!.GetValue<string>("Configs:AdServerAddress") ?? throw new Exception("AdServerAddressの設定がありません");
        }

        public static string GetAdServerPort()
        {
            return Configuration!.GetValue<string>("Configs:GetAdServerPort") ?? throw new Exception("AdServerPortの設定がありません");
        }
    }
}

using System.Collections.Generic;
using System.Security.Principal;
using System.DirectoryServices.Protocols;
using System.Net;

namespace NAiteWebApi.Libs
{
    public class LdapLib
    {
        public static bool Auth(string address, int port, string username, string password)
        {
            try
            {
                // LDAP サーバーとの接続を作成
                using (LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(address, port)))
                {
                    // 認証情報を設定
                    NetworkCredential credential = new NetworkCredential(username, password, address);
                    connection.Credential = credential;

                    // 認証タイプを指定 (簡易バインド)
                    connection.AuthType = AuthType.Basic;

                    // サーバーへのバインド（実際の認証）
                    connection.Bind(); // 認証失敗時に例外がスローされる

                    // バインド成功 → 認証成功
                    return true;
                }
            }
            catch (LdapException ex)
            {
                // LDAP 特有の例外
                Console.WriteLine($"LDAP認証エラー: {ex.Message}");
                Logs.Logger.Error($"LDAP認証エラー: {ex.Message}");
            }
            catch (Exception ex)
            {
                // その他の例外
                Console.WriteLine($"エラー: {ex.Message}");
                Logs.Logger.Error($"エラー: {ex.Message}");
            }

            // 認証失敗時
            return false;
        }

        //public static bool Auth(string server, string username, string password)
        //{
        //    // アカウント情報取得

        //    try
        //    {
        //        string lserver = $"LDAP://{server}";//port:389
        //        string domainAndUsername = server + @"\" + username;

        //        DirectoryEntry entry = new DirectoryEntry(lserver, domainAndUsername, password);
        //        //認証しくじるとここで例外が出る。
        //        object obj = entry.NativeObject;

        //        DirectorySearcher search = new DirectorySearcher(entry);
        //        search.Filter = $"(samAccountName={username})";
        //        SearchResult result = search.FindOne();
        //        if (null == result)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Logger.Info($"LDAP認証エラー：{ex.Message}");

        //        return false;
        //    }
        //}
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace NAiteWebApi.Libs
{
    public class FileUtil
    {
        public record FileUploadRequest([FromForm(Name = "file")] IFormFile File);

        public static string DuplicateFileName(string folder, string fileName)
        {
            var filename_no = fileName;
            string fullFileName = Path.Combine(folder, filename_no);

            int no = 1;
            while (System.IO.File.Exists(fullFileName))
            {
                string filename = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                filename_no = $"{filename}_({no}){ext}";

                fullFileName = Path.Combine(folder, filename_no);
                no++;
            }

            return filename_no;
        }

        // ファイルのエンコードを検出するメソッド
        public static Encoding DetectFileEncoding(string filePath)
        {
            using (var reader = new StreamReader(filePath, true))
            {
                // Peekを呼び出すことで、エンコーディングが設定される
                reader.Peek();
                Encoding encoding = reader.CurrentEncoding;

                // BOMをチェックするために最初の数バイトを読み込む
                byte[] preamble = encoding.GetPreamble();
                byte[] buffer = new byte[preamble.Length];
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(buffer, 0, buffer.Length);
                }

                // BOMが存在しない場合、推測に頼るしかない
                if (!IsPreambleEqual(buffer, preamble))
                {
                    return Encoding.Default; // BOMがない場合のフォールバックエンコーディング
                }

                return encoding;
            }
        }

        // BOM（バイトオーダーマーク）が存在するかどうかをチェックするメソッド
        private static bool IsPreambleEqual(byte[] buffer, byte[] preamble)
        {
            if (buffer.Length < preamble.Length) return false;
            for (int i = 0; i < preamble.Length; i++)
            {
                if (buffer[i] != preamble[i]) return false;
            }
            return true;
        }

        // 文字列をUTF-8エンコードの文字列に変換するメソッド
        public static string ConvertToUtf8String(string input, Encoding sourceEncoding)
        {
            // 入力文字列をバイト配列に変換
            byte[] sourceBytes = sourceEncoding.GetBytes(input);
            // バイト配列をUTF-8の文字列に変換
            return Encoding.UTF8.GetString(sourceBytes);
        }
    }
}

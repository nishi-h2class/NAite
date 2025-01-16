using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NAiteWebApi.Attributes
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.GetString() is not { } str) return DateTime.MinValue;
            if (DateTime.TryParse(str, out var dateTime)) return dateTime;
            throw new JsonException("DateTime Parseエラー");
        }

        /// <summary>
        /// 日時をシリアライズするときにミリ秒は000で出力する
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="dateTimeValue"></param>
        /// <param name="options"></param>
        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(dateTimeValue.ToString(
                "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
    }
}

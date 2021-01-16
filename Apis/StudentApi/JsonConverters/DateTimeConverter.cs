using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentApi.JsonConverters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DATE_FORMAT = "yyyy/MM/dd";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), DATE_FORMAT, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }
    }
}
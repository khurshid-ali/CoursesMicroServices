using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Courses.Common
{
    public class ServiceMessageJsonConverter : JsonConverter<ServiceMessage>
    {
        public override ServiceMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var msg = new ServiceMessage();

            var propertyName = "";
            Dictionary<string, string> dictionary = null;

            while (reader.Read())
            {

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    //starting the dictionary 
                    dictionary = new Dictionary<string, string>();
                    var key = "";
                    var value = "";

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        if (reader.TokenType == JsonTokenType.StartObject)
                        {
                            continue;
                        }

                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            dictionary.Add(key, value);
                            continue;
                        }

                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            reader.Read();
                            key = reader.GetString();
                            reader.Read();
                            reader.Read();
                            value = reader.GetString();

                        }

                    }
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    msg.Parameters = dictionary;
                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                }
                else
                {
                    var value = reader.GetString();
                    msg.GetType().GetProperty(propertyName).SetValue(msg, value, null);
                }
            }

            return msg;
        }

        public override void Write(Utf8JsonWriter writer, ServiceMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("ReplyRoutingKey", value.ReplyRoutingKey);
            writer.WriteString("Body", value.Body);

            writer.WriteStartArray("Parameters");
            foreach (var pair in value.Parameters)
            {
                writer.WriteStartObject();
                writer.WriteString("Key", pair.Key);
                writer.WriteString("Value", pair.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}

 
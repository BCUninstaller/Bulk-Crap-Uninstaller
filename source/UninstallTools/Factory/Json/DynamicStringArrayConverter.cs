using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UninstallTools.Factory.Json
{
    /// <summary>
    /// Handle JSON string array entry that has one dimension less or more.
    /// </summary>
    internal class DynamicStringArrayConverter : JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 0-dimension
            if (reader.TokenType == JsonTokenType.String)
                return new[] { reader.GetString() };

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var results = new List<string>();
                ReadStrings(ref reader, results);
                return results.ToArray();
            }

            throw new JsonException($"Expected string or array token but got {reader.TokenType}");
        }

        private static void ReadStrings(ref Utf8JsonReader reader, List<string> results)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                switch (reader.TokenType)
                {
                    // normal
                    case JsonTokenType.String:
                        results.Add(reader.GetString());
                        break;

                    // nested
                    case JsonTokenType.StartArray:
                        var first = ReadFirstString(ref reader);
                        if (first != null)
                            results.Add(first);
                        break;

                    case JsonTokenType.StartObject:
                        reader.Skip();
                        break;
                }
            }
        }

        private static string ReadFirstString(ref Utf8JsonReader reader)
        {
            string result = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (result != null)
                {
                    if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                        reader.Skip();
                    continue;
                }

                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        result = reader.GetString();
                        break;

                    case JsonTokenType.StartArray:
                        result = ReadFirstString(ref reader);
                        break;

                    case JsonTokenType.StartObject:
                        reader.Skip();
                        break;
                }
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

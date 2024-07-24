using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Nelderim;

public class ShortDoubleJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString("F2", CultureInfo.InvariantCulture));
    }
}
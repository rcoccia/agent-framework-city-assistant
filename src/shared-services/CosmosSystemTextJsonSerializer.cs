using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace SharedServices;

public class CosmosSystemTextJsonSerializer : CosmosSerializer
{
    private readonly JsonSerializerOptions? _options;

//    private static JsonSerializerOptions CreateDefaultJsonOptions()
//    {
//        var options = new JsonSerializerOptions();
//#if NET9_0_OR_GREATER
//        options.TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver();
//#endif
//        return options;
//    }

    public CosmosSystemTextJsonSerializer(JsonSerializerOptions? options = null)
    {
        _options = options;// ?? CreateDefaultJsonOptions();
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream.CanSeek && stream.Length == 0)
            {
                return default!;
            }

            return JsonSerializer.Deserialize<T>(stream, _options)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var json = JsonSerializer.Serialize(input, _options);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}


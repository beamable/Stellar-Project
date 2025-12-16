using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beamable.StellarFederation.Features.Minting.Json;

public class NftAttributesConverter : JsonConverter<IDictionary<string, string>>
{
    public override IDictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IDictionary<string, string> value, JsonSerializerOptions options)
    {
        var nftAttributes = value.Select(kv => new NftExternalMetadata.NftAttribute { Type = kv.Key, Value = kv.Value }).ToList();
        JsonSerializer.Serialize(writer, nftAttributes, options);
    }
}
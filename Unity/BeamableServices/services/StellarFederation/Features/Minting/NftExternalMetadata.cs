using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Beamable.StellarFederation.Features.Minting.Json;

namespace Beamable.StellarFederation.Features.Minting;

[Serializable]
public class NftExternalMetadata
{
	[JsonPropertyName("name")]
	public string Name { get; private set; } = null!;

	[JsonPropertyName("description")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string Description { get; set; } = "";

	[JsonPropertyName("image")]
	public string Image { get; set; } = "";

	[JsonPropertyName("symbol")]
	public string Symbol { get; set; } = "";

	[JsonPropertyName("external_url")]
	public string ExternalUrl { get; set; } = "";

	[JsonPropertyName("attributes")]
	[JsonConverter(typeof(NftAttributesConverter))]
	public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

	public class NftAttribute
	{
		[JsonPropertyName("trait_type")]
		public string Type { get; set; } = "";

		[JsonPropertyName("value")]
		public object Value { get; set; } = "";

	}
}
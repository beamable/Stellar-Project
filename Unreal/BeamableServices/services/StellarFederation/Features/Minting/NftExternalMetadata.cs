using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Beamable.Common.Content;
using Beamable.StellarFederation.Features.Minting.Json;
using StellarFederationCommon.FederationContent;

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

	public NftExternalMetadata Load(IContentObject contentObject, uint tokenId, Dictionary<string, string> requestProperties)
	{
		switch (contentObject)
		{
			case INftBase nftBase:
				Name = nftBase.Name;
				Description = nftBase.Description;
				Image = nftBase.Image;
				Attributes = nftBase.CustomProperties;
				Symbol = nftBase.Name.ToUpper();
				break;
		}
		AddCustomProperties(requestProperties);
		return this;
	}

	private void AddCustomProperties(Dictionary<string, string> requestProperties)
	{
		if (requestProperties.Count == 0) return;
		foreach (var kvp in requestProperties)
		{
			var key = kvp.Key.TrimStart('$');
			Attributes[key] = kvp.Value;
		}
	}
}
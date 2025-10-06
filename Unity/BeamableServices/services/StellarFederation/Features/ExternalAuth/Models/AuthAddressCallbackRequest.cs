using System.Text.Json.Serialization;

namespace Beamable.StellarFederation.Features.ExternalAuth.Models;

public readonly record struct AuthAddressCallbackRequest(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("gamerTag")] string GamerTag);
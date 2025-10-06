using System.Text.Json.Serialization;

namespace Beamable.StellarFederation.Features.ExternalAuth.Models;

public readonly record struct AuthSignatureCallbackRequest(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("signature")] string Signature);
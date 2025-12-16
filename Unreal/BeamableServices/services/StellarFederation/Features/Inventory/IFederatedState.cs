using System.Collections.Generic;
using Beamable.Common;

namespace Beamable.StellarFederation.Features.Inventory;

public interface IFederatedState { }

public record CurrenciesState : IFederatedState
{
    public Dictionary<string, long> Currencies { get; init; } = new();
}

public record ItemsState : IFederatedState
{
    public Dictionary<string, List<FederatedItemProxy>> Items { get; init; } = new();
}
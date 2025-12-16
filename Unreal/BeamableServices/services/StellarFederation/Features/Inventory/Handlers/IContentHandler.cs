using System.Threading.Tasks;

namespace Beamable.StellarFederation.Features.Inventory.Handlers;

public interface IContentHandler
{
    Task<IFederatedState> GetState(string wallet, string contentId);
}
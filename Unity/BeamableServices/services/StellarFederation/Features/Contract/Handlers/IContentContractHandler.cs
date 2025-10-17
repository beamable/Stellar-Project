using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Models;

namespace Beamable.StellarFederation.Features.Contract.Handlers;

public interface IContentContractHandler
{
    Task HandleContract(ContentContractsModel model);
}
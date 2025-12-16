using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Contract.Exceptions;

public class ContractException(string message) : MicroserviceException((int)HttpStatusCode.BadRequest, "ContractException",
    message);
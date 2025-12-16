using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Stellar.Exceptions;

public class StellarServiceException(string message)
    : MicroserviceException((int)HttpStatusCode.BadRequest, "StellarServiceException", message);
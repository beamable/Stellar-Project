using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Accounts.Exceptions;

public class UnauthorizedException(string message)
    : MicroserviceException((int)HttpStatusCode.Unauthorized, "Unauthorized", message);
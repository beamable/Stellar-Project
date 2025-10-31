using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Accounts.Exceptions;

public class UnknownAccountException(string message)
    : MicroserviceException((int)HttpStatusCode.BadRequest, "UnknownAccountException", message);
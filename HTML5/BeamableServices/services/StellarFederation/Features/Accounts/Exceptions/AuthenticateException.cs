using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Accounts.Exceptions;

public class AuthenticateException(string message)
    : MicroserviceException((int)HttpStatusCode.BadRequest, "AuthenticateException", message);
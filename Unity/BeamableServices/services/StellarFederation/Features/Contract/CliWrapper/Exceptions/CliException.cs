using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Contract.CliWrapper.Exceptions;

public class CliException(string message) : MicroserviceException((int)HttpStatusCode.BadRequest, "CliException",
    message);
using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Contract.Exceptions;

public class SendTransactionException(string message) : MicroserviceException((int)HttpStatusCode.BadRequest, "SendTransactionException",
    message);
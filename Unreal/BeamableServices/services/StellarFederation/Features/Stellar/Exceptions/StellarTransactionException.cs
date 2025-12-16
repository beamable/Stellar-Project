using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.Stellar.Exceptions;

public class StellarTransactionException(string message)
    : MicroserviceException((int)HttpStatusCode.BadRequest, "StellarTransactionException", message);
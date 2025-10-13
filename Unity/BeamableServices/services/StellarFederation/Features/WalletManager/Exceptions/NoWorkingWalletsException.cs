using System.Net;
using Beamable.Server;

namespace Beamable.StellarFederation.Features.WalletManager.Exceptions;

public class NoWorkingWalletsException()
    : MicroserviceException((int)HttpStatusCode.BadRequest, "NoWorkingWalletsException", "");
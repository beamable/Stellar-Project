using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Exceptions;
using StellarFederationCommon;

namespace Beamable.StellarFederation.Endpoints;

public class AuthenticateEndpoint : IEndpoint
{
    private readonly AccountsService _accountsService;
    private readonly RequestContext _requestContext;

    public AuthenticateEndpoint(AccountsService accountsService, RequestContext requestContext)
    {
        _accountsService = accountsService;
        _requestContext = requestContext;
    }

    public async Task<FederatedAuthenticationResponse> Authenticate(string token, string challenge, string solution)
    {
        var microserviceInfo = MicroserviceMetadataExtensions.GetMetadata<StellarFederation, StellarWeb3Identity>();
        // Check if an external identity is already attached (from request context)
        if (_requestContext.UserId != 0L)
        {
            var existingExternalIdentity = _requestContext.GetExternalIdentity(microserviceInfo);

            if (existingExternalIdentity is not null)
            {
                return new FederatedAuthenticationResponse
                {
                    user_id = existingExternalIdentity.userId
                };
            }
        }

        if (string.IsNullOrEmpty(token) && _requestContext.UserId != 0L)
        {
            // Create new account for player if token is empty
            var account = await _accountsService.GetAccount(_requestContext.UserId.ToString());
            if (account is null || !account.Value.Created)
            {
                throw new UnauthorizedException($"Wallet for user {_requestContext.UserId} is not created.");
            }
            return new FederatedAuthenticationResponse
            {
                user_id = account.Value.Address
            };
        }

        throw new UnauthorizedException($"{StellarFederationSettings.StellarIdentityName} namespace is not used for external wallets.");
    }
}
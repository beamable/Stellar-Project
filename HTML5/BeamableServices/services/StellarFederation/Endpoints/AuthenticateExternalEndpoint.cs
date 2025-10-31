using System;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts.Exceptions;
using Beamable.StellarFederation.Features.ExternalAuth;
using Beamable.StellarFederation.Features.ExternalAuth.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;
using StellarFederationCommon;

namespace Beamable.StellarFederation.Endpoints;

public class AuthenticateExternalEndpoint : IEndpoint
{
    private readonly Configuration _configuration;
    private readonly RequestContext _requestContext;
    private readonly StellarService _stellarService;
    private readonly ExternalAuthService _externalAuthService;

    private const string SignPrefix = "Stellar Signed Message:\n";

    public AuthenticateExternalEndpoint(Configuration configuration, RequestContext requestContext, StellarService stellarService, ExternalAuthService externalAuthService)
    {
        _configuration = configuration;
        _requestContext = requestContext;
        _stellarService = stellarService;
        _externalAuthService = externalAuthService;
    }

    public async Promise<FederatedAuthenticationResponse> Authenticate(string token, string challenge, string solution)
    {
        // Check if an external identity is already attached (from request context)
        if (_requestContext.UserId != 0L)
        {
            var microserviceInfo = MicroserviceMetadataExtensions.GetMetadata<StellarFederation, StellarWeb3ExternalIdentity>();
            var existingExternalIdentity = _requestContext.GetExternalIdentity(microserviceInfo);

            if (existingExternalIdentity is not null)
            {
                return new FederatedAuthenticationResponse
                {
                    user_id = existingExternalIdentity.userId
                };
            }
        }

        if (string.IsNullOrWhiteSpace(token) && _requestContext.UserId != 0L)
        {
            throw new UnauthorizedException("Please provide an external token");
        }

        // Challenge-based authentication
        if (!string.IsNullOrEmpty(challenge) && !string.IsNullOrEmpty(solution))
        {
            if (_stellarService.IsSignatureValid(token, challenge, solution, SignPrefix))
                // User identity is confirmed
                return new FederatedAuthenticationResponse
                {
                    user_id = token
                };

            // Signature is invalid, user identity isn't confirmed
            BeamableLogger.LogWarning(
                "Invalid signature {signature} for challenge {challenge} and account {account}", solution,
                challenge, token);
            throw new UnauthorizedException("Invalid signature");
        }

        var message = $"Please sign this random message to authenticate: {Guid.NewGuid()}";
        await _externalAuthService.Upsert(new ExternalAuth(token, _requestContext.UserId, message, DateTime.UtcNow.AddSeconds(await _configuration.AuthenticationChallengeTtlSec)));

        // Generate a challenge
        return new FederatedAuthenticationResponse
        {
            challenge = message,
            challenge_ttl = await _configuration.AuthenticationChallengeTtlSec
        };
    }
}
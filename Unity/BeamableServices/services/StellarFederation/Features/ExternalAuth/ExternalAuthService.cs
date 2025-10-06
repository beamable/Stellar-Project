using System;
using System.Text.Json;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.ExternalAuth.Models;
using Beamable.StellarFederation.Features.ExternalAuth.Storage;
using Beamable.StellarFederation.Features.Notifications;
using SuiFederationCommon.Models.Notifications;

namespace Beamable.StellarFederation.Features.ExternalAuth;

public class ExternalAuthService : IService
{
    private readonly PlayerNotificationService _playerNotificationService;
    private readonly ExternalAuthCollection _externalAuthCollection;
    private readonly Configuration _configuration;

    public ExternalAuthService(PlayerNotificationService playerNotificationService, ExternalAuthCollection externalAuthCollection, Configuration configuration)
    {
        _playerNotificationService = playerNotificationService;
        _externalAuthCollection = externalAuthCollection;
        _configuration = configuration;
    }

    public async Task Upsert(Storage.Models.ExternalAuth externalAuth)
    {
        await _externalAuthCollection.Upsert(externalAuth);
    }

    public async Task ProcessAddressCallback(string body)
    {
        try
        {
            var callbackModel = JsonSerializer.Deserialize<AuthAddressCallbackRequest>(body);
            long.TryParse(callbackModel.GamerTag, out var gamerTag);
            await Upsert(new Storage.Models.ExternalAuth(callbackModel.Address, gamerTag, "", DateTime.UtcNow.AddSeconds(await _configuration.AuthenticationChallengeTtlSec)));
            await _playerNotificationService.SendPlayerNotification(gamerTag, new ExternalAuthAddressNotification
            {
                Value = callbackModel.Address
            });
        }
        catch (Exception e)
        {
            BeamableLogger.LogWarning("Error processing auth callback: {Error}", e.Message);
        }
    }

    public async Task ProcessSignatureCallback(string body)
    {
        try
        {
            var callbackModel = JsonSerializer.Deserialize<AuthSignatureCallbackRequest>(body);
            var maybeRecord = await _externalAuthCollection.Get(callbackModel.Address, callbackModel.Message);
            if (maybeRecord is null)
            {
                BeamableLogger.LogWarning("Received invalid or expired auth callback message");
                return;
            }

            await _playerNotificationService.SendPlayerNotification(maybeRecord.GamerTag, new ExternalAuthSignatureNotification
            {
                Value = callbackModel.Signature
            });

        }
        catch (Exception e)
        {
            BeamableLogger.LogWarning("Error processing auth callback: {Error}", e.Message);
        }
    }
}
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
            BeamableLogger.Log("ExternalAuthService.ProcessAddressCallback received payload address={Address} gamerTag={GamerTag}", callbackModel.Address, callbackModel.GamerTag);
            long.TryParse(callbackModel.GamerTag, out var gamerTag);
            if (gamerTag <= 0)
            {
                BeamableLogger.LogWarning("ExternalAuthService.ProcessAddressCallback invalid gamerTag. Raw='{RawGamerTag}' Parsed={Parsed}", callbackModel.GamerTag, gamerTag);
            }
            await Upsert(new Storage.Models.ExternalAuth(callbackModel.Address, gamerTag, "", DateTime.UtcNow.AddSeconds(await _configuration.AuthenticationChallengeTtlSec)));
            BeamableLogger.Log("ExternalAuthService.ProcessAddressCallback upserted external auth record for gamerTag={GamerTag}", gamerTag);
            await _playerNotificationService.SendPlayerNotification(gamerTag, new ExternalAuthAddressNotification
            {
                Value = callbackModel.Address
            });
            BeamableLogger.Log("ExternalAuthService.ProcessAddressCallback queued ExternalAuthAddressNotification for gamerTag={GamerTag}", gamerTag);
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
            BeamableLogger.Log("ExternalAuthService.ProcessSignatureCallback received payload address={Address} messageLength={MessageLength}", callbackModel.Address, callbackModel.Message?.Length ?? 0);
            var maybeRecord = await _externalAuthCollection.Get(callbackModel.Address, callbackModel.Message);
            if (maybeRecord is null)
            {
                BeamableLogger.LogWarning("Received invalid or expired auth callback message");
                return;
            }

            BeamableLogger.Log("ExternalAuthService.ProcessSignatureCallback resolved gamerTag={GamerTag}", maybeRecord.GamerTag);
            await _playerNotificationService.SendPlayerNotification(maybeRecord.GamerTag, new ExternalAuthSignatureNotification
            {
                Value = callbackModel.Signature
            });
            BeamableLogger.Log("ExternalAuthService.ProcessSignatureCallback queued ExternalAuthSignatureNotification for gamerTag={GamerTag}", maybeRecord.GamerTag);

        }
        catch (Exception e)
        {
            BeamableLogger.LogWarning("Error processing auth callback: {Error}", e.Message);
        }
    }
}

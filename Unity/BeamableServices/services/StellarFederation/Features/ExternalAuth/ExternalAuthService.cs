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

    public ExternalAuthService(PlayerNotificationService playerNotificationService, ExternalAuthCollection externalAuthCollection)
    {
        _playerNotificationService = playerNotificationService;
        _externalAuthCollection = externalAuthCollection;
    }

    public async Task Insert(Storage.Models.ExternalAuth externalAuth)
    {
        await _externalAuthCollection.TryInsert(externalAuth);
    }

    public async Task ProcessCallback(string body)
    {
        try
        {
            var callbackModel = JsonSerializer.Deserialize<AuthCallbackRequest>(body);
            var maybeRecord = await _externalAuthCollection.Get(callbackModel.Message);
            if (maybeRecord is null)
            {
                BeamableLogger.LogWarning("Received invalid or expired auth callback message");
                return;
            }

            await _playerNotificationService.SendPlayerNotification(maybeRecord.GamerTag, new ExternalAuthNotification
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common.Api.Auth;
using Beamable.Server.Clients;
using Cysharp.Threading.Tasks;
using Farm.Beam;
using Farm.UI;
using StellarFederationCommon;
using StellarFederationCommon.Models.Response;
using SuiFederationCommon.Models.Notifications;
using TMPro;
using UnityEngine;

namespace Farm.Game.Scripts.Stellar
{
    public class StellarExternalWalletController : MonoBehaviour
    {
        [Header("Id Container")]
        [SerializeField] private GameObject iDContainer;
        [SerializeField] private TextMeshProUGUI externalIdValue;
        
        [Header("Attachment Container")]
        [SerializeField] private GameObject attachmentContainer;
        [SerializeField] private BeamButton attachButton;

        private string _attachUrl = "";
        private ConfigurationResponse _config;
        private ChallengeSolution _challengeSolution;

        public void Init()
        {
            var hasExternalId =
                BeamManager.Instance.AccountManager.HasStellarId(StellarFederationSettings.StellarExternalIdentityName);

            SetupContainers(hasExternalId);
            BeamManager.BeamContext.Api.NotificationService.Subscribe(PlayerNotificationContext.ExternalAuthAddress, OnAddressNotification);
            BeamManager.BeamContext.Api.NotificationService.Subscribe(PlayerNotificationContext.ExternalAuthSignature, OnSignatureNotification);
        }

        private void SetupContainers((bool, string) hasExternalId)
        {
            if (hasExternalId.Item1)
            {
                iDContainer.SetActive(true);
                attachmentContainer.SetActive(false);
                SetExternalId(hasExternalId.Item2);
            }
            else
            {
                iDContainer.SetActive(false);
                attachmentContainer.SetActive(true);
                attachButton.RemoveAllListeners();
                attachButton.AddListener(OnStartAttaching);
            }
        }

        public void DeInit()
        {
            if(attachButton.gameObject.activeInHierarchy) attachButton.RemoveAllListeners();
        }

        private async void OnAddressNotification(object notification)
        {
            //if (notification is not ExternalAuthAddressNotification addressNotification) return;
            if (notification is not IDictionary<string, object> dict) return;
            if (!dict.TryGetValue("Value", out var valueObj)) return;
            var value = valueObj.ToString();
            _challengeSolution = new ChallengeSolution();
            var response = await BeamManager.BeamContext.Api.AuthService.AttachIdentity(value,
                StellarFederationSettings.MicroserviceName,
                StellarFederationSettings.StellarExternalIdentityName);
            _challengeSolution.challenge_token = response.challenge_token;
            var signUrl = ParseChallengeToEncodedMessage(response.challenge_token);
            Application.OpenURL(signUrl);
        }
        
        private async void OnSignatureNotification(object notification)
        {
            if (notification is not IDictionary<string, object> dict) return;
            if (!dict.TryGetValue("Value", out var valueObj)) return;
            var value = valueObj.ToString();
            _challengeSolution.solution = value;
            var playerAccount = BeamManager.Instance.AccountManager.CurrentAccount;
            try
            {
                await BeamManager.BeamContext.Api.AuthService.AttachIdentity(_challengeSolution.challenge_token,
                    StellarFederationSettings.MicroserviceName,
                    StellarFederationSettings.StellarExternalIdentityName);
                await BeamManager.Instance.AccountManager.SwitchAccount(playerAccount);

                iDContainer.SetActive(true);
                var hasExternalId =
                    BeamManager.Instance.AccountManager.HasStellarId(StellarFederationSettings
                        .StellarExternalIdentityName);
                SetExternalId(hasExternalId.Item2);
                attachmentContainer.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.Log($"Signature notification error: {e.Message}");
            }
        }
        
        private string ParseChallengeToEncodedMessage(string urlChallengeToken)
        {
            var parsedToken = BeamManager.BeamContext.Api.AuthService.ParseChallengeToken(urlChallengeToken);
            var challengeBytes = Convert.FromBase64String(parsedToken.challenge);
            var uncodedUrl = Encoding.UTF8.GetString(challengeBytes);
            var encodedMessage = Uri.EscapeDataString(uncodedUrl);
            var signUrl = ConstructConnectUrl(encodedMessage);
            
            return signUrl;
        }

        private void OnStartAttaching()
        {
             StartAttachingAsync().Forget();
        }

        private async UniTask StartAttachingAsync()
        {
            try
            {
                _config = await BeamManager.StellarClient.StellarConfiguration();
                var url = ConstructConnectUrl();
                Application.OpenURL(url);
                Debug.Log($"First Url launched: {url}");
                attachButton.SetText(false, "Attaching...");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error starting attachment process: {e.Message}");
                attachButton.SetText(true, "Attach Wallet");
                //TODO: Better error handling for the player
            }
        }

        private void SetExternalId(string value)
        {
            externalIdValue.text = value;
        }

        private string ConstructConnectUrl()
        {
            var gamerTag = BeamManager.BeamContext.PlayerId;
            var cid = BeamManager.BeamContext.Cid;
            var pid = BeamManager.BeamContext.Pid;
            var configUrl =
                $"{_config.walletConnectBridgeUrl}/?network={_config.network}&projectId={_config.walletConnectProjectId}&cid={cid}&pid={pid}&gamerTag={gamerTag}";
            //walletConnectBridgeUrl/?network=testnet&projectId=walletConnectProjectId&cid=cid&pid=pid&gamerTag=gamerTag&message=urlEncodedMessage
            _attachUrl  = "https://" + configUrl;
            return _attachUrl;
        }

        private string ConstructConnectUrl(string encodedMessage)
        {
            var signUrl = _attachUrl + "&message=" + encodedMessage;
            return signUrl;
        }
    }
}
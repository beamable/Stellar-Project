using Farm.Beam;
using Farm.UI;
using StellarFederationCommon;
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

        public void Init()
        {
            var hasExternalId =
                BeamManager.Instance.AccountManager.HasStellarId(StellarFederationSettings.StellarExternalIdentityName);

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
            }
        }

        public async void OnStartAttaching()
        {
        }

        private void SetExternalId(string value)
        {
            externalIdValue.text = value;
        }
    }
}
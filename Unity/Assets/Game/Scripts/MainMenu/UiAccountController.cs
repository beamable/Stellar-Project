using System;
using Beamable.Editor.Inspectors;
using Cysharp.Threading.Tasks;
using Farm.Beam;
using Farm.Game.Scripts.Stellar;
using Farm.UI;
using StellarFederationCommon;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Farm.MainMenu
{
    public class UiAccountController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI beamLoadingText;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private BeamButton createAccountButton;
        [SerializeField] private GameObject newAccountWindow;
        [SerializeField] private StellarExternalWalletController externalWalletController;
        
        [Header("Account Info")]
        [SerializeField] private GameObject accountInfoWindow;
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI stellarIdValueText;
        [SerializeField] private BeamButton openPortalButton;

        private string _username;
        private bool _isCreatingAccount;

        #region Unity_Methods

        private void Start()
        {
            beamLoadingText.gameObject.SetActive(true);
            newAccountWindow.SetActive(false);
            accountInfoWindow.SetActive(false);
        }

        private void Update()
        {
            if(!newAccountWindow.activeInHierarchy) return;
            if(_isCreatingAccount) return;
            createAccountButton.SetInteractable(!string.IsNullOrEmpty(_username) && _username.Length >= 3);
        }

        private void OnEnable()
        {
            BeamManager.Instance.OnInitialized += Init;
            createAccountButton.AddListener(CreateNewAccount);
            usernameInput.onValueChanged.AddListener(SetUserName);
            openPortalButton.AddListener(OpenUserPortal);
        }

        private void OnDisable()
        {
            BeamManager.Instance.OnInitialized -= Init;
            usernameInput.onValueChanged.RemoveAllListeners();
            openPortalButton.RemoveAllListeners();
            externalWalletController.DeInit();
        }

        private void Init()
        {
            beamLoadingText.gameObject.SetActive(false);
            var hasStellarId = BeamManager.Instance.AccountManager.HasStellarId(StellarFederationSettings.StellarIdentityName);
            if (!hasStellarId.Item1) //no stellar wallet attached
            {
                newAccountWindow.SetActive(true);
            }
            else
            {
                newAccountWindow.SetActive(false);
                accountInfoWindow.SetActive(true);
                SetupAccountPanelInfo(hasStellarId.Item2);
            }
            
            externalWalletController.Init();
        }

        #endregion

        public void OnSwitchToNewAccount()
        {
            newAccountWindow.SetActive(true);
            createAccountButton.SetText(true);
            accountInfoWindow.SetActive(false);
        }

        private void SetupAccountPanelInfo(string stellarId)
        {
            var userName = BeamManager.Instance.AccountManager.CurrentAccount.Alias;
            userNameText.text = $"Welcome {userName}";
            stellarIdValueText.text = stellarId;
        }

        private void SetUserName(string value)
        {
            _username = value;
        }

        private void CreateNewAccount()
        {
            CreateNewAccountAsync().Forget();
        }

        private async UniTask CreateNewAccountAsync()
        {
            _isCreatingAccount = true;
            createAccountButton.SetInteractable(false);
            createAccountButton.SetText(false, "Creating...");
            await BeamManager.Instance.AccountManager.CreateNewAccount(_username);
            Init();
            _isCreatingAccount = false;
        }

        private void OpenUserPortal()
        {
            var url = BeamableBehaviourInspector.PortalPathForContext(BeamManager.BeamContext);
            Application.OpenURL(url);
        }
    }
}
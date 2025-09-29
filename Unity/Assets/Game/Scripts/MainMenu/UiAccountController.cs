using System;
using Cysharp.Threading.Tasks;
using Farm.Beam;
using Farm.UI;
using TMPro;
using UnityEngine;

namespace Farm.MainMenu
{
    public class UiAccountController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI beamLoadingText;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private BeamButton beamButton;
        [SerializeField] private GameObject newAccountWindow;
        
        
        private string _username;

        private void Start()
        {
            beamLoadingText.gameObject.SetActive(true);
            newAccountWindow.SetActive(false);
        }

        private void Update()
        {
            beamButton.SetInteractable(!string.IsNullOrEmpty(_username) && _username.Length >= 3);
        }

        private void OnEnable()
        {
            BeamManager.Instance.OnInitialized += Init;
            beamButton.AddListener(CreateNewAccount);
            usernameInput.onValueChanged.AddListener(SetUserName);
        }

        private void OnDisable()
        {
            BeamManager.Instance.OnInitialized -= Init;
            usernameInput.onValueChanged.RemoveAllListeners();
        }

        private void Init()
        {
            beamLoadingText.gameObject.SetActive(false);
            newAccountWindow.SetActive(true);
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
            await BeamManager.Instance.AccountManager.CreateNewAccount(_username);
        }
    }
}
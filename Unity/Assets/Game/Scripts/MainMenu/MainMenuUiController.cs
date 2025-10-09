using System;
using DG.Tweening;
using Farm.Beam;
using Farm.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Game.Scripts.MainMenu
{
    public class MainMenuUiController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject playGamePanel;
        
        [Header("Scene Names")]
        [SerializeField] private string farmSceneName;

        public void Start()
        {
            DOTween.Init();
            AudioManager.Instance.PlayTitleMusic();
            playGamePanel.SetActive(false);
        }

        private void OnEnable()
        {
            BeamManager.Instance.OnInitialized += Init;
        }
        
        private void OnDisable()
        {
            if(BeamManager.Instance != null)
                BeamManager.Instance.OnInitialized -= Init;
        }
        
        private void Init()
        {
            playGamePanel.SetActive(true);
        }

        #region OnClick Events

        public void OnPlayGame()
        {
            AudioManager.Instance.StopMusic(LoadFarmScene);
            return;
            
            void LoadFarmScene()
            {
                SceneManager.LoadScene(farmSceneName);
            }
        }
        
        public void OnQuitGame()
        {
            Application.Quit();
        }

        #endregion
    }
}
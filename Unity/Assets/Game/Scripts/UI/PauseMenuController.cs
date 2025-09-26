using System;
using Farm.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Farm.UI
{
    
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;

        private UiManager _uiManager;

        public void InitAwake(UiManager uiManager)
        {
            _uiManager = uiManager;
            ForceClosePauseMenu();
        }

        public void InitStart()
        {
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }
        
        private void OnEnable()
        {
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        private void OnDisable()
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
        }

        private void OnSfxChanged(float value)
        {
            AudioManager.Instance.SetVolumeSfx(value);
        }
        private void OnMusicChanged(float value)
        {
            AudioManager.Instance.SetVolumeMusic(value);
        }

        public void OpenPauseMenu()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            _uiManager.RaiseOpenUi(gameObject.activeSelf);
        }
        
        public void ForceClosePauseMenu()
        {
            gameObject.SetActive(false);
            _uiManager.RaiseOpenUi(false);
        }

        public void OnContinueGame()
        {
            ForceClosePauseMenu();
        }

        public void OnMainMenu()
        {
            AudioManager.Instance.StopMusic(LoadMainMenu);
            return;
            
            void LoadMainMenu() => SceneManager.LoadScene(0);
        }
    }
}
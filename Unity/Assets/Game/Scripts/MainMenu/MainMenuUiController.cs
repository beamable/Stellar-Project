using System;
using DG.Tweening;
using Farm.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Game.Scripts.MainMenu
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField] private string farmSceneName;

        public void Start()
        {
            DOTween.Init();
            AudioManager.Instance.PlayTitleMusic();
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
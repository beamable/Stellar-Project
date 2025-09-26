using System;
using Cysharp.Threading.Tasks;
using Farm.Managers;
using Farm.Player;
using Farm.UI;
using UnityEngine;

namespace Farm.Time
{
    public class TimeManager : MonoSingleton<TimeManager>
    {
        [SerializeField] private float hourDuration;
        [SerializeField] private float dayStartTime;
        [SerializeField] private float nightStartTime;
        [SerializeField] private float timeStopToSleep = 24;
        
        private bool _isSleeping, _stopTime;
        public float CurrentTime { get; private set; }
        public int CurrentDay { get; private set; }

        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            CurrentTime = dayStartTime;
            CurrentDay = 1;
            
            AudioManager.Instance.PlayGameMusic();
        }

        private void Update()
        {
            if(_isSleeping || _stopTime) return;
            CurrentTime += UnityEngine.Time.deltaTime / hourDuration;
            if(Mathf.Approximately(CurrentTime, nightStartTime))
            {
                // TODO: do something 
                //maybe cannot use tools anymore? force player to go to bed?
            }

            if (!(CurrentTime >= timeStopToSleep)) return;
            CurrentTime = timeStopToSleep;
            _stopTime = true;
        }

        private void OnEnable()
        {
            PlayerController.OnGoToSleep += OnGoToSleep;
            UiManager.OnPlayerAwoken += OnPlayerAwoken;
            UiManager.OnOpenUi += OnOpenUi;
        }

        private void OnDisable()
        {
            PlayerController.OnGoToSleep -= OnGoToSleep;  
            UiManager.OnPlayerAwoken -= OnPlayerAwoken;
            UiManager.OnOpenUi -= OnOpenUi;
        }

        private void OnGoToSleep()
        {
            _isSleeping = true;
            CurrentDay++;
            UiManager.Instance.GoToSleep(CurrentDay).Forget();
            AudioManager.Instance.StopMusic(null);
            AudioManager.Instance.PlaySfx(1);
        }
        
        private void OnPlayerAwoken()
        {
            _isSleeping = false;
            _stopTime = false;
            CurrentTime = dayStartTime;
            AudioManager.Instance.PlayGameMusic();
            AudioManager.Instance.PlaySfx(6);
        }

        private void OnOpenUi(bool uiOpen)
        {
            _stopTime = uiOpen;
        }
    }
}
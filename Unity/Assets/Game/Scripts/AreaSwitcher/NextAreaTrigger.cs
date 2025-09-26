using System;
using Cysharp.Threading.Tasks;
using Farm.Helpers;
using Farm.UI;
using Farm.Player;
using UnityEngine;

namespace Farm.AreaSwitcher
{
    public class NextAreaTrigger : MonoBehaviour
    {
        [SerializeField] private GameConstants.FarmState nextAreaState;
        [SerializeField] private Transform nextAreaPosition;
       
        private bool _isTriggered;
        public static event Action<GameConstants.FarmState> OnAreaChanged;
        
        private void OnTriggerEnter2D(Collider2D other)
        { 
            HandleAreaTransition(other).Forget();
        }

        private async UniTask HandleAreaTransition(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player) || _isTriggered) return;
            _isTriggered = true;
            player.SetSwitchingAreas(true);
            await UiManager.Instance.FadeIn();
            await UniTask.Yield();
            
            OnAreaChanged?.Invoke(nextAreaState);
            player.transform.position = nextAreaPosition.position;
           
            await UniTask.WaitForEndOfFrame();
            await UiManager.Instance.FadeOut();
            await UniTask.Yield();
            
            _isTriggered = false;
            player.SetSwitchingAreas(false);
        }
    }
}
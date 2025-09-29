using DG.Tweening;
using Farm.Managers;
using UnityEngine;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Farm.UI
{
    public class BeamButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        [Header("Animation Settings")]
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float scaleDuration = 0.25f;

        public void AddListener(UnityAction onClick)
        {
            button.onClick.AddListener(onClick);
        }
        
        public void RemoveAllListeners()
        {
            button.onClick.RemoveAllListeners();
        }
        
        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }
        
        #region Pointer

        public void OnPointerClick()
        {
            AudioManager.Instance.PlaySfx(5);
        }
        
        public void OnPointerEnter()
        {
            transform.DOScale(scaleFactor, scaleDuration).SetEase(Ease.OutBack);
        }
        
        public void OnPointerExit()
        {
            transform.DOScale(1f, scaleDuration).SetEase(Ease.OutBack);
        }

        #endregion
    }
}
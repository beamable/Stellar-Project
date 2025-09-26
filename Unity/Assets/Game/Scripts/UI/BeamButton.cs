using DG.Tweening;
using Farm.Managers;
using UnityEngine;

namespace Farm.UI
{
    public class BeamButton : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float scaleDuration = 0.25f;

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
    }
}
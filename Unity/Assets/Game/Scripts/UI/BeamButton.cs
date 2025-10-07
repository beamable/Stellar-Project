using System;
using DG.Tweening;
using Farm.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Farm.UI
{
    public class BeamButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        [Header("Animation Settings")]
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float scaleDuration = 0.25f;

        private string _defaultText;
        private float _startingScale = 1f;

        private void Start()
        {
            _defaultText = buttonText.text;
        }

        private void OnEnable()
        {
            _startingScale = transform.localScale.x;
        }

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
            if(!button.interactable) return;
            transform.DOScale(scaleFactor, scaleDuration).SetEase(Ease.OutBack);
        }
        
        public void OnPointerExit()
        {
            if(!button.interactable) return;
            transform.DOScale(_startingScale, scaleDuration).SetEase(Ease.OutBack);
        }

        public void SetText(bool toDefault = false, string newText = "")
        {
            buttonText.text = toDefault ? _defaultText : newText;
        }

        #endregion
    }
}
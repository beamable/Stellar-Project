using System;
using Farm.Player;
using Farm.UI;
using UnityEngine;

namespace Farm.Time
{
    public class BedSleepTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject bedCanvas;
        [SerializeField] private Transform playerAwakePosition;

        private PlayerController _player;
        
        private void Awake()
        {
            bedCanvas.SetActive(false);
        }

        private void OnEnable()
        {
            UiManager.OnPlayerAwoken += OnPlayerAwoken;
        }
        
        private void OnDisable()
        {
            UiManager.OnPlayerAwoken -= OnPlayerAwoken;
        }

        private void OnPlayerAwoken()
        {
            if(_player == null) return;
            _player.transform.position = playerAwakePosition.position;
            _player = null;
            bedCanvas.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            _player = player;
            bedCanvas.SetActive(true);
            _player.SetCanGoToSleep(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_player == null) return;
            bedCanvas.SetActive(false);
            _player.SetCanGoToSleep(false);
            _player = null;
        }
    }
}
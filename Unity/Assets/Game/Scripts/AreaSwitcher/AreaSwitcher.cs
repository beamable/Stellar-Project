using Farm.Helpers;
using Farm.Managers;
using UnityEngine;

namespace Farm.AreaSwitcher
{
    public class AreaSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject farmGrid;
        [SerializeField] private GameObject houseGrid;
        [SerializeField] private GameObject toHouseTrigger;
        [SerializeField] private GameObject toFarmTrigger;

        private void Start()
        {
            OnAreaChanged();
        }

        private void OnEnable()
        {
            NextAreaTrigger.OnAreaChanged += OnAreaChanged;
        }

        private void OnDisable()
        {
            NextAreaTrigger.OnAreaChanged -= OnAreaChanged;
        }

        private void OnAreaChanged(GameConstants.FarmState nextAreaState = GameConstants.FarmState.Farm)
        {
            switch (nextAreaState)
            {
                case GameConstants.FarmState.None:
                    break;
                case GameConstants.FarmState.Farm:
                    farmGrid.SetActive(true);
                    houseGrid.SetActive(false);
                    break;
                case GameConstants.FarmState.House:
                    farmGrid.SetActive(false);
                    houseGrid.SetActive(true);
                    break;
            }
            toHouseTrigger.SetActive(farmGrid.activeInHierarchy);
            toFarmTrigger.SetActive(houseGrid.activeInHierarchy);
            
            GameManager.Instance.SetFarmState(nextAreaState);
        }
    }
}
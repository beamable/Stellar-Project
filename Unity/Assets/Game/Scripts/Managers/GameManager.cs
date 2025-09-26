using Farm.Helpers;
using Unity.Cinemachine;
using UnityEngine;

namespace Farm.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private CinemachineVirtualCameraBase cineMachineCamera;

        private CinemachineConfiner2D _confiner2D;
        public GameConstants.FarmState FarmState { get; private set; }

        protected override void OnInitOnce()
        {
            base.OnInitOnce();
            _confiner2D = cineMachineCamera.GetComponent<CinemachineConfiner2D>();
        }

        public void SetFarmState(GameConstants.FarmState state)
        {
            FarmState = state;

            switch (state)
            {
                case GameConstants.FarmState.None:
                    break;
                case GameConstants.FarmState.Farm:
                    _confiner2D.enabled = true;
                    break;
                case GameConstants.FarmState.House:
                    _confiner2D.enabled = false;
                    break;
            }
        }
    }
}
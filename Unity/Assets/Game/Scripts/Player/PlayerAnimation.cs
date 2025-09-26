using UnityEngine;

namespace Farm.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int _ploughHash = Animator.StringToHash("Plough");
        private readonly int _waterHash = Animator.StringToHash("Water");
        private readonly int _basketHash = Animator.StringToHash("Basket");
        
        private PlayerController _playerController;
        
        public void InitAwake(PlayerController playerController)
        {
            _playerController = playerController;
        }
        
        public void SetIsWalking(bool isWalking)
        {
            animator.SetBool(_isWalkingHash, isWalking);
        }
        
        public void SetPloughTrigger()
        {
            animator.SetTrigger(_ploughHash);
        }
        
        public void SetWaterTrigger()
        {
            animator.SetTrigger(_waterHash);
        }
    }
}
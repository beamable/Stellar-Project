using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.UI
{
    public class Fader : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private Image fader;
        [SerializeField] private CanvasGroup faderCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        public void OnInit()
        {
            faderCanvasGroup.alpha = 1;
            FadeOut().Forget();
        }
        
        public async UniTask FadeIn()
        {
            UnityEngine.Time.timeScale = 0.75f;
            Tween t = fader.DOFade(1, fadeDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                UnityEngine.Time.timeScale = 1;
            });
            await t.AsyncWaitForCompletion();
            await UniTask.Yield();
        }
        
        public async UniTask FadeOut()
        {
            UnityEngine.Time.timeScale = 0.75f;
            Tween t = fader.DOFade(0, fadeDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                UnityEngine.Time.timeScale = 1;
            });
            await t.AsyncWaitForCompletion();
            await UniTask.Yield();
        }
    }
}
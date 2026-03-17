using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class ItemsCircleRotator : MonoBehaviour
    {
        [SerializeField] private GameObject itemsCircle;
        [SerializeField] private float rotationDuration = 0.5f;
        private Tween currentTween;

        public void OnPrevious(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            TryRotate(-90f);
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            TryRotate(90f);
        }

        private void TryRotate(float delta)
        {
            if (currentTween != null && currentTween.IsActive() && currentTween.IsPlaying()) return;

            float startZ = itemsCircle.transform.rotation.eulerAngles.z;
            float targetZ = startZ + delta;

            currentTween = itemsCircle.transform
                .DORotate(new Vector3(0, 0, targetZ), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack)
                .OnComplete(() => currentTween = null);
        }

        private void OnDisable()
        {
            currentTween?.Kill();
            currentTween = null;
        }
    }
}
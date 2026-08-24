using DG.Tweening;
using Game.Systems;
using UnityEngine;

namespace Game.Player
{
    public class CharacterExpressions : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer characterSpriteRenderer;
        [SerializeField] private CanvasGroup characterCanvasGroup; // For fade effects
        [SerializeField] private Transform characterMesh;
        
        [SerializeField] private HungerSystem hungerSystem;
        [SerializeField] private AirQualitySystem airQualitySystem;

        [Header("Colors")]
        [SerializeField] private Color happyColor = Color.white; // Normal
        [SerializeField] private Color tirredColor = new Color(0.8f, 0.8f, 0.8f); // Desaturated
        [SerializeField] private Color sadColor = new Color(0.6f, 0.5f, 0.5f); // Reddish, tired

        [Header("Scale")]
        [SerializeField] private float happyScale = 1f;
        [SerializeField] private float tiredScale = 0.95f;

        private Tween colorTween;
        private Tween scaleTween;
        private Tween breatheTween;

        private void OnEnable()
        {
            if (hungerSystem)
                hungerSystem.onHungerChanged.AddListener(UpdateExpression);
            if (airQualitySystem)
                airQualitySystem.onAirQualityChanged.AddListener(UpdateExpression);
        }

        private void OnDisable()
        {
            if (hungerSystem)
                hungerSystem.onHungerChanged.RemoveListener(UpdateExpression);
            if (airQualitySystem)
                airQualitySystem.onAirQualityChanged.RemoveListener(UpdateExpression);

            colorTween?.Kill();
            scaleTween?.Kill();
            breatheTween?.Kill();
        }

        private void UpdateExpression(float _)
        {
            if (!hungerSystem || !airQualitySystem) return;

            float hungerPercent = hungerSystem.HungerPercent;
            float airPercent = airQualitySystem.AirQualityPercent;

            // Determine emotional state
            Color targetColor = happyColor;
            float targetScale = happyScale;
            bool shouldBreathHeavy = false;

            if (hungerPercent < 0.3f)
            {
                // Very hungry - sad face
                targetColor = sadColor;
                targetScale = tiredScale;
                shouldBreathHeavy = true;
            }
            else if (hungerPercent < 0.6f)
            {
                // Moderately hungry - tired
                targetColor = Color.Lerp(happyColor, tirredColor, 0.5f);
                targetScale = Mathf.Lerp(happyScale, tiredScale, 0.3f);
            }

            // Air quality affects brightness
            if (airPercent < 0.3f)
            {
                // Very polluted - darken and sadden
                targetColor = Color.Lerp(targetColor, sadColor, 0.4f);
                shouldBreathHeavy = true;
            }

            // Animate color change
            colorTween?.Kill();
            if (characterSpriteRenderer)
                colorTween = characterSpriteRenderer.DOColor(targetColor, 0.5f).SetEase(Ease.InOutQuad);

            // Animate scale change
            scaleTween?.Kill();
            if (characterMesh)
                scaleTween = characterMesh.DOScale(targetScale, 0.5f).SetEase(Ease.InOutQuad);

            // Heavy breathing when hungry or in bad air
            if (shouldBreathHeavy)
                StartHeavyBreathing();
            else
                StopHeavyBreathing();
        }

        private void StartHeavyBreathing()
        {
            breatheTween?.Kill();
            if (!characterMesh) return;

            Vector3 baseScale = Vector3.one;
            breatheTween = DOTween.Sequence()
                .Append(characterMesh.DOScale(baseScale * 1.02f, 0.3f).SetEase(Ease.InOutQuad))
                .Append(characterMesh.DOScale(baseScale * 0.98f, 0.3f).SetEase(Ease.InOutQuad))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopHeavyBreathing()
        {
            breatheTween?.Kill();
        }

        private void OnDestroy()
        {
            colorTween?.Kill();
            scaleTween?.Kill();
            breatheTween?.Kill();
        }
    }
}

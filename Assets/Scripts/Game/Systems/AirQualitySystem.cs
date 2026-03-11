using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Systems
{
    public class AirQualitySystem : MonoBehaviour
    {
        [SerializeField] private float maxAirQuality = 100f;
        [SerializeField] private float naturalRecoveryRate = 0.05f; // per second
        
        [Header("Visual")]
        [SerializeField] private Light mainLight;
        [SerializeField] private Color cleanSkyColor = new Color(0.5f, 0.8f, 1f); // Light blue
        [SerializeField] private Color pollutedSkyColor = new Color(0.4f, 0.4f, 0.4f); // Grey

        [Header("Particle Effects")]
        [SerializeField] private GameObject smokeParticlePrefab;

        public float CurrentAirQuality { get; private set; }
        public float AirQualityPercent => CurrentAirQuality / maxAirQuality;

        [FormerlySerializedAs("OnAirQualityChanged")] public UnityEvent<float> onAirQualityChanged = new(); // sends AirQualityPercent

        private GameObject smokeInstance;

        private void Start()
        {
            CurrentAirQuality = maxAirQuality; // Start at 100%
            onAirQualityChanged?.Invoke(AirQualityPercent);
        }

        private void Update()
        {
            // Natural recovery when not cooking
            if (CurrentAirQuality < maxAirQuality)
            {
                CurrentAirQuality = Mathf.Min(
                    CurrentAirQuality + naturalRecoveryRate * Time.deltaTime,
                    maxAirQuality
                );
                onAirQualityChanged?.Invoke(AirQualityPercent);
            }

            UpdateSkyColor();
        }

        public void AddPollution(float amount)
        {
            CurrentAirQuality = Mathf.Max(CurrentAirQuality - amount, 0);
            onAirQualityChanged?.Invoke(AirQualityPercent);
            Debug.Log($"Pollution added: {amount}. Air quality now: {CurrentAirQuality:F1}%");
        }

        public void AddCleanAir(float amount)
        {
            CurrentAirQuality = Mathf.Min(CurrentAirQuality + amount, maxAirQuality);
            onAirQualityChanged?.Invoke(AirQualityPercent);
        }

        private void UpdateSkyColor()
        {
            float t = AirQualityPercent; // 0 = polluted, 1 = clean
            Color newSkyColor = Color.Lerp(pollutedSkyColor, cleanSkyColor, t);
            RenderSettings.ambientLight = newSkyColor;
            RenderSettings.ambientSkyColor = newSkyColor;
            
            // Adjust light intensity based on air quality
            if (mainLight)
                mainLight.intensity = Mathf.Lerp(0.5f, 1.2f, t);
        }

        public bool IsPolluted => CurrentAirQuality < 50f;
        public bool IsVeryPolluted => CurrentAirQuality < 25f;
    }
}
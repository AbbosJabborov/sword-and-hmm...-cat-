using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Systems
{
    public class AirQualitySystem : MonoBehaviour
    {
        [SerializeField] private float maxAirQuality = 100f;
        [SerializeField] private float naturalRecoveryRate = 0.05f; // per second
        
        [Header("Visual - Sky")]
        [SerializeField] private Light mainLight;
        [SerializeField] private Color cleanSkyColor = new Color(0.5f, 0.8f, 1f); // Light blue
        [SerializeField] private Color pollutedSkyColor = new Color(0.4f, 0.4f, 0.4f); // Grey

        [Header("Visual - Fog")]
        [SerializeField] private float maxFogDensity = 0.3f; // Max density when completely polluted
        [SerializeField] private float fogUpdateSmoothing = 5f; // Smoothness of fog transition

        [Header("Particle Effects")]
        [SerializeField] private GameObject smokeParticlePrefab;

        public float CurrentAirQuality { get; private set; }
        public float AirQualityPercent => CurrentAirQuality / maxAirQuality;

        [FormerlySerializedAs("OnAirQualityChanged")] public UnityEvent<float> onAirQualityChanged = new();

        private GameObject smokeInstance;
        private float currentFogDensity = 0f;

        private void Start()
        {
            CurrentAirQuality = maxAirQuality;
            currentFogDensity = 0f;
            
            // Enable fog in scene
            RenderSettings.fog = true;
            
            onAirQualityChanged?.Invoke(AirQualityPercent);
            
            Debug.Log("[AIRQUALITY] System initialized. Fog enabled in RenderSettings.");
        }

        private void Update()
        {
            // Natural recovery
            if (CurrentAirQuality < maxAirQuality)
            {
                CurrentAirQuality = Mathf.Min(
                    CurrentAirQuality + naturalRecoveryRate * Time.deltaTime,
                    maxAirQuality
                );
                onAirQualityChanged?.Invoke(AirQualityPercent);
            }

            UpdateEnvironment();
        }

        public void AddPollution(float amount)
        {
            CurrentAirQuality = Mathf.Max(CurrentAirQuality - amount, 0);
            onAirQualityChanged?.Invoke(AirQualityPercent);
            Debug.Log($"[AIRQUALITY] Pollution added: {amount}. Air quality now: {CurrentAirQuality:F1} ({AirQualityPercent * 100:F1}%)");
        }

        public void AddCleanAir(float amount)
        {
            CurrentAirQuality = Mathf.Min(CurrentAirQuality + amount, maxAirQuality);
            onAirQualityChanged?.Invoke(AirQualityPercent);
            Debug.Log($"[AIRQUALITY] Clean air added: {amount}. Air quality now: {CurrentAirQuality:F1} ({AirQualityPercent * 100:F1}%)");
        }

        private void UpdateEnvironment()
        {
            UpdateSkyColor();
            UpdateFogDensity();
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

        private void UpdateFogDensity()
        {
            // Calculate target fog density (inverse of air quality)
            // Clean (1.0) = 0 fog, Polluted (0.0) = max fog
            float targetFogDensity = (1f - AirQualityPercent) * maxFogDensity;

            // Smooth transition to target density
            currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, Time.deltaTime * fogUpdateSmoothing);

            // Apply to RenderSettings
            RenderSettings.fogDensity = currentFogDensity;
            
            Debug.Log($"[AIRQUALITY] Fog density: {currentFogDensity:F3}");
        }

        public bool IsPolluted => CurrentAirQuality < 50f;
        public bool IsVeryPolluted => CurrentAirQuality < 25f;
    }
}
using Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AirQualityUI : MonoBehaviour
    {
        [SerializeField] private AirQualitySystem airQualitySystem;
        [SerializeField] private Image airQualityBar;
        [SerializeField] private TMP_Text airQualityText;

        private void OnEnable()
        {
            if (airQualitySystem)
                airQualitySystem.onAirQualityChanged.AddListener(UpdateAirQualityUI);
        }

        private void OnDisable()
        {
            if (airQualitySystem)
                airQualitySystem.onAirQualityChanged.RemoveListener(UpdateAirQualityUI);
        }

        private void UpdateAirQualityUI(float airQualityPercent)
        {
            if (airQualityBar)
                airQualityBar.fillAmount = airQualityPercent;

            if (airQualityText)
                airQualityText.text = $"{airQualityPercent * 100:F0}%";
        }
    }
}
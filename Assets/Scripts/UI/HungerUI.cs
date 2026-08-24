using Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HungerUI : MonoBehaviour
    {
        [SerializeField] private HungerSystem hungerSystem;
        [SerializeField] private Image hungerBar;
        [SerializeField] private TMP_Text hungerText; // Optional: show percentage

        private void OnEnable()
        {
            if (hungerSystem)
                hungerSystem.onHungerChanged.AddListener(UpdateHungerUI);
        }

        private void OnDisable()
        {
            if (hungerSystem)
                hungerSystem.onHungerChanged.RemoveListener(UpdateHungerUI);
        }

        private void UpdateHungerUI(float hungerPercent)
        {
            if (hungerBar)
                hungerBar.fillAmount = hungerPercent;

            if (hungerText)
                hungerText.text = $"{hungerPercent * 100:F0}%";
        }
    }
}
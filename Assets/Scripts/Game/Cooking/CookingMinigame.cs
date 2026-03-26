using Game.Inventory;
using Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Game.Cooking
{
    public class CookingMinigame : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] public Slider indicatorSlider;
        [SerializeField] public RectTransform goodZoneRect; 
        [SerializeField] public RectTransform perfectZoneRect;
        [SerializeField] public TextMeshProUGUI resultText;
        [SerializeField] public RectTransform spatulaHandle;
        [SerializeField] public CanvasGroup canvasGroup;

        [Header("Game Settings")]
        [SerializeField] public float moveSpeed = 1.2f;
        
        private float goodZoneMin, goodZoneMax, perfectZoneMin, perfectZoneMax;
        private float currentValue = 0f;
        private int direction = 1; 
        private bool isCooking = false;

        private string _foodName = "";
        private float _baseEnergy = 0f;
        private PlayerInventory _inventory;
        private HungerSystem _hungerSystem;

        private void Start()
        {
            if (canvasGroup)
                canvasGroup.alpha = 0f; // Hidden initially
        }

        private void Update()
        {
            if (!isCooking) return;

            // Move slider
            currentValue += direction * moveSpeed * Time.deltaTime;
            if (currentValue >= 1f) { currentValue = 1f; direction = -1; }
            else if (currentValue <= 0f) { currentValue = 0f; direction = 1; }

            indicatorSlider.value = currentValue;
        }

        // Called from PlayerInputHandler when Space is pressed
        public void OnSpacePressed()
        {
            if (isCooking)
            {
                StopAndEvaluate();
                Debug.Log("[COOKING] Space pressed - evaluating result");
            }
        }

        public void StartCooking(string foodName, float baseEnergy, 
                                 PlayerInventory inventory, HungerSystem hungerSystem)
        {
            _foodName = foodName;
            _baseEnergy = baseEnergy;
            _inventory = inventory;
            _hungerSystem = hungerSystem;

            GenerateRandomZones();
            currentValue = 0f;
            direction = 1;
            isCooking = true;

            // Reset UI
            resultText.DOKill();
            resultText.transform.localScale = Vector3.one;
            resultText.text = "READY... COOK!";
            resultText.color = Color.white;

            // Show canvas
            if (canvasGroup)
                canvasGroup.DOFade(1f, 0.3f);

            Debug.Log($"[COOKING] Started cooking {foodName} (base energy: {baseEnergy})");
        }

        private void GenerateRandomZones()
        {
            float goodWidth = Random.Range(0.2f, 0.4f); 
            goodZoneMin = Random.Range(0.05f, 0.95f - goodWidth); 
            goodZoneMax = goodZoneMin + goodWidth;

            float perfectWidth = Random.Range(0.05f, 0.15f); 
            perfectWidth = Mathf.Min(perfectWidth, goodWidth * 0.8f); 

            perfectZoneMin = Random.Range(goodZoneMin + 0.02f, goodZoneMax - perfectWidth - 0.02f);
            perfectZoneMax = perfectZoneMin + perfectWidth;

            goodZoneRect.anchorMin = new Vector2(goodZoneMin, 0);
            goodZoneRect.anchorMax = new Vector2(goodZoneMax, 1);
            perfectZoneRect.anchorMin = new Vector2(perfectZoneMin, 0);
            perfectZoneRect.anchorMax = new Vector2(perfectZoneMax, 1);

            Debug.Log($"[COOKING] Zones generated - Good: {goodZoneMin:F2}-{goodZoneMax:F2}, Perfect: {perfectZoneMin:F2}-{perfectZoneMax:F2}");
        }

        private void StopAndEvaluate()
        {
            if (!isCooking) return;

            isCooking = false;
            resultText.DOKill();

            float finalEnergy = 0f;
            string resultMsg = "";
            Color resultColor = Color.white;
            bool isPerfect = false;

            if (currentValue >= perfectZoneMin && currentValue <= perfectZoneMax)
            {
                finalEnergy = _baseEnergy * 3f;
                resultMsg = "PERFECT!";
                resultColor = Color.cyan;
                isPerfect = true;
                Debug.Log($"[COOKING] PERFECT! {_foodName} × 3 = {finalEnergy} energy");
                ApplyJuice(resultMsg, resultColor, isPerfect);
            }
            else if (currentValue >= goodZoneMin && currentValue <= goodZoneMax)
            {
                finalEnergy = _baseEnergy * 1.5f;
                resultMsg = "GOOD!";
                resultColor = Color.yellow;
                Debug.Log($"[COOKING] GOOD! {_foodName} × 1.5 = {finalEnergy} energy");
                ApplyJuice(resultMsg, resultColor, false);
            }
            else
            {
                finalEnergy = 0f;
                resultMsg = "BURNT!";
                resultColor = Color.red;
                Debug.Log($"[COOKING] BURNT! {_foodName} × 0 = 0 energy (wasted)");
                ApplyJuice(resultMsg, resultColor, false, true);
            }

            // Apply to hunger system
            if (_hungerSystem)
            {
                _hungerSystem.EatCooked(_foodName, finalEnergy);
            }
            else
            {
                Debug.LogError("[COOKING] HungerSystem reference is null!");
            }

            // Hide UI after result
            if (canvasGroup)
                canvasGroup.DOFade(0f, 0.5f).SetDelay(2f);
        }

        private void ApplyJuice(string msg, Color col, bool isPerfect, bool isBurnt = false)
        {
            resultText.text = msg + $"\n<size=60%>+{_baseEnergy * (isPerfect ? 3f : isBurnt ? 0f : 1.5f):F0} hunger</size>";
            resultText.color = col;

            if (isBurnt)
            {
                resultText.rectTransform.DOShakePosition(0.5f, 20f, 20);
                if (spatulaHandle) 
                    spatulaHandle.DOShakePosition(0.5f, 10f, 15);
            }
            else if (isPerfect)
            {
                resultText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 10, 1f);
            }
            else
            {
                resultText.transform.DOScale(1.2f, 0.1f).OnComplete(() => resultText.transform.DOScale(1f, 0.1f));
            }
        }

        public bool IsActive => isCooking;
    }
}
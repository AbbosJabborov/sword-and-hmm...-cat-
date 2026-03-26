using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Import DOTween

public class CookingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public Slider indicatorSlider;
    public RectTransform goodZoneRect; 
    public RectTransform perfectZoneRect;
    public TextMeshProUGUI resultText;
    public RectTransform spatulaHandle; // Optional: Drag the slider handle here to shake it too!

    [Header("Game Settings")]
    public float moveSpeed = 1.2f; 
    
    private float goodZoneMin, goodZoneMax, perfectZoneMin, perfectZoneMax;
    private float currentValue = 0f;
    private int direction = 1; 
    private bool isCooking = false;

    void Start()
    {
        StartCooking(); 
    }

    public void StartCooking()
    {
        GenerateRandomZones();
        currentValue = 0f;
        direction = 1;
        isCooking = true;
        
        // Reset Text and Scale
        resultText.DOKill(); // Always kill active tweens before starting new ones
        resultText.transform.localScale = Vector3.one;
        resultText.text = "READY... COOK!";
        resultText.color = Color.white;
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCooking) StopAndEvaluate();
            else StartCooking();
        }

        if (!isCooking) return;

        currentValue += direction * moveSpeed * Time.deltaTime;
        if (currentValue >= 1f) { currentValue = 1f; direction = -1; }
        else if (currentValue <= 0f) { currentValue = 0f; direction = 1; }

        indicatorSlider.value = currentValue;
    }

    private void StopAndEvaluate()
    {
        isCooking = false;
        resultText.DOKill(); // Stop any "Ready... Cook" tweens

        if (currentValue >= perfectZoneMin && currentValue <= perfectZoneMax)
        {
            ApplyJuice("PERFECT!", Color.cyan, true);
        }
        else if (currentValue >= goodZoneMin && currentValue <= goodZoneMax)
        {
            ApplyJuice("GOOD!", Color.yellow, false);
        }
        else
        {
            ApplyJuice("BURNT!", Color.red, false, true);
        }
    }

    private void ApplyJuice(string msg, Color col, bool isPerfect, bool isBurnt = false)
    {
        resultText.text = msg + "\n<size=60%>Space to Retry</size>";
        resultText.color = col;

        if (isBurnt)
        {
            // Shake effect for burning
            resultText.rectTransform.DOShakePosition(0.5f, 20f, 20);
            if(spatulaHandle) spatulaHandle.DOShakePosition(0.5f, 10f, 15);
        }
        else if (isPerfect)
        {
            // Big punch for Perfect
            resultText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 10, 1f);
        }
        else
        {
            // Small scale pop for Good
            resultText.transform.DOScale(1.2f, 0.1f).OnComplete(() => resultText.transform.DOScale(1f, 0.1f));
        }
    }
}

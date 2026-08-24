using Game;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalPage : MonoBehaviour
{
    [SerializeField] private Image itemIllustration;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI pollutionText;
    [SerializeField] private TextMeshProUGUI notesText;
    [SerializeField] private Sprite silhouetteSprite; // Grey placeholder
    
    private ItemDiscoveryTracker.ItemDiscoveryData currentData;
    
    public void SetItemData(ItemDiscoveryTracker.ItemDiscoveryData data)
    {
        currentData = data;
        Refresh();
    }
    
    public void Refresh()
    {
        if (currentData == null) return;
        
        // Item name (always shown)
        itemName.text = currentData.itemName;
        
        // Illustration (silhouette until eaten)
        if (currentData.hasEaten && currentData.illustrationSprite != null)
        {
            itemIllustration.sprite = currentData.illustrationSprite;
        }
        else
        {
            itemIllustration.sprite = silhouetteSprite; // Grey placeholder
        }
        
        // Energy value (only if eaten)
        if (currentData.hasEaten)
        {
            energyText.text = "Energy: 8"; // Pull from ItemData
            energyText.gameObject.SetActive(true);
        }
        else
        {
            energyText.gameObject.SetActive(false);
        }
        
        // Pollution value (only if cooked)
        if (currentData.hasCooked)
        {
            pollutionText.text = "Pollution (perfect): +1.0"; // Pull from ItemData
            pollutionText.gameObject.SetActive(true);
        }
        else
        {
            pollutionText.gameObject.SetActive(false);
        }
        
        // Notes area (always available, but grey if not eaten)
        if (currentData.hasEaten)
        {
            notesText.color = Color.black;
        }
        else
        {
            notesText.color = new Color(0.5f, 0.5f, 0.5f); // Grey
        }
    }
}
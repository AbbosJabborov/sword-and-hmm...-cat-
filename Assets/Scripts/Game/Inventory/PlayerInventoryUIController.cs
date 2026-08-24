using DG.Tweening;
using Game.Player;
using Game.Player.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inventory
{
    public class PlayerInventoryUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform inventoryPanel;
        [SerializeField] private float hiddenX = -200f;
        [SerializeField] private float visibleX = 0f;
        [SerializeField] private float tweenTime = 0.25f;

        private bool inventoryVisible;

        private void Start()
        {
            inventoryPanel.anchoredPosition = new Vector2(hiddenX, inventoryPanel.anchoredPosition.y);
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.started)
                ShowInventory();
            else if (context.canceled)
                HideInventory();
        }

        private void ShowInventory()
        {
            if (inventoryVisible) return;
            inventoryVisible = true;

            // Slide in
            inventoryPanel.DOAnchorPosX(visibleX, tweenTime).SetEase(Ease.OutCubic);
            
        }

        private void HideInventory()
        {
            if (!inventoryVisible) return;
            inventoryVisible = false;

            // Slide out
            inventoryPanel.DOAnchorPosX(hiddenX, tweenTime).SetEase(Ease.InCubic);
            
        }
    }
}
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

        [Header("Player References")]
        [SerializeField] private PlayerLook playerLook;

        private bool _inventoryVisible;

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
            if (_inventoryVisible) return;
            _inventoryVisible = true;

            // Slide in
            inventoryPanel.DOAnchorPosX(visibleX, tweenTime).SetEase(Ease.OutCubic);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerLook != null)
                playerLook.enabled = false;
        }

        private void HideInventory()
        {
            if (!_inventoryVisible) return;
            _inventoryVisible = false;

            // Slide out
            inventoryPanel.DOAnchorPosX(hiddenX, tweenTime).SetEase(Ease.InCubic);

            // Relock cursor and re-enable look
            if (playerLook != null)
                playerLook.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
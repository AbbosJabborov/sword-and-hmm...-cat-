using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Core;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Journal book system
    /// - Opens/closes with [I] / Gamepad Y button
    /// - Navigate pages with [Q]/[E] or [L1]/[R1]
    /// - Uses New Input System for input handling
    /// - Uses GameStateManager to notify other systems when journal opens/closes
    /// 
    /// Input is handled by PlayerInputHandler.OnOpenJournal(),
    /// OnCycleLeft(), OnCycleRight() which call this script's methods
    /// </summary>
    public class JournalBook : MonoBehaviour
    {
        [SerializeField] private float pageSpeed = 0.5f;
        [SerializeField] private List<Transform> pages;
        [SerializeField] private ItemDiscoveryTracker discoveryTracker;
        [SerializeField] private CanvasGroup journalCanvasGroup;
        
        private int currentPageIndex = -1;
        private bool isRotating = false;
        private bool journalOpen = false;
        
        private void Start()
        {
            InitializePages();
            PopulatePages();
            CloseJournal();
        }
        
        /// <summary>
        /// Initialize all pages to default state
        /// </summary>
        private void InitializePages()
        {
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].transform.rotation = Quaternion.identity;
            }
            pages[0].SetAsLastSibling();
        }
        
        /// <summary>
        /// Populate pages with discovered items from ItemDiscoveryTracker
        /// </summary>
        private void PopulatePages()
        {
            List<ItemDiscoveryTracker.ItemDiscoveryData> discoveries = 
                discoveryTracker.GetDiscoveredItems();
            
            for (int i = 0; i < discoveries.Count && i < pages.Count; i++)
            {
                JournalPage page = pages[i].GetComponent<JournalPage>();
                if (page != null)
                {
                    page.SetItemData(discoveries[i]);
                }
            }
        }
        
        /// <summary>
        /// Toggle journal open/closed
        /// Called from PlayerInputHandler.OnOpenJournal()
        /// </summary>
        public void ToggleJournal()
        {
            if (journalOpen)
            {
                CloseJournal();
            }
            else
            {
                OpenJournal();
            }
        }
        
        /// <summary>
        /// Open journal
        /// Shows UI, resets to first page, blocks gameplay inputs
        /// </summary>
        private void OpenJournal()
        {
            journalOpen = true;
            
            // Show journal UI
            journalCanvasGroup.alpha = 1f;
            journalCanvasGroup.interactable = true;
            journalCanvasGroup.blocksRaycasts = true;
            
            // Reset to first page
            currentPageIndex = -1;
            RotateForward();
            
            Debug.Log("[JOURNAL] Opened");
        }
        
        /// <summary>
        /// Close journal
        /// Hides UI, allows gameplay inputs
        /// </summary>
        private void CloseJournal()
        {
            journalOpen = false;
            
            // Hide journal UI
            journalCanvasGroup.alpha = 0f;
            journalCanvasGroup.interactable = false;
            journalCanvasGroup.blocksRaycasts = false;
            
            Debug.Log("[JOURNAL] Closed");
        }
        
        /// <summary>
        /// Navigate to next page (right/forward)
        /// Called from PlayerInputHandler.OnCycleRight() when journal open
        /// </summary>
        public void RotateForward()
        {
            // Can't rotate while already rotating
            if (isRotating) return;
            
            // Can't go past last page
            if (currentPageIndex >= pages.Count - 1) return;
            
            currentPageIndex++;
            float targetAngle = 180; // Rotate 180 degrees forward
            pages[currentPageIndex].SetAsLastSibling();
            
            StartCoroutine(RotatePage(targetAngle, true));
        }
        
        /// <summary>
        /// Navigate to previous page (left/back)
        /// Called from PlayerInputHandler.OnCycleLeft() when journal open
        /// </summary>
        public void RotateBack()
        {
            // Can't rotate while already rotating
            if (isRotating) return;
            
            // Can't go before first page
            if (currentPageIndex <= 0) return;
            
            float targetAngle = 0; // Rotate back to 0 degrees
            pages[currentPageIndex].SetAsLastSibling();
            
            StartCoroutine(RotatePage(targetAngle, false));
        }
        
        /// <summary>
        /// Smoothly rotate page from current rotation to target rotation
        /// </summary>
        private IEnumerator RotatePage(float targetAngle, bool isMovingForward)
        {
            isRotating = true;
            float elapsedTime = 0f;
            
            while (true)
            {
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                elapsedTime += Time.deltaTime * pageSpeed;
                
                pages[currentPageIndex].rotation = Quaternion.Slerp(
                    pages[currentPageIndex].rotation,
                    targetRotation,
                    elapsedTime
                );
                
                // Check if we've reached the target rotation
                float angleDifference = Quaternion.Angle(pages[currentPageIndex].rotation, targetRotation);
                
                if (angleDifference < 0.1f)
                {
                    // Snap to exact target rotation
                    pages[currentPageIndex].rotation = targetRotation;
                    
                    // Update page index if moving backward
                    if (!isMovingForward)
                    {
                        currentPageIndex--;
                    }
                    
                    isRotating = false;
                    RefreshCurrentPage();
                    break;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Refresh the current page display
        /// Called after rotation completes
        /// </summary>
        private void RefreshCurrentPage()
        {
            if (currentPageIndex >= 0 && currentPageIndex < pages.Count)
            {
                JournalPage page = pages[currentPageIndex].GetComponent<JournalPage>();
                if (page != null)
                {
                    page.Refresh();
                }
            }
        }
        
        /// <summary>
        /// Called when a new item is discovered in the game
        /// Updates journal to show new entry
        /// </summary>
        public void OnItemDiscovered()
        {
            PopulatePages();
        }
        
        /// <summary>
        /// Called when item is eaten or cooked
        /// Refreshes current page to show updated information
        /// </summary>
        public void OnItemUpdated()
        {
            RefreshCurrentPage();
        }
        
        // ==================== PUBLIC ACCESSORS ====================
        
        public bool IsJournalOpen => journalOpen;
        public bool IsRotating => isRotating;
        public int CurrentPageIndex => currentPageIndex;
    }
}
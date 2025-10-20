using UI.Future_Expansions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Interaction
{
    public class Interact : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactRange = 2f;
        public LayerMask interactableLayer;
    
        [Header("References")]
        [SerializeField] private ImageFiller imageFiller;

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
        
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
            foreach (var hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(gameObject);
                    imageFiller.FillImage();
                    break;
                }
            }
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
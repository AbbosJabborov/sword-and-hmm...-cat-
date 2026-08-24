using UnityEngine;

namespace Game.Interaction
{
    public interface IInteractable
    {
        void Interact(GameObject interactor);
    }
}
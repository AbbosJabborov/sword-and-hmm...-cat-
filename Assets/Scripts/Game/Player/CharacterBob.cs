using DG.Tweening;
using UnityEngine;

namespace Game.Player
{
    public class CharacterBob : MonoBehaviour
    {
        [Header("Bob Settings")]
        [SerializeField] private float bobHeight = 0.15f; // How far up/down to bob
        [SerializeField] private float bobSpeed = 1.2f; // Duration of one bob cycle
        [SerializeField] private float idleBobHeight = 0.1f; // Smaller bob when idle
        [SerializeField] private float idleBobSpeed = 1.5f; // Slower when idle

        [Header("References")]
        [SerializeField] private Transform characterMesh; // The visual character (not the controller)
        [SerializeField] private PlayerMovement playerMovement;

        private Vector3 startPosition;
        private Sequence bobSequence;
        private bool isMoving = false;

        private void Start()
        {
            if (!characterMesh)
                characterMesh = transform; // Use self if no mesh specified

            startPosition = characterMesh.localPosition;
            StartIdleBob();
        }

        private void Update()
        {
            // Check if moving
            bool shouldMove = playerMovement != null && playerMovement.CurrentVelocity.magnitude > 0.1f;

            if (shouldMove && !isMoving)
            {
                isMoving = true;
                StartMovingBob();
            }
            else if (!shouldMove && isMoving)
            {
                isMoving = false;
                StartIdleBob();
            }
        }

        private void StartIdleBob()
        {
            bobSequence?.Kill();
            bobSequence = DOTween.Sequence();

            bobSequence
                .Append(characterMesh.DOLocalMoveY(startPosition.y + idleBobHeight, idleBobSpeed / 2f).SetEase(Ease.InOutSine))
                .Append(characterMesh.DOLocalMoveY(startPosition.y - idleBobHeight * 0.5f, idleBobSpeed / 2f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StartMovingBob()
        {
            bobSequence?.Kill();
            bobSequence = DOTween.Sequence();

            bobSequence
                .Append(characterMesh.DOLocalMoveY(startPosition.y + bobHeight, bobSpeed / 2f).SetEase(Ease.InOutQuad))
                .Append(characterMesh.DOLocalMoveY(startPosition.y - bobHeight * 0.3f, bobSpeed / 2f).SetEase(Ease.InOutQuad))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            bobSequence?.Kill();
        }
    }
}

using DG.Tweening;
using UnityEngine;

namespace Game.Player
{
    public class BasketBounce : MonoBehaviour
    {
        [Header("Basket Bob Settings")]
        [SerializeField] private Transform basket; // The basket object
        [SerializeField] private float bounceHeight = 0.1f; // Basket bounces more than body
        [SerializeField] private float bounceSpeed = 0.8f; // Basket bounces faster than body
        [SerializeField] private float bounceRotation = 5f; // Slight tilt rotation
        
        [Header("References")]
        [SerializeField] private PlayerMovement playerMovement;

        private Vector3 basketStartPos;
        private Quaternion basketStartRot;
        private Sequence bounceSequence;
        private bool isMoving = false;

        private void Start()
        {
            if (!basket)
            {
                Debug.LogWarning("BasketBounce: No basket assigned!");
                enabled = false;
                return;
            }

            basketStartPos = basket.localPosition;
            basketStartRot = basket.localRotation;
            StartIdleSway();
        }

        private void Update()
        {
            bool shouldMove = playerMovement != null && playerMovement.CurrentVelocity.magnitude > 0.1f;

            if (shouldMove && !isMoving)
            {
                isMoving = true;
                StartBouncingBounce();
            }
            else if (!shouldMove && isMoving)
            {
                isMoving = false;
                StartIdleSway();
            }
        }

        private void StartIdleSway()
        {
            bounceSequence?.Kill();
            bounceSequence = DOTween.Sequence();

            // Gentle side-to-side sway when idle
            bounceSequence
                .Join(basket.DOLocalRotateQuaternion(
                    basketStartRot * Quaternion.Euler(0, 0, bounceRotation * 0.3f),
                    0.6f).SetEase(Ease.InOutSine))
                .Join(basket.DOLocalMoveY(basketStartPos.y + bounceHeight * 0.2f, 0.8f).SetEase(Ease.InOutSine))
                .AppendCallback(() => {
                    // Sway other direction
                    bounceSequence
                        .Join(basket.DOLocalRotateQuaternion(
                            basketStartRot * Quaternion.Euler(0, 0, -bounceRotation * 0.3f),
                            0.6f).SetEase(Ease.InOutSine))
                        .Join(basket.DOLocalMoveY(basketStartPos.y - bounceHeight * 0.1f, 0.8f).SetEase(Ease.InOutSine));
                })
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StartBouncingBounce()
        {
            bounceSequence?.Kill();
            bounceSequence = DOTween.Sequence();

            // Energetic bouncing when moving
            bounceSequence
                .Append(basket.DOLocalMoveY(basketStartPos.y + bounceHeight, bounceSpeed / 2f)
                    .SetEase(Ease.OutQuad))
                .Join(basket.DOLocalRotateQuaternion(
                    basketStartRot * Quaternion.Euler(bounceRotation * 0.5f, 0, bounceRotation),
                    bounceSpeed / 2f).SetEase(Ease.OutQuad))
                
                .Append(basket.DOLocalMoveY(basketStartPos.y - bounceHeight * 0.4f, bounceSpeed / 2f)
                    .SetEase(Ease.InQuad))
                .Join(basket.DOLocalRotateQuaternion(
                    basketStartRot * Quaternion.Euler(-bounceRotation * 0.3f, 0, -bounceRotation * 0.5f),
                    bounceSpeed / 2f).SetEase(Ease.InQuad))
                
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            bounceSequence?.Kill();
        }
    }
}

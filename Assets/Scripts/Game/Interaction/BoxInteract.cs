using UnityEngine;
using DG.Tweening;
using Game.Interaction;

public class BoxInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform stairs;
    [SerializeField] private int upHeight = 10;

    private bool _isCalled;

    private void Start()
    {
        transform.DOLocalMoveY(transform.localPosition.y + 1f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public void Interact(GameObject interactor)
    {
        if (!_isCalled)
        {
            var targetPos = stairs.position + Vector3.up * upHeight;
            stairs.DOMove(targetPos, 5f).SetEase(Ease.InOutSine);
            _isCalled = true;
        }
        else
        {
            var targetPos = stairs.position - Vector3.up * upHeight;
            stairs.DOMove(targetPos, 5f).SetEase(Ease.InOutSine);
            _isCalled = false;
        }
    }
}
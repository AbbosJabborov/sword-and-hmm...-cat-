using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class WaypointMarker : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Camera cam;
    public RectTransform markerRoot;
    public Image icon;
    public TextMeshProUGUI distanceText;
    public CanvasGroup canvasGroup;

    [Header("Clamp Settings")]
    public Vector2 screenOffset = new Vector2(0, 50f);
    public float circularClampFactor = 0.4f;

    [Header("Fade Settings")]
    public float maxVisibleDistance = 50f;
    public float fadeDuration = 0.2f;

    [Header("Ping Animation")]
    public float pingInterval = 10f;
    public float pingScaleAmount = 1.2f;
    public float pingJumpAmount = 20f;
    public float pingDuration = 0.4f;

    private bool isClamped = false;
    private bool isVisible = true;

    void Start()
    {
        if (!cam)
            cam = Camera.main;
        if (!target || !cam) return;
        
        if (canvasGroup) canvasGroup.alpha = 1f;
        PlayPingLoop().Forget();
    }

    void LateUpdate()
    {
        if (!target || !cam) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);
        bool isBehind = screenPos.z < 0;

        if (isBehind) screenPos *= -1;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 fromCenter = (Vector2)screenPos - screenCenter;

        float maxRadius = Mathf.Min(Screen.width, Screen.height) * circularClampFactor;
        float dist = fromCenter.magnitude;

        isClamped = false;

        if (dist > maxRadius)
        {
            fromCenter = fromCenter.normalized * maxRadius;
            isClamped = true;
        }

        Vector2 finalPos = screenCenter + fromCenter + screenOffset;
        markerRoot.position = finalPos;

        if (distanceText)
        {
            distanceText.gameObject.SetActive(!isClamped);
            float worldDistance = Vector3.Distance(cam.transform.position, target.position);
            distanceText.text = $"{worldDistance:F0}m";
        }

        // fade based on distance and camera direction
        HandleVisibility();
    }
    public void HideMarker()
    {
        if (canvasGroup)
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => isVisible = false);
        else
            isVisible = false;
    }

    public void ShowMarker()
    {
        if (canvasGroup)
            canvasGroup.DOFade(1f, fadeDuration).OnComplete(() => isVisible = true);
        else
            isVisible = true;
    }

    private void HandleVisibility()
    {
        float worldDistance = Vector3.Distance(cam.transform.position, target.position);

        if (worldDistance < 5f)
        {
            HideMarker();
            return;
        }

        bool shouldDim = worldDistance > maxVisibleDistance || isClamped;
        float targetAlpha = shouldDim ? 0.75f : 1f;

        if (canvasGroup && !Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.DOFade(targetAlpha, fadeDuration);
            isVisible = targetAlpha >= 0.5f;
        }
    }


    private async UniTaskVoid PlayPingLoop()
    {
        await UniTask.Delay(1000);
        while (this != null && gameObject.activeInHierarchy)
        {
            await UniTask.Delay((int)(pingInterval * 1000));

            if (!isVisible) continue;
            // change it to if (!isVisible || canvasGroup.alpha < 0.9f) continue; if you want to avoid animation when marker clamped
            
            markerRoot.DOKill(true);

            Sequence s = DOTween.Sequence();
            s.Append(markerRoot.DOScale(pingScaleAmount, pingDuration / 2f).SetEase(Ease.OutQuad));
            s.Append(markerRoot.DOScale(1f, pingDuration / 2f).SetEase(Ease.InQuad));
            s.Join(markerRoot.DOAnchorPosY(markerRoot.anchoredPosition.y + pingJumpAmount, pingDuration / 2f).SetRelative().SetEase(Ease.OutQuad));
            s.Append(markerRoot.DOAnchorPosY(markerRoot.anchoredPosition.y - pingJumpAmount, pingDuration / 2f).SetRelative().SetEase(Ease.InQuad));
        }
    }
}

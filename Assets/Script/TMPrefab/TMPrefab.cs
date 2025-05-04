using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TMPrefab : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public bool isOpenedByHint;
    [SerializeField] private Sprite openedByHintBackGround;
    [SerializeField] private Sprite fixedBackground;
    [SerializeField] public Image backGround;
    [SerializeField] public Color color;
    public float moveAmount = 20f;
    public float duration = 0.5f;
    public Tween bounceTween;
    public void SetColor()
    {
        if (isOpenedByHint)
        {
            backGround.sprite = openedByHintBackGround;
        }
    }
    public void SetColorForText()
    {
        backGround.sprite = fixedBackground;
    }
    public void StartBounceEffect()
    {
        RectTransform rectTransform = textMesh.rectTransform;

        bounceTween = rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + moveAmount, duration)
             .SetLoops(-1, LoopType.Yoyo)
             .SetEase(Ease.InOutSine);
    }
    public void KillDoTween()
    {
        bounceTween.Kill(true);
        bounceTween = null;
        SetBackPosition();
    }
    public void SetBackPosition()
    {
        textMesh.rectTransform.anchorMin = new Vector2(1, 1);
        textMesh.rectTransform.anchorMax = new Vector2(1, 1);
        textMesh.rectTransform.anchoredPosition = new Vector2(textMesh.rectTransform.anchoredPosition.x, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public Image effectSquare; // Hình vuông hiệu ứng
    public float expandSize = 1f; // Kích thước phóng to
    public float duration = 0.05f; // Thời gian hiệu ứng
    [SerializeField] Color color;

    public void PlayEffect()
    {
        effectSquare.gameObject.SetActive(true);
        effectSquare.transform.localScale = Vector3.zero; // Reset kích thước
        effectSquare.color = new Color(0, 0, 0, 0.5f); // Đặt màu đen, alpha = 0.5 (mờ sẵn)

        // Phóng to dần mà không thay đổi độ trong suốt
        effectSquare.transform.DOScale(expandSize, duration)
             .SetEase(Ease.OutQuad).OnComplete(() => effectSquare.gameObject.SetActive(false));
    }
}

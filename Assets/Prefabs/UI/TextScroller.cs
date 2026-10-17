using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    public RectTransform textRectA;
    public RectTransform textRectB;
    public float scrollSpeed = 100f; // Pixels per second

    private float textWidth;
    private float canvasWidth;

    void Start()
    {
        // 1. Get the dynamic dimensions
        canvasWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        textWidth = textRectA.rect.width;

        // 2. Position Text B directly behind Text A
        textRectA.anchoredPosition = new Vector2(0, 0);
        textRectB.anchoredPosition = new Vector2(textWidth, 0);

        // 3. Create a continuous loop using DOTween's DOAnchorPosX
        // Calculate dynamic duration based on speed so it stays consistent
        float duration = textWidth / scrollSpeed;

        // Move Text A left, then snap it back to the right side when it completely finishes
        textRectA.DOAnchorPosX(-textWidth, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .OnStepComplete(() => textRectA.anchoredPosition = new Vector2(textWidth, 0));

        // Do the same for Text B, offset by its current position
        textRectB.DOAnchorPosX(0, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .OnStepComplete(() => textRectB.anchoredPosition = new Vector2(textWidth, 0));
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TextScroller : NetworkBehaviour
{
    public GameObject PanelContainenr;
    public RectTransform textRectA;
    public RectTransform textRectB;
    public float scrollSpeed = 100f; // Pixels per second

    private float textWidth;
    private float canvasWidth;

    public override void OnNetworkSpawn()
    {
        AbilitySpawner.Instance.OnMaterializeAbility += PresentText;

    }



    public override void OnNetworkDespawn()
    {
        AbilitySpawner.Instance.OnMaterializeAbility -= PresentText;
    }

    private void PresentText(Transform transform, float warningDuration)
    {
        // 1. Get the dynamic dimensions
        canvasWidth = this.gameObject.GetComponent<RectTransform>().rect.width;

        PanelContainenr.SetActive(true);
        // 1. Get the dynamic dimensions
        canvasWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
        textWidth = this.gameObject.GetComponent<RectTransform>().rect.width;

        textRectA.parent.gameObject.SetActive(true);
        textRectA.anchoredPosition = new Vector2(textWidth, 0);

        textRectB.parent.gameObject.SetActive(true);
        textRectB.anchoredPosition = new Vector2(textWidth, 0);

        // Calculate dynamic duration based on speed so it stays consistent
        float duration = textWidth / scrollSpeed;


        //  offset by its current position
        textRectA.DOAnchorPosX(0, warningDuration / 1.2f)
           .SetEase(Ease.Linear)
           .SetLoops(-1, LoopType.Incremental)
           .OnStepComplete(() => textRectA.anchoredPosition = new Vector2(textWidth, 0));

        textRectB.DOAnchorPosX(0, warningDuration / 1.2f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .OnStepComplete(() => textRectB.anchoredPosition = new Vector2(textWidth, 0));
    }

}

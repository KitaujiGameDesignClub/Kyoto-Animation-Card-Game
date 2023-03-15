using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardPanelInformation : MonoBehaviour
{
    public TMP_Text text;
    public SpriteRenderer iamge;
    public Sprite NegativeImage;
    readonly Vector2 negativeTextPos = new Vector2(-0.046f, -0.366f);
    readonly Color negativeTextColor = new Color(0.09908406f, 0.754717f, 0f);
    readonly float negativeSize = 1.5f;
    public Sprite PositiveImage;
    readonly Vector2 positiveTextPos = new Vector2(0.313f, -0.234f);
    readonly Color positiveTextColor = new Color(1f, 0.2806301f, 0.145283f);
    readonly float positiveSize = 2.2f;
    public Animator animator;
    private int animationHash;

    private void Awake()
    {
        animationHash = Animator.StringToHash("CardInformaiton");
    }

   
    public void Show(string content,bool negative)
    {      
        gameObject.SetActive(true);
        text.text = content;
        //给好的和不好的分别个性化
        iamge.sprite = negative ? NegativeImage : PositiveImage;
        text.transform.localPosition = negative ? negativeTextPos : positiveTextPos;
        text.fontSize = negative ? negativeSize : positiveSize;
        text.color = negative ? negativeTextColor : positiveTextColor;
        animator.Play(animationHash,0,0f);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [ContextMenu("试试动画")]
    public void tryAnimation()  => Show("节操 + 1",false);
#endif
}

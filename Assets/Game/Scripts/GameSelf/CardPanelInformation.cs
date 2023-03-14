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
    public GameObject gameObject;
    public Animator animator;
    private int animationHash;

    private void Awake()
    {
        animationHash = Animator.StringToHash("CardInformaiton");
    }

   
    public void Show(string content)
    {
        gameObject.SetActive(true);
       text.text = content;
        animator.Play(animationHash);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [ContextMenu("ÊÔÊÔ¶¯»­")]
    public void tryAnimation()  => Show("½Ú²Ù + 1");
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class TestMode : MonoBehaviour
{
    [Header("卡牌选择器")]
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorConfirmButton;

    private Animator animator;

    private void Awake()
    {
        //如果没有开启测试模式，就销毁这个物体
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        #region 卡牌选择器
        //展开和关闭
        CardSelectorToggle.onClick.AddListener(delegate
        {
            //切换开关状态
            isExpanded = !isExpanded;
            animator.SetBool("expanded", isExpanded);
        });
        CardSelectorCloseButton.OnClick.AddListener(delegate
        {
            //保存关闭状态
            isExpanded = false;
            animator.SetBool("expanded", isExpanded);
        });

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

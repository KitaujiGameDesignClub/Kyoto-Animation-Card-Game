using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class TestMode : MonoBehaviour
{
    [Header("����ѡ����")]
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorConfirmButton;

    private Animator animator;

    private void Awake()
    {
        //���û�п�������ģʽ���������������
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        #region ����ѡ����
        //չ���͹ر�
        CardSelectorToggle.onClick.AddListener(delegate
        {
            //�л�����״̬
            isExpanded = !isExpanded;
            animator.SetBool("expanded", isExpanded);
        });
        CardSelectorCloseButton.OnClick.AddListener(delegate
        {
            //����ر�״̬
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;

public class TestMode : MonoBehaviour
{
    [Header("ͨ��")]
 

    [Header("����ѡ����")]
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorConfirmButton;
    public Button BundleListItem;
    public RectTransform BundleListParent;
    private CardBundlesManifest selectedBundle;
    public Button CardListItem;
    private Bundle[] allBundles;


    private Animator animator;

    private void Awake()
    {
        //���û�п�������ģʽ���������������
    }

    /// <summary>
    /// ���ز���ģʽ
    /// </summary>
    async void Start()
    {
        animator = GetComponent<Animator>();
        GameUI.gameUI.SetBanInputLayer(true, "����ģʽ������...");

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
        //��ȡ���еĿ���
        allBundles = await CardReadWrite.GetAllBundles();
        //�ѿ����嵥������ӳ�䵽�������б��У�����ѡ�������Ķ�����
        foreach (var item in allBundles)
        {
            var manifest = Instantiate(BundleListItem, BundleListParent);
            manifest.gameObject.SetActive(true);
          //  manifest

        }
      

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}

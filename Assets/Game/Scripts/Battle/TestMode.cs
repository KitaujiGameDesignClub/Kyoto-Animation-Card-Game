using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using KitaujiGameDesignClub.GameFramework.UI;

public class TestMode : MonoBehaviour
{
    [Header("ͨ��")]
 

    [Header("����ѡ����")]
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorConfirmButton;
    /// <summary>
    /// ������ѡ����"�Ŀ����б�
    /// </summary>
    public InputFieldWithDropdown BundleList;
    /// <summary>
    /// ������ѡ����"�Ŀ����б�
    /// </summary>
    public InputFieldWithDropdown CardList;
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
        GameUI.gameUI.SetBanInputLayer(true, "�����ȡ��...");
        allBundles = await CardReadWrite.GetAllBundles();
        //�ѿ����嵥������ӳ�䵽�������б��У�����ѡ�������Ķ�����
        List<string> bundlesName = new();
       // bundlesName.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");
        foreach (var item in allBundles)
        {
            bundlesName.Add($"��{item.manifest.Anime}��{item.manifest.FriendlyBundleName}");
        }
        BundleList.ChangeOptionDatas(bundlesName);
      //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>����Ϊ���ÿ���");
      

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion



        //�ر���������
        GameUI.gameUI.SetBanInputLayer(false, "����ģʽ������...");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}

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
    [Header("通用")]
 

    [Header("卡牌选择器")]
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorConfirmButton;
    /// <summary>
    /// “卡牌选择器"的卡组列表
    /// </summary>
    public InputFieldWithDropdown BundleList;
    /// <summary>
    /// “卡牌选择器"的卡牌列表
    /// </summary>
    public InputFieldWithDropdown CardList;
    private Bundle[] allBundles;


    private Animator animator;

    private void Awake()
    {
        //如果没有开启测试模式，就销毁这个物体
    }

    /// <summary>
    /// 加载测试模式
    /// </summary>
    async void Start()
    {
        animator = GetComponent<Animator>();
        GameUI.gameUI.SetBanInputLayer(true, "测试模式载入中...");

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
        //读取所有的卡组
        GameUI.gameUI.SetBanInputLayer(true, "卡组读取中...");
        allBundles = await CardReadWrite.GetAllBundles();
        //把卡组清单的内容映射到“卡组列表”中（卡牌选择器左侧的东西）
        List<string> bundlesName = new();
       // bundlesName.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");
        foreach (var item in allBundles)
        {
            bundlesName.Add($"【{item.manifest.Anime}】{item.manifest.FriendlyBundleName}");
        }
        BundleList.ChangeOptionDatas(bundlesName);
      //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");
      

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion



        //关闭输入遮罩
        GameUI.gameUI.SetBanInputLayer(false, "测试模式载入中...");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}

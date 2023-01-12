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
    //备注：进入测试模式之前，应当修改内存中的设置，使测试模式永远开启控制台和帧率显示

    [Header("通用")]
    public TMP_Text title;

    [Header("卡牌选择器")]
    public CanvasGroup CardSelector;
    public Button CardSelectorToggle;
    private bool isExpanded;
    public LeanButton CardSelectorCloseButton;
    public LeanButton CardSelectorOpenByFileExplorerButton;
    public LeanButton CardSelectorConfirmButton;
    /// <summary>
    /// “卡牌选择器"的卡组列表
    /// </summary>
    public InputFieldWithDropdown BundleList;
    /// <summary>
    /// 卡牌选择器左侧的卡组信息展示
    /// </summary>
    public GameObject BundleInformationDisplay;
    public TMP_Text manifestFriendlyName;
    public TMP_Text manifestName;
    public TMP_Text manifestAnime;
    public TMP_Text manifestAuthorName;
    public TMP_Text manifestDescription;
    /// <summary>
    /// “卡牌选择器"的卡牌列表
    /// </summary>
    public InputFieldWithDropdown CardList;
    /// <summary>
    /// 卡牌选择器右侧的卡牌信息展示
    /// </summary>
    public GameObject CardInformationDisplay;
    public TMP_Text cardFriendlyName;
    public TMP_Text cardCharacterName;
    public TMP_Text cardCharacterVoiceName;
    public TMP_Text cardAnime;
    public TMP_Text cardBasicInf;//攻击力和生命值
    public TMP_Text cardDescription;
    /// <summary>
    /// 缓存的所有卡组
    /// </summary>
    private Bundle[] allBundles;



    private Animator animator;

    private void Awake()
    {
        //如果没有开启测试模式，就销毁这个物体

        //面板初始化
        title.text = $"目前处于测试模式\nDevice:{SystemInfo.deviceType}  CPU:{SystemInfo.processorType}  OS:{SystemInfo.operatingSystem}  RAM:{SystemInfo.systemMemorySize}MiB  Screen:{Screen.currentResolution}";

        #region 卡牌选择器
        //透明度调整
        CardSelector.alpha = 0f;
        CardSelector.blocksRaycasts = false;
        //调整卡牌选择器板块的激活状态
        CardSelector.gameObject.SetActive(true);
        BundleList.gameObject.SetActive(true);
        BundleInformationDisplay.SetActive(false);
        CardList.gameObject.SetActive(false);
        CardInformationDisplay.SetActive(false);

        #endregion
    }

    /// <summary>
    /// 加载测试模式
    /// </summary>
    async void Start()
    {
       //加载测试模式
       await  LoadTestMode();

    }


    private async UniTask LoadTestMode()
    {
        animator = GetComponent<Animator>();
        GameUI.gameUI.SetBanInputLayer(true, "测试模式载入中...");

        #region 卡牌选择器


        //读取所有的卡组
        GameUI.gameUI.SetBanInputLayer(true, "卡组读取中...");
        allBundles = await CardReadWrite.GetAllBundles();

#if UNITY_EDITOR || UNITY_STANDALONE
        //用资源管理器打开规定的目录
        CardSelectorOpenByFileExplorerButton.OnClick.AddListener(delegate
        {
            Application.OpenURL($"file://{Information.bundlesPath}");
        });

#else
  Destroy(CardSelectorOpenByFileExplorerButton.gameObject);
#endif


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


        //把卡组清单的内容映射到“卡组列表”中（卡牌选择器左侧的东西）
        List<string> bundlesName = new();
        // bundlesName.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");
        foreach (var item in allBundles)
        {
            Debug.Log(item);
            if (string.IsNullOrEmpty(item.manifest.Anime)) bundlesName.Add($"{item.manifest.FriendlyBundleName}");
            else bundlesName.Add($"【{item.manifest.Anime}】{item.manifest.FriendlyBundleName}");
        }
        BundleList.ChangeOptionDatas(bundlesName);
        bundlesName = null;
        //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");

        //选定的卡组信息同步
        BundleList.onDropdownValueChangedWithoutInt.AddListener(delegate
        {
            //卡组有选择，信息同步与卡牌列表激活
            if (BundleList.DropdownValue != 0)
            {
                UpdateSelectorBundleInformation(allBundles[BundleList.DropdownValue - 1].manifest);
                //显示此卡组信息，并允许选择卡牌
                BundleInformationDisplay.SetActive(true);
                CardList.gameObject.SetActive(true);
                //可用卡组也放到列表中
                List<string> cardsName = new();
                foreach (var item in allBundles[BundleList.DropdownValue - 1].cards)
                {
                    if (string.IsNullOrEmpty(item.CharacterName)) cardsName.Add($"{item.FriendlyCardName}");
                    else cardsName.Add($"【{item.CharacterName}】{item.FriendlyCardName}");
                }
                CardList.ChangeOptionDatas(cardsName);
            }
            //value = 0，没有选择卡组，将不必要的板块禁用
            else
            {
                BundleInformationDisplay.SetActive(false);
                CardList.gameObject.SetActive(false);
                CardInformationDisplay.SetActive(false);
            }

        });
        //选定的卡牌信息同步
        CardList.onDropdownValueChangedWithoutInt.AddListener(delegate 
        {
            UpdateSelectorCardInformation(allBundles[BundleList.DropdownValue - 1].cards[CardList.DropdownValue - 1]);
            CardInformationDisplay.SetActive(true);
        });

        CardSelectorConfirmButton.OnClick.AddListener(delegate { });
        #endregion



        //关闭输入遮罩
        GameUI.gameUI.SetBanInputLayer(false, "测试模式载入中...");
    }




#region 卡牌选择器配套方法

    /// <summary>
    /// 在卡牌选择器中同步卡组清单信息
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorBundleInformation(CardBundlesManifest manifestContent)
    {
        manifestFriendlyName.text = $"友好名称：\n<margin-left=1em><size=80%>{manifestContent.FriendlyBundleName}";
        manifestName.text =  $"识别名称：\n<margin-left=1em><size=80%>{manifestContent.BundleName}";
        manifestAnime.text = $"所属动画：\n<margin-left=1em><size=80%>{manifestContent.Anime}";
        manifestAuthorName.text = $"作者名称：\n<margin-left=1em><size=80%>{manifestContent.AuthorName}";
        manifestDescription.text = $"卡组介绍：\n<margin-left=1em><size=80%>{manifestContent.Description}";
    }

    /// <summary>
    /// 在卡牌选择器中同步卡牌信息
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorCardInformation(CharacterCard cardContent)
    {
        cardFriendlyName.text = $"友好名称：\n<margin-left=1em><size=80%>{cardContent.FriendlyCardName}";
        cardCharacterName.text = $"角色名称：\n<margin-left=1em><size=80%>{cardContent.CharacterName}";
        cardCharacterVoiceName.text = $"声优名称：\n<margin-left=1em><size=80%>{cardContent.CV}";
        cardAnime.text = $"所属动画：\n<margin-left=1em><size=80%>{cardContent.Anime}";
        cardBasicInf.text = $"执行力/体力值：{cardContent.BasicPower}/{cardContent.BasicHealthPoint}";
        cardDescription.text = $"能力介绍：\n<margin-left=1em><size=80%>{cardContent.AbilityDescription}";
    }

    #endregion



}

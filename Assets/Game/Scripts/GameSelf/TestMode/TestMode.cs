using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using KitaujiGameDesignClub.GameFramework.UI;
using System.IO;
using UnityEngine.Profiling;

public class TestMode : MonoBehaviour
{

    //备注：进入测试模式之前，应当修改内存中的设置，使测试模式永远开启控制台和帧率显示

    [Header("通用")]
    public TMP_Text title;
    public CanvasGroup panel;
    public LeanButton panelCloseButton;
    public LeanButton CardSelectorOpenByFileExplorerButton;
    public Button GoToEditor;
    [Header("卡牌选择器")]
    public Toggle CardSelectorToggle;
    public GameObject cardSelector;
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
    public TMP_Text cardTag;
    public TMP_Text cardDescription;
    public Button[] DeletionButtons = new Button[6];
    /// <summary>
    /// 缓存的所有卡组
    /// </summary>
    private Bundle[] allBundles;
    /// <summary>
    /// 选择的卡组的id
    /// </summary>
    int selectedBundleId = -1;
    /// <summary>
    /// 选择的卡牌的id（选定卡组内的）
    /// </summary>
    int selectedCardId = -1;

    /// <summary>
    /// 已经加载过卡牌了
    /// </summary>
    bool hasAddCard = false;

    [Header("语音测试器")]
    public GameObject voiceTestor;
    public Toggle voiceTestorToggle;
    public AudioSource voiceTestPlayer;
    public Slider voiceTestVolume;
    /// <summary>
    /// 6个音频测试器
    /// </summary>
    public TMAudioTestor[] tMAudioTestors = new TMAudioTestor[6];

    [Header("打架模拟器")]
    public GameObject battleEmulator;
    public Toggle battleEmulatorToggle;
    public TMEnemyEmluation[] enemy = new TMEnemyEmluation[0];
    public Button BattleEnemyAdditionButton;
    public Button GoToBattleFieldButton;
    [Header("打架模拟器-控制部分")]
    public GameObject BattleEmulatorControlPart;
    public LeanToggle[] PauseModeSelector = new LeanToggle[4];
    public Button ContinueBattleButton;
    [SerializeField] private TMP_Text roundLogger;
    [SerializeField] private ScrollRect loggerScroll;
    public LeanButton StartBattleButton;
    public LeanButton RestartTestButton;
    public LeanButton ShutdownTestButton;
    private string[] EnemyCardUUIDCache;
    private string[] PlayerCardUUIDCache;


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
        //面板初始化
        title.text = $"目前处于测试模式\n设备: {SystemInfo.deviceType}  CPU:{SystemInfo.processorType}  OS:{SystemInfo.operatingSystem}  Screen:{Screen.currentResolution.width}x{Screen.currentResolution.height}";
        //初始化一个新游戏
        GameStageCtrl.stageCtrl.InitializeGame();

        panel.gameObject.SetActive(true);

        #region 调整卡牌选择器板块的激活状态
        BundleList.gameObject.SetActive(true);
        BundleInformationDisplay.SetActive(false);
        CardList.gameObject.SetActive(false);
        CardInformationDisplay.SetActive(false);
        #endregion

        #region 调整音频测试器板块的激活状态
        foreach (var item in tMAudioTestors)
        {
            item.gameObject.SetActive(false);
        }
        #endregion

        #region 调整打架模拟的激活状态
        BattleEmulatorControlPart.SetActive(false);
        #endregion


        //加载测试模式
        await LoadTestMode();

    }

    /// <summary>
    /// 加载 初始化测试模式
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadTestMode()
    {
        animator = GetComponent<Animator>();

        GameUI.gameUI.SetBanInputLayer(true, "测试模式载入中...");

        #region 跳转到编辑器
        GoToEditor.onClick.AddListener(delegate ()
        {
            Loading.LoadScene(1);
        });
        #endregion


        #region 卡牌选择器
        //卡牌选择器 展开和关闭
        Toggle(CardSelectorToggle,cardSelector);

        for (int i = 0; i < DeletionButtons.Length; i++)
        {
            //缓存i的值
            var value = i;
            Button deletionButton = DeletionButtons[value];
            //总之先把所有的删除按钮禁用了
            deletionButton.interactable = false;
            //注册卡牌删除事件
            deletionButton.onClick.AddListener(delegate
            {
                //移除要删除的卡牌
                GameStageCtrl.stageCtrl.RemoveCardOnSpot(0, value);
                //调整按钮的禁用关系
                RefreshDeletionButtonState();

                //己方这边已经没有卡组在场上了
                if (GameState.CardOnSpot[0].Count == 0)
                {
                    //恢复卡组选择
                    BundleList.interactable = true;
                    //不知道为什么总是要给删掉加载的卡牌。。。
                  //  BundleList.ChangeOptionDatas(allBundles);
                }

             //   Debug.Log($"{value}  {i}"); 经过这行代码的验证，i在这个事件中永远是6，所以需要缓存一个value
            });
        }

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

        //把卡组清单的内容映射到“卡组列表”中（卡牌选择器左侧的东西）
        List<string> bundlesName = new();
        // bundlesName.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");
        for (int i = 0; i < allBundles.Length; i++)
        {
            Bundle bundle = allBundles[i];
            if (string.IsNullOrEmpty(bundle.manifest.Anime)) bundlesName.Add($"{bundle.manifest.FriendlyBundleName}<alpha=#00>{i}");
            else bundlesName.Add($"【{bundle.manifest.Anime}】{bundle.manifest.FriendlyBundleName}<alpha=#00>{i}");
            //透明度为0的隐藏字符：<alpha=#00>{i}，用来记录这些卡组的序号，便于搜索后选择
        }
        BundleList.ChangeOptionDatas(bundlesName);
        bundlesName = null;
        //  BundleList.ban.Add("<align=\"center\"><alpha=#CC>以下为可用卡组");

        //选定的卡组信息同步
        BundleList.onDropdownValueChangedWithoutInt.AddListener(delegate
        {
            //清除卡组缓存
            GameState.cardCaches.Clear();

            //选定另一个卡组时，清空卡牌选择栏
            selectedCardId = -1;
            CardList.text = string.Empty;

            //allBundles[selectedBundleId]：所选卡组

            //获取所选bundle的序号
            selectedBundleId = BundleList.text.Contains("<alpha=#00>") ? int.Parse(BundleList.text.Split("<alpha=#00>")[1]) : -1;

            //卡组有选择，信息同步与卡牌列表激活
            if (BundleList.DropdownValue != 0)
            {
                UpdateSelectorBundleInformation(allBundles[selectedBundleId].manifest);
                //显示此卡组信息，并允许选择卡牌
                BundleInformationDisplay.SetActive(true);
                CardList.gameObject.SetActive(true);
                //可用卡组也放到列表中
                List<string> cardsName = new();
                for (int i = 0; i < allBundles[selectedBundleId].cards.Length; i++)
                {
                    CharacterCard card = allBundles[selectedBundleId].cards[i];
                    if (string.IsNullOrEmpty(card.CharacterName)) cardsName.Add($"{card.FriendlyCardName}<alpha=#00>{i}");
                    else cardsName.Add($"【{card.CharacterName}】{card.FriendlyCardName}<alpha=#00>{i}");
                    //透明度为0的隐藏字符：<alpha=#00>{i}，用来记录这些卡组的序号，便于搜索后选择
                }
                CardList.ChangeOptionDatas(cardsName);
            }
            //value = 0，没有选择卡组，将不必要的板块禁用
            else
            {
                BundleInformationDisplay.SetActive(false);
                CardList.gameObject.SetActive(false);
                CardInformationDisplay.SetActive(false);
                CardList.text = string.Empty;
                CardList.ClearOptions();
            }

        });
        //选定的卡牌信息同步
        CardList.onDropdownValueChangedWithoutInt.AddListener(delegate 
        {
            //获取所选card的序号
            selectedCardId = CardList.text.Contains("<alpha=#00>") ? int.Parse(CardList.text.Split("<alpha=#00>")[1]) : -1;

            UpdateSelectorCardInformation(allBundles[selectedBundleId].cards[selectedCardId]);
            CardInformationDisplay.SetActive(true);
        });


        //确认此卡牌上场
        CardSelectorConfirmButton.OnClick.AddListener(UniTask.UnityAction( async () => 
        {
            //allBundles[selectedBundleId]：所选卡组

            //不选择卡牌，不允许执行确认操作
            if (CardList.DropdownValue > 0)
            {

                //禁止切换卡组
                BundleList.interactable = false;

                //如果卡组没有缓存，那就将此卡组内所有的卡牌导入到缓存中
                if (GameState.cardCaches.Count == 0)
                {
                    //第一次加载卡牌的话不回收垃圾
                    if (hasAddCard)
                    {
                        GameUI.gameUI.SetBanInputLayer(true, "清理中...");

                        //垃圾清理
                        GC.Collect();
                        await Resources.UnloadUnusedAssets(); 

                        await UniTask.Delay(10);

                    }

                    GameUI.gameUI.SetBanInputLayer(true, "卡组加载中...");


                    var beforeMemoryUsed = Profiler.GetTotalAllocatedMemoryLong(); 
                    await CardReadWrite.LoadBundleAndSaveInCache(Path.GetDirectoryName(allBundles[selectedBundleId].manifestFullPath));

                    manifestDescription.text = $"{manifestDescription.text}\n\n卡组内存占用：{((Profiler.GetTotalAllocatedMemoryLong() - beforeMemoryUsed)/1024/1024f).ToString("#0.00")}MiB";

                    GameUI.gameUI.SetBanInputLayer(false, "卡组加载中...");
                }

                //所选卡牌
                var card = allBundles[selectedBundleId].cards[selectedCardId];

                //从缓存中添加卡牌咯
                await GameStageCtrl.stageCtrl.DisplayCardFromCache(card.UUID, 0);  

                //刷新删除按钮的激活状态
                RefreshDeletionButtonState();

                //第一次加载卡牌已经结束了
                hasAddCard = true;
            }

          
        }));
        #endregion

        #region 语音测试器
        Toggle(voiceTestorToggle,voiceTestor);
        //打开音频测试器之后，读取场上存在的卡牌，用于测试音频
        voiceTestorToggle.onValueChanged.AddListener(delegate
        {
            //当然是得激活才读取
            if (voiceTestorToggle.isOn)
            {
                //得到所有的cardpanel，用于读取其中的音频资源
                var allCardPanels = GameStageCtrl.stageCtrl.GetAllCardOnStage(0);
                //激活测试器，然后配置相应的资源
                for (int i = 0; i < allCardPanels.Length; i++)
                {
                    tMAudioTestors[i].EnableAudioTestor(allCardPanels[i]);
                }
               
            }
            //关闭测试器界面，就把所有的测试器禁用了
            else
            {
                foreach (var item in tMAudioTestors)
                {
                    item.gameObject.SetActive(false);
                }
            }
          
        });

        //音量调整
        voiceTestVolume.onValueChanged.AddListener(delegate (float arg0)
        {
            voiceTestPlayer.volume = arg0;
        });

        #endregion

        #region 打架模拟器

        //掐架的控制器与选人面板的切换
        void ChangePanel(bool intoBattleControl)
        {
            BattleEmulatorControlPart.SetActive(intoBattleControl);
            battleEmulatorToggle.gameObject.SetActive(!intoBattleControl);
            CardSelectorToggle.gameObject.SetActive(!intoBattleControl);
            voiceTestorToggle.gameObject.SetActive(!intoBattleControl);
            GoToEditor.gameObject.SetActive(!intoBattleControl);
            panel.gameObject.SetActive(!intoBattleControl);
        }

        //开关
        Toggle(battleEmulatorToggle, battleEmulator);
        //添加敌机
        BattleEnemyAdditionButton.onClick.AddListener(async delegate 
        {
            //清除已有敌机
            GameStageCtrl.stageCtrl.RemoveAllCardsOnSpot(1);
            //添加卡牌（反正现在就一个敌机卡）
            for (int i = 0; i < enemy[0].enemyProfile.CardCount; i++)
            {
               await AddEnemyCard(enemy[0].enemyProfile);
            } 
        });

        //进入到掐架控制部分
        GoToBattleFieldButton.onClick.AddListener(delegate
        {
            if(GameStageCtrl.stageCtrl.GetAllCardOnStage(0).Length > 0 && GameStageCtrl.stageCtrl.GetAllCardOnStage(1).Length > 0)
            {
                ChangePanel(true);
                //记录缓存
                PlayerCardUUIDCache = new string[GameState.CardOnSpot[0].Count];
                for (int i = 0; i < GameState.CardOnSpot[0].Count; i++)
                {
                    PlayerCardUUIDCache[i] = GameState.CardOnSpot[0][i].Profile.UUID;                   
                }
                EnemyCardUUIDCache = new string[GameState.CardOnSpot[1].Count];
                for (int i = 0; i < GameState.CardOnSpot[1].Count; i++)
                {
                    EnemyCardUUIDCache[i] = GameState.CardOnSpot[1][i].Profile.UUID;
                }
            }
            else
            {
                Notify.notify.CreateBannerNotification(null, "场上卡牌数量不足，不能测试");
            }

            //更新游戏状态
            GameState.gameState = Information.GameState.Preparation;
        });

        //取消掐架测试，并返回到选人界面
        ShutdownTestButton.OnClick.AddListener(delegate ()
        {
            //重置测试状态
            RestartTestButton.OnClick.Invoke();
            ChangePanel(false);
        });
        //重新测试
        RestartTestButton.OnClick.AddListener(delegate ()
        {
            //停止打架
            GameStageCtrl.stageCtrl.StopBattle();

            GameStageCtrl.stageCtrl.RemoveAllCardsOnSpot(0);
            GameStageCtrl.stageCtrl.RemoveAllCardsOnSpot(1);
            foreach (var item in PlayerCardUUIDCache)
            {
                GameStageCtrl.stageCtrl.DisplayCardFromCache(item, 0);
            }
            foreach (var item in EnemyCardUUIDCache)
            {
                GameStageCtrl.stageCtrl.DisplayCardFromCache(item, 1);
            }

            //激活开始测试的按钮
            StartBattleButton.gameObject.SetActive(true);
            //禁用继续测试按钮
            ContinueBattleButton.interactable = false;
        });

        //模拟开战（下方那个蓝色的按钮）
        StartBattleButton.OnClick.AddListener(delegate
        {

            for (int i = 0; i < PauseModeSelector.Length; i++)
            {
                //找到选择的是哪个暂停模式，开打
                //顺便注册激活“继续战斗”按钮事件
                if (PauseModeSelector[i].On)
                {
                    GameStageCtrl.stageCtrl.BattleSystem((Information.PauseModeOfBattle)i, (Action)delegate { ContinueBattleButton.interactable = true; });
                    break;
                }

            }
            //清除日志
            roundLogger.text = string.Empty;
            //禁用“继续战斗”按钮
            ContinueBattleButton.interactable = false;//（上方那个白色的按钮）
            //禁用“开始战斗”按钮
            StartBattleButton.gameObject.SetActive(false);


        });

        //继续战斗
        ContinueBattleButton.onClick.AddListener(delegate
        {
            //禁用“继续战斗”按钮
            ContinueBattleButton.interactable = false;

            for (int i = 0; i < PauseModeSelector.Length; i++)
            {
                //找到选择的是哪个暂停模式，开打
                //顺便注册激活“继续战斗”按钮事件
                if (PauseModeSelector[i].On) GameStageCtrl.stageCtrl.BattleSystem((Information.PauseModeOfBattle)i, (Action)delegate { ContinueBattleButton.interactable = true; });

            }
        });


        #endregion

        //关闭输入遮罩
        GameUI.gameUI.SetBanInputLayer(false, "测试模式载入中...");
    }

    public void RoundLoggerManager(string news,bool clear = false)
    {
        if(clear) roundLogger.text = string.Empty;
        else roundLogger.text = $"{roundLogger.text}\n <b>YUKI.N ></b> {news}";
        //拉到最底下
        loggerScroll.verticalNormalizedPosition = 0f;
    }


    #region 通用
    void Toggle(Toggle toggle,GameObject panelObject)
    {
        //上方的切换器
        toggle.onValueChanged.AddListener(delegate
        {
                //切换开关状态                
                animator.SetBool("Expanded", toggle.isOn);
                panelObject.SetActive(toggle.isOn);

        });

        //每个panel内部的关闭按钮
        panelCloseButton.OnClick.AddListener(delegate
        {
            //保存关闭状态
            toggle.isOn = false;

        });

        //关闭这个工具的界面
        panelObject.SetActive(false);
    }
    #endregion


    #region 卡牌选择器配套方法

    /// <summary>
    /// 根据己方场上卡牌的数量，刷新删除按钮的可用状态
    /// </summary>
    void RefreshDeletionButtonState()
    {
        var count = GameStageCtrl.stageCtrl.GetCardCount(0);
        for (int i = 0; i < 6; i++)
        {
            DeletionButtons[i].interactable = count > i;
        }
    }

    /// <summary>
    /// 在卡牌选择器中同步卡组清单信息
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorBundleInformation(CardBundlesManifest manifestContent)
    {
        manifestFriendlyName.text = $"<b>友好名称：</b>\n<margin-left=1em><size=80%>{manifestContent.FriendlyBundleName}";
        manifestAnime.text = $"<b>所属动画：</b>\n<margin-left=1em><size=80%>{manifestContent.Anime}";
        manifestAuthorName.text = $"<b>作者名称：</b>\n<margin-left=1em><size=80%>{manifestContent.AuthorName}";
        manifestDescription.text = $"<b>卡组介绍：</b>\n<margin-left=1em><size=80%>{manifestContent.Description}";
    }

    /// <summary>
    /// 在卡牌选择器中同步卡牌信息
    /// </summary>
    /// <param name="manifestContent"></param>
    void UpdateSelectorCardInformation(CharacterCard cardContent)
    {
        cardFriendlyName.text = $"<b>友好名称：</b>\n<margin-left=1em><size=80%>{cardContent.FriendlyCardName}";
        cardCharacterName.text = $"<b>角色名称：</b>\n<margin-left=1em><size=80%>{cardContent.CharacterName}";
        cardCharacterVoiceName.text = $"<b>声优名称：</b>\n<margin-left=1em><size=80%>{cardContent.CV}";
        cardAnime.text = $"<b>所属动画：</b>\n<margin-left=1em><size=80%>{cardContent.Anime}";
        cardBasicInf.text = $"<b>执行力/体力值：</b>{cardContent.BasicPower}/{cardContent.BasicHealthPoint}";
        //标签展示
        cardTag.text = $"<b>标签：</b>";
        for (int i = 0; i < cardContent.tags.Count; i++)
        {
            string item = cardContent.tags[i];
            if (i == 0) cardTag.text = $"{cardTag.text}{item}";           
            cardTag.text = $"{cardTag.text}、{item}";
        }
        cardDescription.text = $"<b>能力介绍：</b>\n<margin-left=1em><size=80%>{cardContent.AbilityDescription}";
    }

    #endregion

    #region 打架模拟器配套方法

    private async UniTask AddEnemyCard(CharacterCard characterCard)
    {
        CardPanel card = null;

        //先看看缓存里有没有
        foreach (CardCache item in GameState.cardCaches)
        {            
            //有的话就用缓存了
            if (item.UUID == characterCard.UUID)
            {
              card =  await GameStageCtrl.stageCtrl.DisplayCardFromCache(characterCard.UUID, 1);
                break;
            }
        }

        //没有的话就生成吧
      if(card == null)
        {
            var uuid = await CardReadWrite.LoadCardAndSaveInCache(null, characterCard, enemy[0].image.sprite);
            card = await GameStageCtrl.stageCtrl.DisplayCardFromCache(characterCard.UUID, 1);

        }

    }
    #endregion

}



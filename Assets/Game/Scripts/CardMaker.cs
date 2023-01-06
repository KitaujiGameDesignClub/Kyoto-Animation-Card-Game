using System;
using System.Collections;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using KitaujiGameDesignClub.GameFramework.Tools;
using System.Collections.Generic;
using System.IO;
using Debug = UnityEngine.Debug;

/// <summary>
/// 卡包/卡牌制作用
/// </summary>
public class CardMaker : MonoBehaviour
{
    public static CardMaker cardMaker;

    public BasicEvents basicEvents;

    /// <summary>
    /// 内置卡组路径的图标
    /// </summary>
    public Sprite bundlePathIco;


    [Header("界面切换")] public GameObject title;
    [FormerlySerializedAs("ManifestEditor")] public GameObject ManifestEditorPlane;
    [FormerlySerializedAs("CardEditor")] public GameObject CardEditorPlane;
 public BundleEditor bundleEditor;
 public CardEditor cardEditor;


    /// <summary>
    /// 禁用输入层
    /// </summary>
    [Header("加载与保存")] public GameObject banInput;

    /// <summary>
    /// 修改标记
    /// </summary>
    public GameObject changeSignal;

    /// <summary>
    /// 保存的状态
    /// </summary>
    public TMP_Text saveStatus;
    

    /// <summary>
    /// 现在正在编辑的卡包（卡组）（包含了清单和卡片配置）
    /// </summary>
    public BundleOfCreate nowEditingBundle = new BundleOfCreate();

    /// <summary>
    /// 游戏加载时的实践
    /// </summary>
    [Header("游戏加载时的实践")] public UnityEvent OnGameLoad;


    private void Awake()
    {
        Information.bundlesPath = $"{YamlReadWrite.UnityButNotAssets}/bundles";


        cardMaker = this;

        //加载游戏的事件
        OnGameLoad.Invoke();

        //初始化界面
        title.SetActive(true);
        ManifestEditorPlane.SetActive(false);
        CardEditorPlane.SetActive(false);

        //关闭修改信号
        changeSignal.SetActive(false);
    }

    private void Start()
    {
        //刷新yaml资源
        refreshYamlRes();


        //隐藏文件选择器
        FileBrowser.HideDialog();

        //允许玩家输入
        banInput.SetActive(false);

        //加入一个quickLink
        FileBrowser.AddQuickLink("游戏卡组", $"{Information.bundlesPath}", bundlePathIco);

        //android储存权限申请
        AndroidRequestPermeission();
    }

    public void AndroidRequestPermeission()
    {
        FileBrowser.RequestPermission();

        if (FileBrowser.CheckPermission() == FileBrowser.Permission.Denied)
        {
            //不能读取外部储存的有关逻辑
            Notify.notify.CreateStrongNotification(null, delegate { basicEvents.ReturnToTitle(); },
                "储存权限被拒绝", "如果要编辑或制作卡包，需要储存权限\n点击黑色区域返回游戏标题");
        }
    }

    /// <summary>
    /// 创建卡包
    /// </summary>
    public void CreateBundle()
    {
        //创建新内容
        nowEditingBundle = new();
        //然后打开编辑器
        bundleEditor.OpenManifestEditor();
    }

    public async void EditBundleButton()
    {
              await EditBundle();
    }

    /// <summary>
    /// 编辑卡组
    /// </summary>
    private async UniTask EditBundle()
    {
        //得到文件内容
        FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组清单文件", Information.ManifestExtension));
        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡组清单", loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            nowEditingBundle = new();
            //现在还没有改内容，关闭修改标记
            changeSignal.SetActive(false);
            nowEditingBundle.loadedManifestFullPath = FileBrowser.Result[0];

            BanInputLayer(true,"读取卡组配置...");
            var bundle = await CardReadWrite.GetOneBundle(FileBrowser.Result[0]);

            nowEditingBundle.manifest = bundle.manifest;

            //缓存所有卡牌的友好和识别名称
            foreach (var variable in bundle.cards)
            {
                nowEditingBundle.allCardsFriendlyName.Add(variable.FriendlyCardName);
                nowEditingBundle.allCardsName.Add(variable.CardName);
            }

            Debug.Log($"成功加载卡组“{bundle.manifest.FriendlyBundleName}”，内含{bundle.cards.Length}张卡牌");

            //然后打开编辑器
         await bundleEditor.OpenManifestEditor();
        }
    }


    public void CreateNewCard()
    {
       
        //创建新内容
        nowEditingBundle = new();
        //然后打开编辑器
        cardEditor.OpenCardEditor();
    }

    public async void EditCardButton()
    {
                await EditCard();
    }

    private async UniTask EditCard()
    {
        //得到配置文件
        FileBrowser.SetFilters(false, new FileBrowser.Filter("卡牌配置文件", Information.CardExtension));
        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡牌配置", loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            nowEditingBundle = new();
            //现在还没有改内容，关闭修改标记
            changeSignal.SetActive(false);

            cardMaker.BanInputLayer(true, "卡牌配置加载中...");
            nowEditingBundle.loadedCardFullPath = FileBrowser.Result[0];

            var card = await YamlReadWrite.ReadAsync(
                new DescribeFileIO($"{Path.GetFileName(FileBrowser.Result[0])}",
                    $"-{Path.GetDirectoryName(FileBrowser.Result[0])}"), new CharacterCard(), false);

            nowEditingBundle.card = card;
            
            Debug.Log($"成功加载卡牌“{nowEditingBundle.card.FriendlyCardName}”");

            //然后打开编辑器
            await cardEditor.OpenCardEditor();
        }
        else
        {
            
        }

    }

   


/// <summary>
/// 异步保存
/// </summary>
/// <param name="manifestSaveFullPath">manifest储存的路径（含文件名和拓展名）</param>
/// <param name="manifestNewImageFullPath"></param>
/// <param name="cardSaveFullPath"></param>
/// <param name="cardNewImageFullPath">卡牌储存的路径（含文件名和拓展名）</param>
/// <param name="saveManifest"></param>
/// <param name="saveCard"></param>
/// <param name="cardAudioSettings"></param>
/// <exception cref="Exception"></exception>
    public async UniTask AsyncSave(string manifestSaveFullPath, string manifestNewImageFullPath,string cardSaveFullPath , string cardNewImageFullPath,
        bool saveManifest, bool saveCard, audioSetting[] cardAudioSettings)
    {
        
        BanInputLayer(true,"保存中...");

        if (saveManifest)
        {
            saveStatus.text = "卡组清单保存中...";
            
            try
            {
                Debug.Log( manifestSaveFullPath);
                await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest,
                    manifestSaveFullPath, manifestNewImageFullPath);
            }
            catch (Exception e)
            {
                Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                    "文件储存错误，详细信息请看控制台");
                banInput.SetActive(false);
                throw e;
            }
        }

        if (saveCard)
        {
            saveStatus.text = "卡牌保存中...";

            try
            {
                //音频路径与文件名获取
                var audios = new string[cardAudioSettings.Length];
                var audioNamesWithoutExtension = new string[cardAudioSettings.Length];
                for (int i = 0; i < audios.Length; i++)
                {
                    audios[i] = cardAudioSettings[i].newAudioFullFileName;
                    audioNamesWithoutExtension[i] = cardAudioSettings[i].VoiceName;
                }

                await CardReadWrite.CreateCardFile(nowEditingBundle.card, nowEditingBundle.loadedCardFullPath,
                    cardNewImageFullPath, audios, audioNamesWithoutExtension);
            }
            catch (Exception e)
            {
                Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                    "文件储存错误，详细信息请看控制台");
                banInput.SetActive(false);
                throw e;
            }
        }

        //关闭输入禁用层
        banInput.SetActive(false);
        cardMaker.changeSignal.SetActive(false);
    }

    /// <summary>
    /// 卡组清单另存为
    /// </summary>
    /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
    /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
    /// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
    /// <param name="saveManifest"></param>
    /// <param name="saveCard"></param>
    /// <returns>保存成功了吗？</returns>
    public async UniTask AsyncSaveTo(string manifestNewImageFullPath)
    {
        await AsyncSaveTo(manifestNewImageFullPath, null, true, false, null);
    }

    /// <summary>
    /// 卡牌另存为
    /// </summary>
    /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
    /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
    /// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
    /// <param name="saveManifest"></param>
    /// <param name="saveCard"></param>
    /// <returns>保存成功了吗？</returns>
    public async UniTask AsyncSaveTo( string cardNewImageFullPath, audioSetting[] cardAudioSettins)
    {
        await AsyncSaveTo(null,  cardNewImageFullPath, false, true, cardAudioSettins);
    }

    /// <summary>
    /// 另存为（整套卡包）
    /// </summary>
    /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
    /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
    /// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
    /// <param name="saveManifest"></param>
    /// <param name="saveCard"></param>
    /// <returns>保存成功了吗？</returns>
    public async UniTask AsyncSaveTo(string manifestNewImageFullPath, string cardNewImageFullPath,
        bool saveManifest, bool saveCard, audioSetting[] cardAudioSettings)
    {
        //还没有保存过/不是打开编辑卡包，打开选择文件的窗口，选择保存位置

        await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, false, title: "保存卡包",
            saveButtonText: "选择文件夹");

        //开启输入禁用层
        BanInputLayer(true, "另存为中...");


        if (FileBrowser.Success)
        {
            //保存
            await AsyncSave($"{FileBrowser.Result[0]}/{nowEditingBundle.manifest.BundleName}/{nowEditingBundle.manifest.BundleName}{Information.ManifestExtension}", manifestNewImageFullPath, FileBrowser.Result[0],
                cardNewImageFullPath, saveManifest, saveCard,
                cardAudioSettings);
        }
        else
        {
            //关闭禁用曾
            banInput.SetActive(false);
        }
    }


    /// <summary>
    /// 输入禁用层
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="textContent"></param>
    public void BanInputLayer(bool enable, string textContent)
    {
        //调整开关
        banInput.SetActive(enable);
        saveStatus.text = textContent;
    }


    

    #region 界面切换（返回，编辑器间切换）

    public void SwitchPanel(Action saveOrSaveTo, Action doWhat)
    {
        //修改信息被激活，说明修改了，提示要不要保存后在返回
        if (CardMaker.cardMaker.changeSignal.activeSelf)
        {
            Notify.notify.CreateStrongNotification(null, null, "修改尚未保存", "对此卡配置文件的修改尚未保存，要保存吗？", delegate
            {
                //保存，并停留在编辑器界面

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //弹出保存界面
                saveOrSaveTo.Invoke();
               //执行该做的事情
               doWhat.Invoke();
               
            }, "保存", delegate
            {
                //不保存，且回到Maker标题

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //执行该做的事情
                doWhat.Invoke();
                
            }, "不保存", delegate
            {
                //不保存，但是停留在编辑器节界面

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
            });
        }
        //不存在任何修改的话
        else
        {
            //执行该做的事情
            doWhat.Invoke();
        }
    }
    
    /// <summary>
    /// 回到编辑器标题界面
    /// </summary>
    public void ReturnToMakerTitle()
    {
        changeSignal.SetActive(false);
        CardEditorPlane.SetActive(false);
        ManifestEditorPlane.SetActive(false);
        title.SetActive(true);
    }

    

    #endregion
    
   
    /// <summary>
    /// 刷新yaml资源（tag anime cv列表，从本地文件中读取）
    /// </summary>
    public void refreshYamlRes() => CardReadWrite.refreshYamlResFromDisk();


#if UNITY_EDITOR
    [ContextMenu("各类测试")]
    public void test()
    {
        var s = new List<string>();
        s.Add("dd");
        s.Add("dd");
        s.Add("dw");
        s.Add("dd");
        Debug.Log(s.Count);
        s.RemoveAll(d => d == "dd");
        Debug.Log(s.Count); //把dd都删掉了
    }
#endif
}
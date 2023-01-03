using System;
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


    [Header("界面切换")] public GameObject title;

    [FormerlySerializedAs("ManifestEditor")]
    public GameObject ManifestEditorPlane;

    [FormerlySerializedAs("CardEditor")] public GameObject CardEditorPlane;


    [Header("bundle editor")] public BundleEditor bundleEditor;

    [Header("card editor")] public CardEditor cardEditor;


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
    /// 储存路径
    /// </summary>
    string savePath = string.Empty;

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
        OpenBundleEditor();
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
        nowEditingBundle = new();
        
        //得到文件内容
        FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组清单文件", Information.manifestExtension));
        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡组清单", loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            nowEditingBundle.loadedManifestFullPath = FileBrowser.Result[0];

            var bundle = await CardReadWrite.GetOneBundle(FileBrowser.Result[0]);

            nowEditingBundle.manifest = bundle.manifest;

            foreach (var variable in bundle.cards)
            {
                nowEditingBundle.allCardsFriendlyName.Add(variable.FriendlyCardName);
                nowEditingBundle.allCardsName.Add(variable.CardName);
            }

            Debug.Log($"成功加载卡组“{bundle.manifest.FriendlyBundleName}”，内含{bundle.cards.Length}张卡牌");

            //然后打开编辑器
            OpenBundleEditor();
        }
    }

    /// <summary>
    /// 打开卡组清单编辑器
    /// </summary>
    public void OpenBundleEditor()
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
        bundleEditor.OpenManifestEditor();
    }

    public void CreateNewCard()
    {
       
        //创建新内容
        nowEditingBundle = new();
        //然后打开编辑器
        openCardEditor();
    }

    /// <summary> 
    /// 打开卡牌文件编辑器
    /// </summary>
    /// <param name="onlyCard">仅创建卡牌</param>
    public void openCardEditor()
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
        cardEditor.OpenCardEditor();
    }


    public async UniTask AsyncSave(string manifestNewImageFullPath,  string cardNewImageFullPath,
        bool saveManifest, bool saveCard, audioSetting[] cardAudioSettins)
    {
        if (saveManifest)
        {
            //文件存在，在这保存
            if (File.Exists(nowEditingBundle.loadedManifestFullPath))
            {
                try
                {
                    await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest,
                        nowEditingBundle.loadedManifestFullPath, manifestNewImageFullPath);
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    throw e;
                }
            }
            //如果原文件直接就不存在了，那就调用另存为
            else
            {
                await AsyncSaveTo(manifestNewImageFullPath, cardNewImageFullPath, saveManifest, saveCard, cardAudioSettins);
            }
        }

        if (saveCard)
        {
            //文件存在，在这保存
            if (File.Exists(nowEditingBundle.loadedCardFullPath))
            {
                try
                {
                    var audios = new string[cardAudioSettins.Length];
                    for (int i = 0; i < audios.Length; i++)
                    {
                        audios[i] = cardAudioSettins[i].newAudioFullFileName;
                    }


                    await CardReadWrite.CreateCardFile(nowEditingBundle.card, nowEditingBundle.loadedCardFullPath,
                        cardNewImageFullPath, audios);
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    throw e;
                }
            }
            //如果原文件直接就不存在了，那就调用另存为
            else
            {
                await AsyncSaveTo(manifestNewImageFullPath, cardNewImageFullPath, saveManifest, saveCard, cardAudioSettins);
            }
        }

       
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
    public async UniTask AsyncSaveTo(string manifestNewImageFullPath,  string cardNewImageFullPath,
        bool saveManifest, bool saveCard, audioSetting[] cardAudioSettins)
    {
        //还没有保存过/不是打开编辑卡包，打开选择文件的窗口，选择保存位置
        if (savePath == string.Empty)
        {
            await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, false, title: "保存卡包",
                saveButtonText: "选择文件夹");


            if (FileBrowser.Success)
            {
                if (nowEditingBundle.manifest != null && saveManifest)
                {
                    saveStatus.text = "保存卡组清单文件...";

                    //保存储存路径，之后的话就可以不用进行另存为了
                    nowEditingBundle.loadedManifestFullPath = FileBrowser.Result[0];

                    try
                    {
                        //FileBrowser.Result[0]：仅仅是选定的目录，得给一个专有文件夹和文件名
                        await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest,
                            $"{FileBrowser.Result[0]}/{nowEditingBundle.manifest.BundleName}/{nowEditingBundle.manifest.BundleName}{Information.manifestExtension}",
                            manifestNewImageFullPath);
                    }
                    catch (Exception e)
                    {
                        Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                            "文件储存错误，详细信息请看控制台");
                        throw e;
                    }
                }

                if (nowEditingBundle.card != null && saveCard)
                {
                    saveStatus.text = "保存卡牌文件...";
                    
                    //保存储存路径，之后的话就可以不用进行另存为了
                    nowEditingBundle.loadedCardFullPath = FileBrowser.Result[0];

                    try
                    {
                        var audios = new string[cardAudioSettins.Length];
                        for (int i = 0; i < audios.Length; i++)
                        {
                            audios[i] = cardAudioSettins[i].newAudioFullFileName;
                        }


                        await CardReadWrite.CreateCardFile(nowEditingBundle.card, FileBrowser.Result[0],
                            cardNewImageFullPath, audios);
                    }
                    catch (Exception e)
                    {
                        Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                            "文件储存错误，详细信息请看控制台");
                        throw e;
                    }
                }

                //关闭输入禁用层
                banInput.SetActive(false);
                CardMaker.cardMaker.changeSignal.SetActive(false);
            }
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


    public void switchManifestCardEditor(UnityAction save)
    {
        //修改信息被激活，说明修改了，提示要不要保存后在返回
        if (CardMaker.cardMaker.changeSignal.activeSelf)
        {
            Notify.notify.CreateStrongNotification(null, null, "卡包清单尚未保存", "此卡包的清单文件尚未保存，要保存吗？", delegate()
            {
                //保存，并切换到另外一个编辑器

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //弹出保存界面，保存后会弹到另外一个编辑器,不保存也会去
                save.Invoke();
            }, "保存", delegate
            {
                //不保存，直接到另外一个编辑器

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //切换编辑器
                var bundleEditor = cardMaker.bundleEditor.gameObject;
                var cardEditor = cardMaker.cardEditor.gameObject;
                if (!cardEditor.activeSelf)
                {
                    this.cardEditor.OpenCardEditor();
                }
                else
                {
                    cardEditor.SetActive(false);
                }

                if (!bundleEditor.activeSelf)
                {
                    this.bundleEditor.OpenManifestEditor();
                }
                else
                {
                    bundleEditor.SetActive(false);
                }
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
            //切换编辑器
//切换编辑器
            var bundleEditor = cardMaker.bundleEditor.gameObject;
            var cardEditor = cardMaker.cardEditor.gameObject;
            if (!cardEditor.activeSelf)
            {
                this.cardEditor.OpenCardEditor();
            }
            else
            {
                cardEditor.SetActive(false);
            }

            if (!bundleEditor.activeSelf)
            {
                this.bundleEditor.OpenManifestEditor();
            }
            else
            {
                bundleEditor.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 想要回到编辑器的标题界面
    /// </summary>
    public void ReturnToTitle(Action save)
    {
        //修改信息被激活，说明修改了，提示要不要保存后在返回
        if (CardMaker.cardMaker.changeSignal.activeSelf)
        {
            Notify.notify.CreateStrongNotification(null, null, "卡包清单尚未保存", "此卡包的清单文件尚未保存，要保存吗？", delegate
            {
                //保存，并停留在编辑器界面

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //弹出保存界面
                save.Invoke();
            }, "保存", delegate
            {
                //不保存，且回到Maker标题

                //关闭通知
                Notify.notify.TurnOffStrongNotification();
                //回到标题节目（退出清单编辑器）
                CardMaker.cardMaker.ReturnToMakerTitle();
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
            //回到标题节目（退出清单编辑器）
            CardMaker.cardMaker.ReturnToMakerTitle();
        }
    }

    /// <summary>
    /// 回到编辑器标题界面
    /// </summary>
    private void ReturnToMakerTitle()
    {
        changeSignal.SetActive(false);
        CardEditorPlane.SetActive(false);
        ManifestEditorPlane.SetActive(false);
        title.SetActive(true);
    }

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
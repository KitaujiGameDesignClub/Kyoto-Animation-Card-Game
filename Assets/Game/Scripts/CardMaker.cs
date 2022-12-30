using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Console = System.Console;
using KitaujiGameDesignClub.GameFramework.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 卡包/卡牌制作用
/// </summary>
public class CardMaker : MonoBehaviour
{
    public static CardMaker cardMaker;

    public KitaujiGameDesignClub.GameFramework.Tools.BasicEvents basicEvents;
   

    [Header("界面切换")] public GameObject title;
    [FormerlySerializedAs("ManifestEditor")] public GameObject ManifestEditorPlane;
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
    [HideInInspector] public BundleOfCreate nowEditingBundle;

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
        
        nowEditingBundle = new();
        bundleEditor.OpenManifestEditorForCreation();
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
    }

    /// <summary>
    /// 编辑卡包
    /// </summary>
    public void EditBundle()
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
        bundleEditor.OpenManifestEditor();
    }

    /// <summary> 
    /// 创建新卡牌
    /// </summary>
    /// <param name="onlyCard">仅创建卡牌</param>
    public void CreateCard(bool onlyCard)
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);

        cardEditor.OpenCardEditorForCreation(onlyCard);
        nowEditingBundle = new();
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

/// <summary>
/// 保存或另存为（整套卡包）
/// </summary>
/// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
/// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
/// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
/// <param name="saveManifest"></param>
/// <param name="saveCard"></param>
/// <returns>保存成功了吗？</returns>
public async UniTask AsyncSave(string manifestNewImageFullPath)
{
  await   AsyncSave(manifestNewImageFullPath, 0, null, true, false, null);
}

/// <summary>
/// 保存或另存为（整套卡包）
/// </summary>
/// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
/// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
/// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
/// <param name="saveManifest"></param>
/// <param name="saveCard"></param>
/// <returns>保存成功了吗？</returns>
public async UniTask AsyncSave(int index,string cardNewImageFullPath,audioSetting[] cardAudioSettins)
{
    await   AsyncSave(null, index, cardNewImageFullPath, false, true, cardAudioSettins);
}

/// <summary>
/// 保存或另存为（整套卡包）
/// </summary>
/// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
/// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
/// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
/// <param name="saveManifest"></param>
/// <param name="saveCard"></param>
/// <returns>保存成功了吗？</returns>
public async UniTask AsyncSave(string manifestNewImageFullPath,int index,string cardNewImageFullPath,bool saveManifest,bool saveCard,audioSetting[] cardAudioSettins)
    {
        //还没有保存过/不是打开编辑卡包，打开选择文件的窗口，选择保存位置
        if (savePath == string.Empty)
        {
            await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, false, title: "保存卡包",
                saveButtonText: "选择文件夹");


            if (FileBrowser.Success)
            {
                //打开输入禁用层
                banInput.SetActive(true);


                if (nowEditingBundle.manifest != null && saveManifest)
                {
                    saveStatus.text = "保存卡组清单文件...";

                    try
                    {
                        await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest, FileBrowser.Result[0],
                            manifestNewImageFullPath);
                    }
                    catch (Exception e)
                    {
                        Notify.notify.CreateBannerNotification(delegate {  banInput.SetActive(false); },"文件储存错误，详细信息请看控制台");
                        throw e;
                    }
                 
                }

                if (nowEditingBundle.card != null && saveCard)
                {
                    saveStatus.text = "保存卡牌文件...";

                    try
                    {
                        var audios = new string[cardAudioSettins.Length];
                        for (int i = 0; i < audios.Length; i++)
                        {
                            audios[i] = cardAudioSettins[i].newAudioFullFileName;
                        }
                        
                       
                        
                        await CardReadWrite.CreateCardFile(nowEditingBundle.manifest.BundleName,
                            nowEditingBundle.card, FileBrowser.Result[0],cardNewImageFullPath, audios);

                    }
                    catch (Exception e)
                    {
                        Notify.notify.CreateBannerNotification(delegate {  banInput.SetActive(false); },"文件储存错误，详细信息请看控制台");
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
        Debug.Log(s.Count);//把dd都删掉了

        

    }
#endif
}
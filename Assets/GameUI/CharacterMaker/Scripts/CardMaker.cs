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

/// <summary>
/// 卡包/卡牌制作用
/// </summary>
public class CardMaker : MonoBehaviour
{
    public static CardMaker cardMaker;

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
    /// 游戏加载时的实践
    /// </summary>
    [Header("游戏加载时的实践")] public UnityEvent OnGameLoad;


    private void Awake()
    {
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
            Notify.notify.CreateStrongNotification(null, delegate { GetComponent<basicEvents>().ReturnToTitle(); },
                "储存权限被拒绝", "如果要编辑或制作卡包，需要储存权限\n点击黑色区域返回游戏标题");
        }
    }

    /// <summary>
    /// 创建卡包
    /// </summary>
    public void CreateBundle()
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
        bundleEditor.CreateNewBundle();
    }

    /// <summary>
    /// 创建新卡牌
    /// </summary>
    public void CreateCard(bool onlyCard)
    {
        //现在还没有改内容，关闭修改标记
        changeSignal.SetActive(false);
        // cardEditor.nowEditingCard = CardReadWrite.CreateNewCard(onlyCard);
        cardEditor.gameObject.SetActive(true);
    }

/// <summary>
/// 回到编辑器标题界面
/// </summary>
    public void ReturnToMakerTitle()
    {
        CardEditorPlane.SetActive(false);
        ManifestEditorPlane.SetActive(false);
        title.SetActive(true);
    }
    

   /// <summary>
   /// 保存或另存为（整套卡包）
   /// </summary>
   /// <param name="nowEditingCard"></param>
   /// <param name="nowEditingBundle"></param>
   /// <param name="manifestNewImageFullPath"></param>
   /// <returns>保存成功了吗？</returns>
    public async UniTask AsyncSave(CharacterCard nowEditingCard, CardBundlesManifest nowEditingBundle,
        string manifestNewImageFullPath)
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


                if (nowEditingBundle != null)
                {
                    saveStatus.text = "保存卡包配置文件...";

                    try
                    {
                        await CardReadWrite.CreateBundleManifestFile(nowEditingBundle, FileBrowser.Result[0],
                            manifestNewImageFullPath);
                    }
                    catch (Exception e)
                    {
                        Notify.notify.CreateBannerNotification(delegate {  banInput.SetActive(false); },"文件储存错误，详细信息请看控制台");
                        throw e;
                    }
                 
                }

                if (nowEditingCard != null)
                {
                    saveStatus.text = "保存卡牌配置文件...";
                }

                //关闭输入禁用层
                banInput.SetActive(false);
                CardMaker.cardMaker.changeSignal.SetActive(false);
              
            }
        }
    }


#if UNITY_EDITOR
    [ContextMenu("各类测试")]
    public void test()
    {   
        Notify.notify.CreateBannerNotification(delegate {  banInput.SetActive(false); },"文件储存错误，详细信息请看控制台");
        throw new Exception("114");
    }
#endif
}
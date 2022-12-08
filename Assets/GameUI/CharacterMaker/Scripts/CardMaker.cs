using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// 卡包/卡牌制作用
/// </summary>
public class CardMaker : MonoBehaviour
{
    public static CardMaker cardMaker;

    [Header("bundle editor")] public BundleEditor bundleEditor;

    [Header("card editor")] public CardEditor cardEditor;


    /// <summary>
    /// 禁用输入层
    /// </summary>
    [Header("加载与保存")] public GameObject banInput;

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
        bundleEditor.CreateNewBundle();
    }

    /// <summary>
    /// 创建新卡牌
    /// </summary>
    public void CreateCard(bool onlyCard)
    {
        // cardEditor.nowEditingCard = CardReadWrite.CreateNewCard(onlyCard);
        cardEditor.gameObject.SetActive(true);
    }


    /// <summary>
    /// 保存或另存为（整套卡包）
    /// </summary>
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

                    await CardReadWrite.CreateBundleManifestFile(nowEditingBundle, FileBrowser.Result[0],
                        manifestNewImageFullPath);
                }

                if (nowEditingCard != null)
                {
                    saveStatus.text = "保存卡牌配置文件...";
                }

                //关闭输入禁用层
                banInput.SetActive(false);
            }
        }
    }


#if UNITY_EDITOR
    [ContextMenu("各类测试")]
    public void test()
    {
        //不能读取外部储存的有关逻辑
    }
#endif
}
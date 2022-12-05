using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.UI;
using UnityEngine.Events;

/// <summary>
/// 卡包/卡牌制作用
/// </summary>
public class CardMaker : MonoBehaviour
{
    /// <summary>
    /// 工作路径（卡包的读写）
    /// </summary>
    private string WorkingPath;

    [Header("bundle editor")] public BundleEditor bundleEditor;

    [Header("card editor")] public CardEditor cardEditor;

    /// <summary>
    /// 游戏加载时的实践
    /// </summary>
    [Header("游戏加载时的实践")] public UnityEvent OnGameLoad;

    private void Awake()
    {
        //加载游戏的事件
        OnGameLoad.Invoke();
    }

    private void Start()
    {
        //隐藏文件选择器
        FileBrowser.HideDialog();

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
        cardEditor.nowEditingCard = CardReadWrite.CreateNewCard(onlyCard);
        cardEditor.gameObject.SetActive(true);
    }


#if UNITY_EDITOR
    [ContextMenu("各类测试")]
    public void test()
    {
        //不能读取外部储存的有关逻辑
    }
#endif
}
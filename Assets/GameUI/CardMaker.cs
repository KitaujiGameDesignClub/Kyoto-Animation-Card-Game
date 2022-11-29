using System;
using System.Collections;
using System.IO;
using Core;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.Tools;
using KitaujiGameDesignClub.GameFramework.UI;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardMaker : MonoBehaviour
{

    /// <summary>
    /// 主要的界面和debug界面，在选择文件的时候输入被禁用
    /// </summary>
    public GraphicRaycaster MainUIAndDebug;
    
    /// <summary>
    /// 工作路径（卡包的读写）
    /// </summary>
    private string WorkingPath;

    /// <summary>
    /// 临时路径（制作卡包时存放各类资源和文档）
    /// </summary>
    private readonly string tempPath = $"Temp/CardMaker/{Information.CardMakerVersion}";

    
    /// <summary>
    /// 游戏加载时的实践
    /// </summary>
    [Header("游戏加载时的实践")]
    public UnityEvent OnGameLoad;

    private void Awake()
    {
        //加载游戏的事件
        OnGameLoad.Invoke();
       
    }

    private void Start()
    {
        //隐藏文件选择器
        FileBrowser.HideDialog(true);
        
        //android储存权限申请
        AndroidRequestPermeission();
    }

    public void AndroidRequestPermeission()
    {
        FileBrowser.RequestPermission();

        if (FileBrowser.CheckPermission() == FileBrowser.Permission.Denied)
        {
            //不能读取外部储存的有关逻辑
            Notify.notify.CreateNotification(null, delegate { }, "储存权限被拒绝", "如果要编辑卡包，需要储存权限\n点击黑色区域返回游戏标题", 0.8f);
        }
    }
    

    public void CreateBundle()
    {
        OnBrowerShow();
        CardReadWrite.CreateNewBundle();
       
    }




    void OnBrowerShow()
    {
        MainUIAndDebug.ignoreReversedGraphics = true;
    }
    
    public void ShowEditor()
    {
        
    }


#if UNITY_EDITOR
    [ContextMenu("各类测试")]
    public void test()
    {
        //不能读取外部储存的有关逻辑
        Notify.notify.CreateNotification(null, delegate { }, "储存权限被拒绝", "如果要编辑卡包，需要储存权限\n点击黑色区域返回游戏标题", 0.8f);
    }
#endif
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.Tools;
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
 

    private void Start()
    {
        //隐藏文件选择器
        FileBrowser.HideDialog(true);
    }

    public void AndroidRequestPermeission()
    {
        FileBrowser.RequestPermission();

        if (FileBrowser.CheckPermission() == FileBrowser.Permission.Denied)
        {
            //不能读取外部储存的有关逻辑
        }
    }



    public void CreateBundle()
    {
        OnBrowerShow();
        StartCoroutine(createBundle());

    }

    IEnumerator createBundle()
    {

        
       
        
  
           //创建临时目录
           Directory.CreateDirectory(tempPath);
           
           //创建清单文件
           YamlReadWrite.Write(new BasicYamlIO("manifest",tempPath,"# 卡包清单文件。提供简单的、笼统的卡包信息"),new CardBundlesManifest());

           //先暂时防着
           yield return new WaitForEndOfFrame();
    }


    void OnBrowerShow()
    {
        MainUIAndDebug.ignoreReversedGraphics = true;
    }
    
    public void ShowEditor()
    {
        
    }
}

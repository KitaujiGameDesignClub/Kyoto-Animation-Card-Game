using System;
using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BundleEditor : MonoBehaviour
{
    /// <summary>
    /// 现在正在编辑的卡包的清单
    /// </summary>
    private CardBundlesManifest nowEditingBundle;

    [Header("编辑器")] public TMP_InputField bundleName;
    public TMP_InputField bundleFriendlyName;
    public TMP_InputField bundleVersion;
    public TMP_InputField authorName;
    public TMP_InputField description;
    public TMP_InputField remark;
    public TMP_Text codeVersionCheck;
    public Image bundleImage;
    /// <summary>
    /// 修改标记
    /// </summary>
    [Space] public GameObject changeSignal;
    [Header("效果显示")] public TMP_Text friendlyName;
    public TMP_Text descriptionOfBundle;
    public TMP_Text authorAndVersion;
    public SpriteRenderer image;
    
    
    
    public void CreateNewBundle()
    {
        gameObject.SetActive(true);

        //初始化编辑器内容
        ReadManifestContent(true);
    }


    /// <summary>
    /// 确认回到标题界面了
    /// </summary>
    public void Exit()
    {
    }

    public void ReadManifestContent(bool create)
    {
        //根据是否为新创建的bundle，来判断code version，初始化编辑器内容
        if (create)
        {
            //直接把新的当作正在编辑的卡包，并获取信息
            nowEditingBundle = CardReadWrite.CreateNewBundle();
            bundleName.text = nowEditingBundle.BundleName;
            bundleFriendlyName.text = nowEditingBundle.FriendlyBundleName;
            authorName.text = nowEditingBundle.AuthorName;
            description.text = nowEditingBundle.Description;
            remark.text = nowEditingBundle.Remarks;
            bundleVersion.text = nowEditingBundle.BundleVersion;
          //  shortDescription.text = nowEditingBundle.shortDescription;

            codeVersionCheck.text =
                $"清单代码版本号：{Information.ManifestVersion}\n编辑器代码版本号：{Information.ManifestVersion}\n<color=green>完全兼容</color>";
            
            //更新一下卡包预览
            OnEndEdit();
            //现在还没有改内容，关闭修改标记
            changeSignal.SetActive(false);
        }
        //不是创建的，是在修改已有的卡包
        else
        {
        }
    }


    /// <summary>
    /// 输入框内的数据完成，调用此函数，用于显示最终的效果
    /// </summary>
    public void OnEndEdit()
    {
        friendlyName.text = bundleFriendlyName.text;
        descriptionOfBundle.text = description.text;
        authorAndVersion.text = $"{authorName.text} - {bundleVersion.text}";
        image.sprite = bundleImage.sprite;
        changeSignal.SetActive(true);
    }
    

    public async void selectImage()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", ".jpg", ".bmp", ".png"));
        UniTask.SwitchToThreadPool();

        Debug.Log("要加载");

        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "选择卡包图片", "选择");

        Debug.Log("加载了");

        if (FileBrowser.Success)
        {
            Debug.Log("加载成功");

            //下载（加载）图片
            var hander = new DownloadHandlerTexture();
            UnityWebRequest unityWebRequest = new UnityWebRequest(FileBrowser.Result[0],"GET",hander,null);
            var sendWebRequest = await unityWebRequest.SendWebRequest();
            
            
            
            if (sendWebRequest.isDone)
            {
                if (sendWebRequest.result == UnityWebRequest.Result.Success)
                {
                   
                    int size;
                    //正方形图片就随便取一个边作为图片大小
                    if (hander.texture.width == hander.texture.height)
                    {
                        size = hander.texture.width;
                    }
                    //非正方形则取最短边
                    else
                    {
                        size = hander.texture.width > hander.texture.height
                            ? hander.texture.height
                            : hander.texture.width;
                    }
                    var sprite = Sprite.Create(hander.texture,new Rect(0f,0f,size,size),Vector2.one/2f);
                  
                    //更新预览图片和编辑器内图片
                    bundleImage.sprite = sprite;
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning($"{unityWebRequest.url}加载失败");
                }
            }
            
        }
        else
        {
            Debug.Log("及");
        }
    }
}
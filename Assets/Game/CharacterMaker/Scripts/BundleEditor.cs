using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using KitaujiGameDesignClub.GameFramework.UI;


public class BundleEditor : MonoBehaviour
{


    [Header("编辑器")] public TMP_InputField bundleName;
    public TMP_InputField bundleFriendlyName;
    public InputFieldWithDropdown Anime;
    public TMP_InputField bundleVersion;
    public TMP_InputField authorName;
    public TMP_InputField description;
    public TMP_InputField remark;
    public TMP_Text codeVersionCheck;
    public Image bundleImage;
    public Sprite DefaultImage;

    [Header("效果显示")] public TMP_Text friendlyName;
    public TMP_Text descriptionOfBundle;
    public TMP_Text authorAndVersion;
    public SpriteRenderer image;




    /// <summary>
    /// 新图片的完整路径
    /// </summary>
    string newImageFullPath = string.Empty;


    private void Start()
    {
        //初始化Anime的下拉栏
        List<string> all = new();
        for (int i = 0; i < Information.AnimeList.Length; i++)
        {
            all.Add(Information.AnimeList[i]);
        }
        Anime.ChangeOptionDatas(all);
       
    }

    /// <summary>
    /// 打开manifest editor（适用于编辑已有的卡组清单文件）
    /// </summary>
    /// <param name="create">是新创建的吗</param>
    public async UniTask OpenManifestEditor()
    {
     
        
        //启用编辑器
        gameObject.SetActive(true);
        //初始化编辑器内容
        await ReadManifestContent();

    }

    /// <summary>
    /// 打开manifest editor（适用于创建卡组清单文件）
    /// </summary>
    public void OpenManifestEditorForCreation()
    {
        //启用编辑器
        gameObject.SetActive(true);

        //直接把新的当作正在编辑的卡包，并获取信息
        var manifest = CardMaker.cardMaker.nowEditingBundle.manifest;
        bundleName.text = manifest.BundleName;
        bundleFriendlyName.text = manifest.FriendlyBundleName;
        authorName.text = manifest.AuthorName;
        Anime.text = Information.AnimeList[0];
        description.text = manifest.Description;
        remark.text = manifest.Remarks;
        bundleVersion.text = manifest.BundleVersion;
        //  shortDescription.text = CardMaker.cardMaker.nowEditingBundle.manifest.shortDescription;
        bundleImage.sprite = DefaultImage;


        codeVersionCheck.text =
            $"清单代码版本号：{Information.ManifestVersion}\n编辑器代码版本号：{Information.ManifestVersion}\n<color=green>完全兼容</color>";



        //更新一下卡包预览
        OnEndEdit();
    }


    /// <summary>
    /// 想要回到标题界面
    /// </summary>
    public void ReturnToTitle()
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
                   Save();
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
    /// 读取清单文件内容
    /// </summary>
    /// <param name="create"></param>
    async UniTask ReadManifestContent()
    {


        FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组清单文件", ".kabmanifest"));
        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, title: "选择卡组", loadButtonText: "加载");

        if (FileBrowser.Success)
        {
            await CardReadWrite.GetBundle(FileBrowser.Result[0]);
        }


        //更新一下卡包预览
        OnEndEdit();
        CardMaker.cardMaker.changeSignal.SetActive(false);

    }


    /// <summary>
    /// 保存或另存为
    /// </summary>
    public async void Save()
    {

        //执行保存or另存为操作
        await CardMaker.cardMaker.AsyncSave(null, CardMaker.cardMaker.nowEditingBundle.manifest, newImageFullPath);

    }



    /// <summary>
    /// 输入框内的数据完成，调用此函数，用于显示最终的效果
    /// </summary>
    public void OnEndEdit()
    {
        //更新暂存在内存中的清单
        CardMaker.cardMaker.nowEditingBundle.manifest.BundleName = bundleName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName = friendlyName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.BundleVersion = bundleVersion.text;
             CardMaker.cardMaker.nowEditingBundle.manifest.ImageName = newImageFullPath == string.Empty
            ? CardMaker.cardMaker.nowEditingBundle.manifest.ImageName
            : Path.GetFileName(newImageFullPath);
        CardMaker.cardMaker.nowEditingBundle.manifest.AuthorName = authorName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Description = description.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Remarks = remark.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Anime = Anime.text;
        
        //更新预览
        friendlyName.text = bundleFriendlyName.text;
        descriptionOfBundle.text = $"<B><#AD0015><size=113%>{Anime.text}</size></color></B>\n{description.text}";
        authorAndVersion.text = $"{authorName.text} - ver {bundleVersion.text}";
        image.sprite = bundleImage.sprite;
        CardMaker.cardMaker.changeSignal.SetActive(true);


    }



    /// <summary>
    /// 玩家选择图片
    /// </summary>
#pragma warning disable CS4014 // 没有等待的必要
    public void selectImage() => AsyncSelectImage();
#pragma warning restore CS4014 // 没有等待的必要



    async UniTask AsyncSelectImage()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", ".jpg", ".bmp", ".png", ".gif"));


        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "选择卡包图片", "选择");


        if (FileBrowser.Success)
        {
            Debug.Log($"加载成功，图片文件{FileBrowser.Result[0]}");
            //加载图片
            await AsyncLoadImage(FileBrowser.Result[0]);
            CardMaker.cardMaker.changeSignal.SetActive(true);
            newImageFullPath = FileBrowser.Result[0];
        }
    }


    async UniTask AsyncLoadImage(string imageFullPath)
    {
        //下载（加载）图片
        var hander = new DownloadHandlerTexture();
        UnityWebRequest unityWebRequest = new UnityWebRequest(imageFullPath, "GET", hander, null);
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
                var sprite = Sprite.Create(hander.texture, new Rect(0f, 0f, size, size), Vector2.one / 2f);

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

#if UNITY_EDITOR

    [ContextMenu("测试")]
    public void test()
    {

       
    }
#endif
    }
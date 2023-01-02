using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;


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

    [Header("面板")]
    public LeanButton returnToTitle;
    public TMP_Dropdown cardsFriendlyNamesList;
    public LeanButton switchToCardEditor;
    
    /// <summary>
    /// 新图片的完整路径
    /// </summary>
    string newImageFullPath { get; set; }



    private void Start()
    {
        #region 事件组

        //返回标题
        returnToTitle.OnClick.AddListener(delegate { CardMaker.cardMaker.ReturnToTitle(UniTask.Action(async () =>
        {
            await Save();
        })); });
        
        //切换到card editor，事先把要编辑的卡牌创建好
        switchToCardEditor.OnClick.AddListener(UniTask.UnityAction(async () =>
        {
            CardMaker.cardMaker.BanInputLayer(true, "切换中...");
            
            var card = new CharacterCard();
            if (cardsFriendlyNamesList.value >= 1)
            {
                var manifestPath = CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath;
                var cardFileName = CardMaker.cardMaker.nowEditingBundle.allCardsName[cardsFriendlyNamesList.value - 1];
                card = await YamlReadWrite.ReadAsync<CharacterCard>(
                    new DescribeFileIO($"{cardFileName}{Information.cardExtension}",
                        $"-{Path.GetDirectoryName(manifestPath)}/cards/{cardFileName}"), null, false);
            }
            CardMaker.cardMaker.nowEditingBundle.card = card;

            //切换到零一个编辑器
            CardMaker.cardMaker.switchManifestCardEditor(UniTask.UnityAction(
                async () =>
                {

                    //先检查保存
                    await Save();

                    //切换到另外一个编辑器
                 await CardMaker.cardMaker.cardEditor.OpenCardEditor();
                    gameObject.SetActive(false);
                }));
            
   
        }));
        

        

        #endregion
        
       
        
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
    public void OpenManifestEditor()
    {
     
        //启用编辑器
        gameObject.SetActive(true);
        //获取卡组清单信息
        var manifest = CardMaker.cardMaker.nowEditingBundle.manifest;
        bundleName.SetTextWithoutNotify(manifest.BundleName);
        bundleFriendlyName.SetTextWithoutNotify(manifest.FriendlyBundleName);
        authorName.SetTextWithoutNotify(manifest.AuthorName);
        Anime.inputField.SetTextWithoutNotify(manifest.Anime);
        description.SetTextWithoutNotify(manifest.Description);
        remark.SetTextWithoutNotify(manifest.Remarks);
        bundleVersion.SetTextWithoutNotify(manifest.BundleVersion);
        //  shortDescription.text = CardMaker.cardMaker.nowEditingBundle.manifest.shortDescription;
        bundleImage.sprite = DefaultImage;

        //读取卡组内所有卡牌的友好名称
        if (CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.Count == 0)
        {
            cardsFriendlyNamesList.gameObject.SetActive(false);
        }
        else
        {
            cardsFriendlyNamesList.ClearOptions();
            cardsFriendlyNamesList.AddOptions(CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName);
            cardsFriendlyNamesList.options.Insert(0,new TMP_Dropdown.OptionData("<创建新卡牌>"));
            cardsFriendlyNamesList.value = 0;
            cardsFriendlyNamesList.RefreshShownValue();
        }
      
        

        //兼容性检查
        if (manifest.CodeVersion == Information.ManifestVersion)
        {

            codeVersionCheck.text =
                $"清单代码版本号：{Information.ManifestVersion}\n编辑器代码版本号：{Information.ManifestVersion}\n<color=green>完全兼容</color>";
        }
        
        

      



        //更新一下卡包预览
        OnEndEdit();
        CardMaker.cardMaker.changeSignal.SetActive(false);
    }
    


    /// <summary>
    /// 保存或另存为
    /// </summary>
    public async void SaveButton()
    {
       await Save();
    }

    private async UniTask Save()
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
        
        //执行保存or另存为操作
        await CardMaker.cardMaker.AsyncSave(newImageFullPath);
    }



    /// <summary>
    /// 输入框内的数据完成，调用此函数，用于显示最终的效果
    /// </summary>
    public void OnEndEdit()
    {
      
        
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


        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "选择卡组图片", "选择");


        if (FileBrowser.Success)
        {
            Debug.Log($"加载成功，图片文件{FileBrowser.Result[0]}");
            //加载图片
            await AsyncLoadImage(FileBrowser.Result[0]);
            //显示已修改的印记
            CardMaker.cardMaker.changeSignal.SetActive(true);
            //更新新的图片全路径
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
                Debug.LogWarning($"{unityWebRequest.url}加载失败,错误原因{unityWebRequest.error}");
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
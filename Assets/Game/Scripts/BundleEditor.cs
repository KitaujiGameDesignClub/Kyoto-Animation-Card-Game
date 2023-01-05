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
            //检查保存（仅限在这个事件中）
            await SaveOrSaveTo();
        })); });
        
        //切换到card editor，事先把要编辑的卡牌创建好
        switchToCardEditor.OnClick.AddListener(UniTask.UnityAction(async () =>
        {
           
            
            CardMaker.cardMaker.BanInputLayer(true, "切换中...");
            
            //按照选定的卡牌进行资源加载
            var card = new CharacterCard();
            if (cardsFriendlyNamesList.value >= 1)
            {
                var manifestPath = CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath;
                var cardFileName = CardMaker.cardMaker.nowEditingBundle.allCardsName[cardsFriendlyNamesList.value - 1];
                CardMaker.cardMaker.BanInputLayer(true, "读取卡牌配置...");
                card = await YamlReadWrite.ReadAsync<CharacterCard>(
                    new DescribeFileIO($"{cardFileName}{Information.CardExtension}",
                        $"-{Path.GetDirectoryName(manifestPath)}/cards/{cardFileName}"), null, false);
                
                //保存一下加载的卡牌的路径
                CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath =
                    $"{Path.GetDirectoryName(manifestPath)}/cards/{cardFileName}/{cardFileName}{Information.CardExtension}";
            }
            CardMaker.cardMaker.nowEditingBundle.card = card;

            //切换到零一个编辑器
         
   
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
    /// 打开manifest editor（同步方法中，一定要放到最后一步）
    /// </summary>
    public async UniTask OpenManifestEditor()
    {
        CardMaker.cardMaker.BanInputLayer(true, "卡组配置加载中...");
       
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
       
        
        CardMaker.cardMaker.BanInputLayer(true, "卡组封面加载中...");
        //封面图片加载
        //先设置成默认的
        bundleImage.sprite = DefaultImage;
        //如果是加载的已有卡组，则读取现成的cover图片
        if (CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath != string.Empty)
        {
            //缓存卡组清单文件的根目录（文件夹）
            string rootPath = Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath);
            foreach (var format in Information.SupportedImageExtension)
            {
                //找一下根目录下有没有卡组用的封面

                //有的话，就尝试读取一下
                if (File.Exists($"{rootPath}/cover{format}"))
                {
                    //加载图片
                    var sprite = await CardReadWrite.ManifestLoadImageAsync($"{rootPath}/cover{format}");
                    sprite = sprite == null ? DefaultImage : sprite;
                    bundleImage.sprite = sprite;
                    break;
                }
            }
            
            ////没有的话，就保持刚刚设置的默认图片
        }
      
        

      
        //读取卡组内所有卡牌的友好名称
        if (CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName.Count == 0)
        {
            cardsFriendlyNamesList.gameObject.SetActive(false);
        }
        else
        {
            CardMaker.cardMaker.BanInputLayer(true, "所含卡牌缓存中...");
            cardsFriendlyNamesList.ClearOptions();
            cardsFriendlyNamesList.AddOptions(CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName);
            cardsFriendlyNamesList.options.Insert(0,new TMP_Dropdown.OptionData("<创建新卡牌>"));
            cardsFriendlyNamesList.value = 0;
            cardsFriendlyNamesList.RefreshShownValue();
            cardsFriendlyNamesList.gameObject.SetActive(true);
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
        CardMaker.cardMaker.BanInputLayer(false, "所含卡牌缓存中...");
        
        //启用编辑器
        gameObject.SetActive(true);
    }
    


    /// <summary>
    /// 保存或另存为
    /// </summary>
    public async void SaveButton()
    {
       
       await SaveOrSaveTo();
    }

    private async UniTask SaveOrSaveTo()
    {
        //更新暂存在内存中的清单
        CardMaker.cardMaker.nowEditingBundle.manifest.BundleName = bundleName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName = friendlyName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.BundleVersion = bundleVersion.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.AuthorName = authorName.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Description = description.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Remarks = remark.text;
        CardMaker.cardMaker.nowEditingBundle.manifest.Anime = Anime.text;
        
        

        /*保存和另存为，都是自己保存自己的
         * 卡组清单就只保存清单（此脚本）
         * 卡牌就仅保存正在编辑的那个卡牌
         */
        //执行保存or另存为操作
        //没有预先加载卡包，或者原加载的清单文件不存在了，则另存为
        if (CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath == string.Empty || !File.Exists(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
        {
            await CardMaker.cardMaker.AsyncSaveTo(newImageFullPath);
        }
        //有加载卡包，保存
        else
        {
            await CardMaker.cardMaker.AsyncSave(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath,newImageFullPath, null,null, true, false, null);

        } 
            
            
           
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
        FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", Information.SupportedImageExtension));


        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "选择卡组图片", "选择");


        if (FileBrowser.Success)
        {
            Debug.Log($"加载成功，图片文件{FileBrowser.Result[0]}");
            //加载图片
            var sprite = await CardReadWrite.ManifestLoadImageAsync(FileBrowser.Result[0]);
            sprite = sprite == null ? DefaultImage : sprite;
            bundleImage.sprite = sprite;
            image.sprite = sprite;
            
            //显示已修改的印记
            CardMaker.cardMaker.changeSignal.SetActive(true);
            //更新新的图片全路径，用于保存
            newImageFullPath = FileBrowser.Result[0];
        }
    }


   

#if UNITY_EDITOR

    [ContextMenu("测试")]
    public void test()
    {

       
    }
#endif
    }
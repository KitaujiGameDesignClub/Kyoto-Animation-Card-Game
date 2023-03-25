using System;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using UnityEngine.Serialization;

namespace Maker
{
    public class BundleEditor : MonoBehaviour
    {
        [Header("编辑器")] public LeanToggle OtherContent;
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

        [FormerlySerializedAs("returnToTitle")] [Header("面板")] public LeanButton returnButton;

        /// <summary>
        /// 列举此卡组内所有的卡牌的友好名称
        /// </summary>
        public TMP_Dropdown cardsFriendlyNamesList;

        public LeanButton switchToCardEditor;

        /// <summary>
        /// 新图片的完整路径
        /// </summary>
        string newImageFullPath { get; set; }


        internal void Initialization()
        {
            //保存热键
            CardMaker.cardMaker.WantToSave.AddListener(UniTask.UnityAction(async () => { await SaveOrSaveTo(); }));

            #region 切换事件组

            //返回标题
            returnButton.OnClick.AddListener(delegate
            {
                CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
                {
                    //检查保存（仅限在这个事件中）
                    await SaveOrSaveTo();
                }), delegate { CardMaker.cardMaker.ReturnToCreateEditPanel(); });
            });

            //切换到card editor
            switchToCardEditor.OnClick.AddListener(delegate
            {
                CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
                    {
                        //检查保存（仅限在这个事件中）
                        await SaveOrSaveTo();
                    }),
                    UniTask.Action(async () =>
                    {
                        //切换界面
                        //加载所需资源
                        await LoadSelectedCardOfSwitch();
                        //然后切换
                        await CardMaker.cardMaker.cardEditor.OpenCardEditor();
                        gameObject.SetActive(false);
                    }));
            });

            #endregion
        }

        /// <summary>
        /// 打开manifest editor（同步方法中，一定要放到最后一步）
        /// </summary>
        public async UniTask OpenManifestEditor(bool load = true)
        {
          if(load) CardMaker.cardMaker.BanInputLayer(true, "卡组配置加载中...");

            //获取卡组清单信息
            var manifest = CardMaker.cardMaker.nowEditingBundle.manifest;
            OtherContent.Set(manifest.OtherContent);
            bundleFriendlyName.SetTextWithoutNotify(manifest.FriendlyBundleName);
            authorName.SetTextWithoutNotify(manifest.AuthorName);
            Anime.inputField.SetTextWithoutNotify(manifest.Anime);
            description.SetTextWithoutNotify(manifest.Description);
            remark.SetTextWithoutNotify(manifest.Remarks);
            bundleVersion.SetTextWithoutNotify(manifest.BundleVersion);
            //  shortDescription.text = CardMaker.cardMaker.nowEditingBundle.manifest.shortDescription;


            if (load)
            {
                CardMaker.cardMaker.BanInputLayer(true, "卡组封面加载中...");
                //封面图片加载
                //先设置成默认的
                bundleImage.sprite = DefaultImage;
                //如果是加载的已有卡组，则读取现成的cover图片
                if (CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath != string.Empty)
                {
                    //加载图片
                    await ManifestLoadImageAsync(
                        $"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/{CardMaker.cardMaker.nowEditingBundle.manifest.ImageName}");


                    //新建卡组，找不到图片，就保持刚刚设置的默认图片
                }

            }

            //调整能否切换到卡牌编辑器
            CheckSwitchToCardEditorAvailable();

            //加载（刷新）可变下拉框的内容
            RefreshVariableInputField();

            //兼容性检查
            if (manifest.CodeVersion == Information.ManifestVersion)
            {
                codeVersionCheck.text =
                    $"清单代码版本号：{Information.ManifestVersion}\n编辑器代码版本号：{Information.ManifestVersion}\n<color=green>完全兼容</color>";
            }
            else
            {
                //不兼容就要尝试修复
            }


            //更新一下卡包预览
            OnEndEdit();
            CardMaker.cardMaker.changeSignal.SetActive(false);
            CardMaker.cardMaker.BanInputLayer(false, "所含卡牌缓存中...");

            //启用编辑器
            gameObject.SetActive(true);
        }

        /// <summary>
        /// /加载（刷新）可变下拉框的内容
        /// </summary>
        public void RefreshVariableInputField()
        {
            //初始化Anime（可变输入框/下拉框）的下拉栏
            List<string> all = new();
            for (int i = 0; i < Information.AnimeList.Length; i++)
            {
                all.Add(Information.AnimeList[i]);
            }

            Anime.ChangeOptionDatas(all);
        }


        /// <summary>
        /// 保存或另存为的按钮
        /// </summary>
        public void SaveButton()
        {
            SaveOrSaveTo();
        }


        public void ExportButton()
        {
            if (CardMaker.cardMaker.changeSignal.activeSelf)
            {
                Notify.notify.CreateBannerNotification(null, "在导出之前，需要先保存卡组");
            }
            else
            {
               CardMaker.cardMaker.outputButton.OnClick.Invoke();
            }
        }

        private async UniTask SaveOrSaveTo()
        {
            if (!gameObject.activeSelf && FileBrowser.IsOpen)
            {
                return;
            }

            //更新暂存在内存中的清单
            CardMaker.cardMaker.nowEditingBundle.manifest.OtherContent = OtherContent.On;
            CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName = friendlyName.text;
            CardMaker.cardMaker.nowEditingBundle.manifest.BundleVersion = bundleVersion.text;
            CardMaker.cardMaker.nowEditingBundle.manifest.AuthorName = authorName.text;
            CardMaker.cardMaker.nowEditingBundle.manifest.Description = description.text;
            CardMaker.cardMaker.nowEditingBundle.manifest.Remarks = remark.text;
            CardMaker.cardMaker.nowEditingBundle.manifest.Anime = Anime.text;
            //图片
            CardMaker.cardMaker.nowEditingBundle.manifest.ImageName = string.IsNullOrEmpty(newImageFullPath)
                ? CardMaker.cardMaker.nowEditingBundle.manifest.ImageName
                : $"{Information.DefaultCoverNameWithoutExtension}{Path.GetExtension(newImageFullPath)}";
            //UUID生成
            CardMaker.cardMaker.nowEditingBundle.manifest.UUID =
                string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.manifest.UUID)
                    ? Guid.NewGuid().ToString()
                    : CardMaker.cardMaker.nowEditingBundle.manifest.UUID;

            //记录储存的位置
            CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath =
                $"{Path.Combine(Information.bundlesPath, CardMaker.cardMaker.nowEditingBundle.manifest.UUID)}/{Information.ManifestFileName}";

            //保存逻辑执行
            await CardMaker.cardMaker.AsyncSave(
                Path.GetDirectoryName( CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath),
                newImageFullPath, null, null, true, false, null);

            //允许玩家进行卡牌创建了
            CheckSwitchToCardEditorAvailable();


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

        #region 与卡牌编辑器切换

        /// <summary>
        /// 检查能否切换到卡牌编辑器
        /// </summary>
        void CheckSwitchToCardEditorAvailable()
        {
            cardsFriendlyNamesList.ClearOptions();

            if (string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
            {
                cardsFriendlyNamesList.gameObject.SetActive(false);
                switchToCardEditor.gameObject.SetActive(false);
                return;
            }

            //不是新建的卡牌，就打开切换器
            cardsFriendlyNamesList.gameObject.SetActive(true);
            switchToCardEditor.gameObject.SetActive(true);

            //读取卡组内所有卡牌的友好名称
            cardsFriendlyNamesList.options.Insert(0, new TMP_Dropdown.OptionData("<创建新卡牌>"));
                cardsFriendlyNamesList.AddOptions(CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName);
                cardsFriendlyNamesList.value = 0;
                cardsFriendlyNamesList.RefreshShownValue();

        }

        #endregion

        /// <summary>
        /// 切换编辑器用的，加载所选的卡牌的配置文件
        /// </summary>
        private async UniTask LoadSelectedCardOfSwitch()
        {
            CardMaker.cardMaker.BanInputLayer(true, "切换中...");

            //按照选定的卡牌进行资源加载
            var card = new CharacterCard();

            //选择的value=0，即在此卡组内新建卡牌
            if (cardsFriendlyNamesList.value == 0)
            {
                //但是把新卡牌的Anime设置成与卡组一致（可以在card editor内修改）
                card.Anime = CardMaker.cardMaker.nowEditingBundle.manifest.Anime;
            }
            else
            {
                var manifestPath = CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath;
                var cardDirectoryName =
                    CardMaker.cardMaker.nowEditingBundle.allCardName[cardsFriendlyNamesList.value - 1];

                CardMaker.cardMaker.BanInputLayer(true, "读取卡牌配置...");
                card = await YamlReadWrite.ReadAsync<CharacterCard>(
                    new DescribeFileIO(Information.CardFileName,
                        $"-{Path.GetDirectoryName(manifestPath)}/cards/{cardDirectoryName}"), null, false);

                //保存一下加载的卡牌的路径
                CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath =
                    $"{Path.GetDirectoryName(manifestPath)}/cards/{cardDirectoryName}/{Information.CardFileName}";
            }

            CardMaker.cardMaker.nowEditingBundle.card = card;
        }


        #region 图片加载

        /// <summary>
        /// 玩家选择图片
        /// </summary>
        public void selectImage() => AsyncSelectImage();

        async UniTask AsyncSelectImage()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", Information.SupportedImageExtension));


            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "选择卡组图片",
                "选择");


            if (FileBrowser.Success)
            {
                //加载图片
                await ManifestLoadImageAsync(CardReadWrite.FixedLoadedPathDueToSAF(FileBrowser.Result[0]));

                //显示已修改的印记
                CardMaker.cardMaker.changeSignal.SetActive(true);
                //更新新的图片全路径，用于保存
                newImageFullPath = CardReadWrite.FixedLoadedPathDueToSAF(FileBrowser.Result[0]);
            }
        }

        /// <summary>
        /// （卡组清单用）加载指定路径的图片
        /// </summary>
        /// <param name="imageFullPath">string.Empty时返回空</param>
        /// <returns></returns>
        private async UniTask ManifestLoadImageAsync(string imageFullPath)
        {
            //下载（加载）图片
            var texture2D = await CardReadWrite.CoverImageLoader(imageFullPath);

            //图片不存在或者读取失败，用默认的图片
            if (texture2D == null)
            {
                image.sprite = DefaultImage;
                bundleImage.sprite = DefaultImage;
            }
            //存在的话，调整图片大小，然后应用
            else
            {
                //毕竟是正方形，进行大小调整
                int size;
                //正方形图片就随便取一个边作为图片大小
                if (texture2D.width == texture2D.height)
                {
                    size = texture2D.width;
                }
                //非正方形则取最短边
                else
                {
                    size = texture2D.width > texture2D.height
                        ? texture2D.height
                        : texture2D.width;
                }

                var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, size, size), Vector2.one / 2f);
                image.sprite = sprite;
                bundleImage.sprite = sprite;
            }
        }

        #endregion


#if UNITY_EDITOR

        [ContextMenu("测试")]
        public void test()
        {
        }
#endif
    }
}
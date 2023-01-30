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

namespace Maker
{
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

        [Header("面板")] public LeanButton returnToTitle;

        /// <summary>
        /// 列举此卡组内所有的卡牌的友好名称
        /// </summary>
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
            returnToTitle.OnClick.AddListener(delegate
            {
                CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
                {
                    //检查保存（仅限在这个事件中）
                    await SaveOrSaveTo();
                }), delegate { CardMaker.cardMaker.ReturnToMakerTitle(); });
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
                Debug.Log(
                    $"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/{CardMaker.cardMaker.nowEditingBundle.manifest.ImageName}");
                //加载图片
                var sprite = await ManifestLoadImageAsync(
                    $"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/{CardMaker.cardMaker.nowEditingBundle.manifest.ImageName}");
                sprite = sprite == null ? DefaultImage : sprite;
                bundleImage.sprite = sprite;

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
                cardsFriendlyNamesList.options.Insert(0, new TMP_Dropdown.OptionData("<创建新卡牌>"));
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
            CardMaker.cardMaker.nowEditingBundle.manifest.ImageName = newImageFullPath == string.Empty
                ? CardMaker.cardMaker.nowEditingBundle.manifest.ImageName
                : $"cover{Path.GetExtension(newImageFullPath)}";

            /*保存和另存为，都是自己保存自己的
             * 卡组清单就只保存清单（此脚本）
             * 卡牌就仅保存正在编辑的那个卡牌
             */
            //执行保存or另存为操作
            //没有预先加载卡包，或者原加载的清单文件不存在了，则另存为
            if (string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath) ||
                !File.Exists(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
            {
                await CardMaker.cardMaker.AsyncSaveTo(newImageFullPath);
            }
            //有加载卡包，保存
            else
            {
                await CardMaker.cardMaker.AsyncSave(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath,
                    newImageFullPath, null, null, true, false, null);
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
                Debug.Log($"{card.Anime}   {CardMaker.cardMaker.nowEditingBundle.manifest.Anime}");
            }
            else
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
                Debug.Log($"加载成功，图片文件{FileBrowser.Result[0]}");
                //加载图片
                var sprite = await ManifestLoadImageAsync(FileBrowser.Result[0]);
                sprite = sprite == null ? DefaultImage : sprite;
                bundleImage.sprite = sprite;
                image.sprite = sprite;

                //显示已修改的印记
                CardMaker.cardMaker.changeSignal.SetActive(true);
                //更新新的图片全路径，用于保存
                newImageFullPath = FileBrowser.Result[0];
            }
        }

        /// <summary>
        /// （卡组清单用）加载指定路径的图片
        /// </summary>
        /// <param name="imageFullPath">string.Empty时返回空</param>
        /// <returns></returns>
        private async UniTask<Sprite> ManifestLoadImageAsync(string imageFullPath)
        {
            if (imageFullPath == string.Empty)
            {
                return null;
            }

            if (!File.Exists(imageFullPath))
            {
                Debug.LogError($"图片文件“{imageFullPath}”不存在，已应用默认图片");
                return null;
            }

            newImageFullPath = imageFullPath;

            //下载（加载）图片
            var hander = await CardReadWrite.CoverImageLoader(imageFullPath);

            //图片不存在或者读取失败，返回null
            if (hander == null) return null;
            else
            {
                //毕竟是正方形，进行大小调整
                int size;
                //正方形图片就随便取一个边作为图片大小
                if (hander.width == hander.height)
                {
                    size = hander.width;
                }
                //非正方形则取最短边
                else
                {
                    size = hander.width > hander.height
                        ? hander.height
                        : hander.width;
                }

                var sprite = Sprite.Create(hander, new Rect(0f, 0f, size, size), Vector2.one / 2f);
                return sprite;
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
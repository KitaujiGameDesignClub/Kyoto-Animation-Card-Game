using System;
using System.Collections;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using KitaujiGameDesignClub.GameFramework;
using System.Collections.Generic;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Maker
{
    /// <summary>
    /// 卡包/卡牌制作用
    /// </summary>
    public class CardMaker : MonoBehaviour
    {
        public static CardMaker cardMaker;

        public BasicEvents basicEvents;

        /// <summary>
        /// 内置卡组路径的图标
        /// </summary>
        public Sprite bundlePathIco;


        [Header("界面切换")] public GameObject title;
        [Header("选择编辑界面")] public GameObject EditPanel;

        [FormerlySerializedAs("ManifestEditor")]
        public GameObject ManifestEditorPlane;

        [FormerlySerializedAs("CardEditor")] public GameObject CardEditorPlane;
        public BundleEditor bundleEditor;
        public CardEditor cardEditor;


        /// <summary>
        /// 禁用输入层
        /// </summary>
        [Header("加载与保存")] public GameObject banInput;

        /// <summary>
        /// 修改标记
        /// </summary>
        public GameObject changeSignal;

        /// <summary>
        /// 保存的状态
        /// </summary>
        public TMP_Text saveStatus;


        /// <summary>
        /// 现在正在编辑的卡包（卡组）（包含了清单和卡片配置）
        /// </summary>
        public BundleOfCreate nowEditingBundle = new BundleOfCreate();

        /// <summary>
        /// 游戏加载时的实践
        /// </summary>
        [Header("游戏加载时的实践")] public UnityEvent OnGameLoad;


        private void Awake()
        {
            Information.bundlesPath = $"{YamlReadWrite.UnityButNotAssets}/bundles";


            cardMaker = this;

            //加载游戏的事件
            OnGameLoad.Invoke();

            //初始化界面
            title.SetActive(true);
            ManifestEditorPlane.SetActive(false);
            CardEditorPlane.SetActive(false);

            //关闭修改信号
            changeSignal.SetActive(false);
        }

        private void Start()
        {
            //刷新yaml资源
            refreshYamlRes();

            //隐藏文件选择器
            FileBrowser.HideDialog();
           
            //允许玩家输入
            banInput.SetActive(false);

            //加入一个quickLink
            FileBrowser.AddQuickLink("游戏卡组", $"{Information.bundlesPath}", bundlePathIco);

            //android储存权限检查
            AndroidRequestCheck();

        }

        public void AndroidRequestCheck()
        {
            switch (FileBrowser.CheckPermission())
            {
                case FileBrowser.Permission.Denied:
                    AskPermission("储存权限被拒绝！卡牌编辑器需要储存权限以储存、读取卡组文件\n请按意愿授予");
                    break;

                case FileBrowser.Permission.Granted:
                    break;

                case FileBrowser.Permission.ShouldAsk:
                    AskPermission("卡牌编辑器需要储存权限以储存、读取卡组文件\n请按意愿授予");
                    break;

               
            }
         

            void AskPermission(string content) => Notify.notify.CreateStrongNotification(null, null, "需要储存权限", content, delegate 
            { 
                //按这个按钮，就申请权限
                FileBrowser.RequestPermission();  

                //得到权限就关闭通知
                if(FileBrowser.CheckPermission() == FileBrowser.Permission.Granted)
                {
                    Notify.notify.TurnOffStrongNotification();
                }
                //得不到就算了
                else
                {
                    DeniedPermissionAgain();
                }

            }, "申请", DeniedPermissionAgain, "取消并关闭", allowBackgroundCloseNotification: false);

            //权限申请再次被拒绝，回到游戏主界面
            void DeniedPermissionAgain()
            {
                Notify.notify.CreateStrongNotification(null, delegate
                {
                    Application.Quit(-233);
                }, "储存权限被拒绝", "如果要编辑或制作卡包，需要储存权限\n此通知会与游戏一起关闭",Notify.notify.TurnOffStrongNotification,"确认并关闭");
            }
        }

        public void ReturnToStartMenu()
        {
            nowEditingBundle = null;
            //允许玩家输入
            banInput.SetActive(false);
            //关闭编辑器
            bundleEditor.gameObject.SetActive(false);
            cardEditor.gameObject.SetActive(false);
        }

        /// <summary>
        /// 创建卡包
        /// </summary>
        public void CreateBundle()
        {
            //创建新内容
            nowEditingBundle = new();
            //然后打开编辑器
            bundleEditor.OpenManifestEditor();
        }

        public async void EditBundleButton()
        {
            await EditBundle();
        }

        /// <summary>
        /// 编辑卡组
        /// </summary>
        private async UniTask EditBundle()
        {
            //得到文件内容
            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组清单文件", Information.ManifestExtension));
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡组清单", loadButtonText: "选择");

            if (FileBrowser.Success)
            {
                //现在还没有改内容，关闭修改标记
                changeSignal.SetActive(false);
                nowEditingBundle.loadedManifestFullPath = FileBrowser.Result[0];

                BanInputLayer(true, "读取卡组配置...");
                var bundle = await CardReadWrite.GetOneBundle(FileBrowser.Result[0]);

                nowEditingBundle.manifest = bundle.manifest;

                //缓存所有卡牌的友好和识别名称
                foreach (var variable in bundle.cards)
                {
                    nowEditingBundle.allCardsFriendlyName.Add(variable.FriendlyCardName);
                    nowEditingBundle.allCardsName.Add(variable.CardName);
                }

                Debug.Log($"成功加载卡组“{bundle.manifest.FriendlyBundleName}”，内含{bundle.cards.Length}张卡牌");

                //关闭编辑选择界面
                EditPanel.SetActive(false);

                //然后打开编辑器
                await bundleEditor.OpenManifestEditor();
            }
        }


        public void CreateNewCard()
        {
            //创建新内容
            nowEditingBundle = new();
            //然后打开编辑器
            cardEditor.OpenCardEditor();
        }

        public async void EditCardButton()
        {
            await EditCard();
        }

        private async UniTask EditCard()
        {
            //得到配置文件
            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡牌配置文件", Information.CardExtension));
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡牌配置", loadButtonText: "选择");

            if (FileBrowser.Success)
            {
                nowEditingBundle = new();
                //现在还没有改内容，关闭修改标记
                changeSignal.SetActive(false);

                cardMaker.BanInputLayer(true, "卡牌配置加载中...");
                nowEditingBundle.loadedCardFullPath = FileBrowser.Result[0];

                var card = await YamlReadWrite.ReadAsync(
                    new DescribeFileIO($"{Path.GetFileName(FileBrowser.Result[0])}",
                        $"-{Path.GetDirectoryName(FileBrowser.Result[0])}"), new CharacterCard(), false);

                nowEditingBundle.card = card;

                Debug.Log($"成功加载卡牌“{nowEditingBundle.card.FriendlyCardName}”");

                //关闭编辑选择界面
                EditPanel.SetActive(false);

                //然后打开编辑器
                await cardEditor.OpenCardEditor();
            }
        }


        /// <summary>
        /// 异步保存（清单和卡牌自己保存自己的）
        /// </summary>
        /// <param name="manifestSaveFullPath">manifest储存的路径（含文件名和拓展名）</param>
        /// <param name="manifestNewImageFullPath"></param>
        /// <param name="cardToSaveFullPath">卡牌储存的路径（含文件名和拓展名）</param>
        /// <param name="newCardImageFullPath">新图片所在路径</param>
        /// <param name="saveManifest"></param>
        /// <param name="saveCard"></param>
        /// <param name="cardAudioSettings"></param>
        /// <exception cref="Exception"></exception>
        public async UniTask AsyncSave(string manifestSaveFullPath, string manifestNewImageFullPath,
            string cardToSaveFullPath, string newCardImageFullPath,
            bool saveManifest, bool saveCard, audioSetting[] cardAudioSettings)
        {
            BanInputLayer(true, "保存中...");

            if (saveManifest)
            {
                saveStatus.text = "卡组清单保存中...";

                try
                {
                    Debug.Log(manifestSaveFullPath);
                    await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest, manifestSaveFullPath,
                        manifestNewImageFullPath);
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    banInput.SetActive(false);
                    throw e;
                }
            }

            if (saveCard)
            {
                saveStatus.text = "卡牌保存中...";

                try
                {
                    //音频路径与文件名获取
                    var newAudiosFullPath = new string[cardAudioSettings.Length];
                    var audioNamesWithoutExtension = new string[cardAudioSettings.Length];
                    for (int i = 0; i < newAudiosFullPath.Length; i++)
                    {
                        newAudiosFullPath[i] = cardAudioSettings[i].newAudioFullFileName;
                        audioNamesWithoutExtension[i] = cardAudioSettings[i].VoiceName;
                    }

                    await CardReadWrite.CreateCardFile(nowEditingBundle.card, cardToSaveFullPath, newCardImageFullPath,
                        newAudiosFullPath, audioNamesWithoutExtension);
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    banInput.SetActive(false);
                    throw e;
                }
            }

            //关闭输入禁用层
            banInput.SetActive(false);
            cardMaker.changeSignal.SetActive(false);
        }

        /// <summary>
        /// 卡组清单另存为（返回值为保存的路径，仅DirectoryName）
        /// </summary>
        /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
        /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
        /// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
        /// <param name="saveManifest"></param>
        /// <param name="saveCard"></param>
        /// <returns>保存成功了吗？</returns>
        public async UniTask<string> AsyncManifestSaveTo(string manifestNewImageFullPath)
        {
            //执行保存逻辑
          return  await AsyncSaveTo(manifestNewImageFullPath, null, true, false, null);
           
        }

        /// <summary>
        /// 卡牌另存为
        /// </summary>
        /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
        /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
        /// <param name="cardNewImageFullPath">卡牌的新图片的全路径</param>
        /// <param name="saveManifest"></param>
        /// <param name="saveCard"></param>
        /// <returns>保存成功了吗？</returns>
        public async UniTask AsyncCardSaveTo(string cardNewImageFullPath, audioSetting[] cardAudioSettins)
        {
            await AsyncSaveTo(null, cardNewImageFullPath, false, true, cardAudioSettins);
        }

        /// <summary>
        /// 通用的另存为（返回值为保存的路径，仅DirectoryName）
        /// </summary>
        /// <param name="manifestNewImageFullPath">清单文件的新图片的全路径</param>
        /// <param name="index">卡牌文件，在卡组内是第几张牌？</param>
        /// <param name="newCardImageFullPath">卡牌的新图片的全路径</param>
        /// <param name="saveManifest"></param>
        /// <param name="saveCard"></param>
        /// <returns>保存成功了吗？</returns>
        public async UniTask<string> AsyncSaveTo(string manifestNewImageFullPath, string newCardImageFullPath,
            bool saveManifest, bool saveCard, audioSetting[] cardAudioSettings)
        {
            //还没有保存过/不是打开编辑卡包，打开选择文件的窗口，选择保存位置

            await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, false, title: "另存为",
                saveButtonText: "选择文件夹");

            //开启输入禁用层
            BanInputLayer(true, "另存为中...");


            if (FileBrowser.Success)
            {
                //保存
                await AsyncSave(
                    $"{FileBrowser.Result[0]}/{nowEditingBundle.manifest.BundleName}/{nowEditingBundle.manifest.BundleName}{Information.ManifestExtension}",
                    manifestNewImageFullPath,
                    $"{FileBrowser.Result[0]}/{nowEditingBundle.card.CardName}/{nowEditingBundle.card.CardName}{Information.CardExtension}",
                    newCardImageFullPath, saveManifest, saveCard,
                    cardAudioSettings);

                return FileBrowser.Result[0];
            }
            else
            {
                //关闭禁用曾
                banInput.SetActive(false);
                return null;
            }

           
        }


        /// <summary>
        /// 输入禁用层
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="textContent"></param>
        public void BanInputLayer(bool enable, string textContent)
        {
            //调整开关
            banInput.SetActive(enable);
            saveStatus.text = textContent;
        }


        #region 界面切换（返回，编辑器间切换）和打开文档

        public void OpenHelpDocument() => basicEvents.OpenURL("https://shimo.im/docs/N2A1M7mzZ4S0NRAD");

        public void ExitGame() => basicEvents.ExitGame();

        public void SwitchPanel(Action saveOrSaveTo, Action doWhat)
        {
            //修改信息被激活，说明修改了，提示要不要保存后在返回
            if (CardMaker.cardMaker.changeSignal.activeSelf)
            {
                Notify.notify.CreateStrongNotification(null, null, "修改尚未保存", "对此卡配置文件的修改尚未保存，要保存吗？", delegate
                {
                    //保存，并停留在编辑器界面

                    //关闭通知
                    Notify.notify.TurnOffStrongNotification();
                    //弹出保存界面
                    saveOrSaveTo.Invoke(); //用的Unitask.Action，确实能在这里等待
                    //执行该做的事情
                    doWhat.Invoke();
                }, "保存", delegate
                {
                    //不保存，且回到Maker标题

                    //关闭通知
                    Notify.notify.TurnOffStrongNotification();
                    //执行该做的事情
                    doWhat.Invoke();
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
                //执行该做的事情
                doWhat.Invoke();
            }
        }

        /// <summary>
        /// 回到编辑器标题界面
        /// </summary>
        public void ReturnToMakerTitle()
        {
            changeSignal.SetActive(false);
            CardEditorPlane.SetActive(false);
            ManifestEditorPlane.SetActive(false);
            title.SetActive(true);
        }

        #endregion


        /// <summary>
        /// 刷新yaml资源（tag anime cv列表，从本地文件中读取）
        /// </summary>
        public void refreshYamlRes() => CardReadWrite.refreshYamlResFromDisk();


#if UNITY_EDITOR
        [ContextMenu("各类测试")]
        public void test()
        {
            var s = new List<string>();
            s.Add("dd");
            s.Add("dd");
            s.Add("dw");
            s.Add("dd");
            Debug.Log(s.Count);
            s.RemoveAll(d => d == "dd");
            Debug.Log(s.Count); //把dd都删掉了
        }
#endif
    }
}
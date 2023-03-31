using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework;
using KitaujiGameDesignClub.GameFramework.Tools;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
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

        public TMP_Text VersionField;

        
        [Header("界面切换")] public GameObject title;
        [FormerlySerializedAs("InternalPanel")][FormerlySerializedAs("EditPanel")] public GameObject EditAndCreatePanel;
        [FormerlySerializedAs("ExternalPanel")] public GameObject ExportAndImportPanel;

        //两个编辑器  
        [FormerlySerializedAs("ManifestEditor")]
        public GameObject ManifestEditorPlane;
        public GameObject BundleProductPreview;

        [FormerlySerializedAs("CardEditor")] public GameObject CardEditorPlane;
        public BundleEditor bundleEditor;
        public CardEditor cardEditor;
        public GameObject CardProductPreview;

        //编辑创建用
        [FormerlySerializedAs("allAvailableBundles")]
        [Header("编辑创建用")]
        public LeanButton editButton;
        public LeanButton createButton;
        public TMP_Dropdown allAvailableBundlesDropdownToEdit;
        public Button DeleteThisBundleButton;
        [Header("导入导出用")]
        public LeanButton outputButton;
        public LeanButton inputButton;
        [FormerlySerializedAs("allAvailableBundlesDropdown")] public TMP_Dropdown allAvailableBundlesDropdownToExport;


        private List<string> allBundleLoadedGUID = new();

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


        #region 键盘输入事件

        /// <summary>
        /// 按下ctrl了吗？
        /// </summary>
        private bool controlPressed = false;
        /// <summary>
        /// 上次激活热键后，松开了吗
        /// </summary>
        private bool releaseButton = true;

        #endregion

        private void Awake()
        {
            var ss = Application.version.Contains("weekly") ? $"{Application.version}\n<color=red><size=130%>此为每周版，很可能有相当多的bug，在操作卡组之前请先备份" : Application.version;
            VersionField.text = $"游戏版本：ver.{ss}";

            //创建bundle文件夹
            if (!File.Exists(Information.bundlesPath)) Directory.CreateDirectory(Information.bundlesPath);

            //帧率修正
            Application.targetFrameRate = 60;
#if UNITY_ANDROID
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen,
                60);
#endif

            cardMaker = this;

            //加载游戏的事件
            OnGameLoad.Invoke();

            //运行编辑器的初始化方法
            cardEditor.Initialization();
            bundleEditor.Initialization();


            //关闭修改信号
            changeSignal.SetActive(false);


        }

        private void Start()
        {
            //初始化界面
            title.SetActive(true);
            ManifestEditorPlane.SetActive(false);
            CardEditorPlane.SetActive(false);
            ExportAndImportPanel.SetActive(false);
            EditAndCreatePanel.SetActive(false);

            //刷新yaml资源
            refreshYamlRes();

            //隐藏文件选择器
            FileBrowser.HideDialog();

            //允许玩家输入
            banInput.SetActive(false);


            #region 事件注册

            //外部导入导出用
            outputButton.OnClick.AddListener(UniTask.UnityAction(async () => { await Export(); }));
            inputButton.OnClick.AddListener(UniTask.UnityAction(async () => { await Import(); }));

            //内部编辑创建用
            createButton.OnClick.AddListener(CreateBundle);
            editButton.OnClick.AddListener(EditBundleButton);
            DeleteThisBundleButton.onClick.AddListener(delegate
            {
                DeletionBundle(allBundleLoadedGUID[allAvailableBundlesDropdownToEdit.value],
                    allAvailableBundlesDropdownToEdit.captionText.text);
            });

            #endregion

            //刷新已经加载了的卡组
            RefreshAllLoadedBundle();

            //检验第一次打开游戏？
            if (!File.Exists(Path.Combine(YamlReadWrite.UnityButNotAssets, "used")))
            {
                File.Create(Path.Combine(YamlReadWrite.UnityButNotAssets, "used"));

                //第一次打开游戏的话，提示一下不用给外部储存你权限
#if UNITY_ANDROID && !UNITY_EDITOR
             Notify.notify.CreateStrongNotification(null, delegate () { FileBrowser.RequestPermission(); }, "欢迎使用编辑器", "这是你第一次打开。\n我们不需要外部储存权限\n因此在接下来的弹窗中，请拒绝", delegate () { });

#endif
            }




        }

        private void Update()
        {
            #region 键盘输入监听

            if (!banInput.activeSelf && releaseButton)
            {
                controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

                //保存ctrl+s
                if (Input.GetKey(KeyCode.S) && controlPressed)
                {
                    releaseButton = false;
                   
                    if(ManifestEditorPlane.activeSelf) 
                    {
                        bundleEditor.SaveButton();
                    }
                    else if(CardEditorPlane.activeSelf) 
                    {
                        cardEditor.SaveButton();
                    }

                }
                //打开根目录
                if (Input.GetKey(KeyCode.E) && controlPressed)
                {
                    releaseButton = false;
                    Application.OpenURL(YamlReadWrite.UnityButNotAssets);
                }
                //刷新字典缓存
                if (Input.GetKey(KeyCode.R) && controlPressed)
                {
                    releaseButton = false;
                    RefreshDictionariesAndUpdateVariableInputField();
                }
            }
            else
            {
                releaseButton = !Input.anyKey;
            }

            #endregion
        }

        #region 字典的导入导出

        /// <summary>
        /// 字典初始化
        /// </summary>
        /// <param name="dictionaryType"></param>
        public void DictionaryInitialize(int dictionaryType)
        {
            Notify.notify.CreateStrongNotification(null, null, "要重置字典吗？", "这会使<b><size=110%>此字典</size></b>恢复到最初的状态\n重置后编辑器需要重启", delegate ()
            {
                var path =
    $"{YamlReadWrite.UnityButNotAssets}/saves/{((Information.DictionaryType)dictionaryType).ToString()}.yml";
                Debug.Log(path);
                if (File.Exists(path)) File.Delete(path);
                ExitGame();

            }, "确认", delegate () { },"取消");


        }

        public async void DictionaryExport(int dictionaryType)
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("字典文件", ".yml"));
            await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, true, title: "导出字典", saveButtonText: "选择");

            if (FileBrowser.Success)
            {
                CardReadWrite.ExportDictionary(FileBrowser.Result[0], $"{((Information.DictionaryType)dictionaryType).ToString()}.yml");

                Notify.notify.CreateBannerNotification(null, $"字典导出成功");
            }
        }

        public async void DictionaryImport(int dictionaryType)
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("字典文件", ".yml"));
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, title: "导出字典", loadButtonText: "选择");

            if (FileBrowser.Success)
            {
                CardReadWrite.ImportDictionary(FileBrowser.Result[0], (Information.DictionaryType)dictionaryType);
                Notify.notify.CreateBannerNotification(null, $"字典导入成功");

                //重新读取字典文件
                switch (dictionaryType)
                {
                    case 0:
                        CardReadWrite.ReadAnimeList();
                        break;
                    case 1:
                        CardReadWrite.ReadCV();
                        break;
                    case 2:
                        CardReadWrite.ReadCharacterNames();
                        break;
                    case 3:
                        CardReadWrite.ReadTags();
                        break;
                }

            }
        }

        #endregion

        #region 外部卡组的导入与导出，和他的面板切换


        /// <summary>
        /// 导出卡组
        /// </summary>
        internal async UniTask Export()
        {
            if (allAvailableBundlesDropdownToExport.options.Count <= 0)
            {
                return;
            }


            //还没加载所有已经加载的卡组

            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组", ".zip"));

            await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, true, title: "导出卡组",
                saveButtonText: "选择");

            if (FileBrowser.Success)
            {

                var selectedBundlePath =
                    $"{Information.bundlesPath}/{allBundleLoadedGUID[allAvailableBundlesDropdownToExport.value]}/{Information.ManifestFileName}";


#if UNITY_STANDALONE || UNITY_EDITOR

                var rawSavePathNoExtension =
                    $"{FileBrowser.Result[0]}/{CommonTools.CleanInvalid(allAvailableBundlesDropdownToExport.captionText.text)}";
                //不断循环，在文件名后面加数字，直到不重名为止
                var savePathNoRepeat = rawSavePathNoExtension;

                int i = 0;
                while (true)
                {
                    i++;
                    if (FileBrowserHelpers.FileExists($"{savePathNoRepeat}.zip"))
                    {
                        savePathNoRepeat = $"{rawSavePathNoExtension}-{i}";
                    }
                    else
                    {
                        //导出
                        CardReadWrite.ExportBundleZip(Path.GetDirectoryName($"{savePathNoRepeat}.zip"), Path.GetFileName($"{savePathNoRepeat}.zip"), selectedBundlePath);
                        break;
                    }
                }

                Notify.notify.CreateBannerNotification(null,
                    $"“{allAvailableBundlesDropdownToExport.captionText.text}”卡组导出成功\n{savePathNoRepeat}.zip");

#elif UNITY_ANDROID && !UNITY_EDITOR


                    CardReadWrite.ExportBundleZip(FileBrowser.Result[0],
                        $"{CommonTools.CleanInvalid(allAvailableBundlesDropdownToExport.captionText.text)}.zip",
                        selectedBundlePath);
                    
                     Notify.notify.CreateBannerNotification(null,
                        $"“{allAvailableBundlesDropdownToExport.captionText.text}”导出成功");

#endif




                GC.Collect();
                await Resources.UnloadUnusedAssets();


            }
        }

        /// <summary>
        /// 导入卡组
        /// </summary>
        private async UniTask Import()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡组", ".zip"));

            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, title: "导入卡组", loadButtonText: "选择");

            BanInputLayer(true, "卡组导入中...");

            if (FileBrowser.Success)
            {
                try
                {
                    CardReadWrite.ImportBundleZip(FileBrowser.Result[0]);
                    await RefreshAllLoadedBundle();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

            BanInputLayer(false, "卡组导入中...");
        }


        #endregion


        #region 内部卡组编辑、创建与删除

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

            //可以了，就开始编辑吧
            await EditBundle();

        }

        /// <summary>
        /// 编辑卡组
        /// </summary>
        private async UniTask EditBundle()
        {
            if (allAvailableBundlesDropdownToEdit.options.Count > 0)
            {

                //现在还没有改内容，关闭修改标记
                changeSignal.SetActive(false);

                //开始读取
                BanInputLayer(true, "读取卡组配置...");
                var bundle = new Bundle();

                //选定要编辑卡组manifest的路径
                var manifestFullPath =
                    $"{Information.bundlesPath}/{allBundleLoadedGUID[allAvailableBundlesDropdownToEdit.value]}/{Information.ManifestFileName}";

                //直接读的yml文件
                bundle = await CardReadWrite.GetOneBundle(manifestFullPath);
                nowEditingBundle.loadedManifestFullPath = manifestFullPath;


                nowEditingBundle.manifest = bundle.manifest;

                //缓存所有卡牌的友好和识别名称还有UUID
                nowEditingBundle.ClearAllCardCache();
                foreach (var variable in bundle.cards)
                {
                    nowEditingBundle.allCardsFriendlyName.Add(variable.FriendlyCardName);
                    nowEditingBundle.allCardName.Add(variable.CardName);
                    nowEditingBundle.allCardUuid.Add(variable.UUID);
                }

                Debug.Log($"成功加载卡组“{bundle.manifest.FriendlyBundleName}”，内含{bundle.cards.Length}张卡牌");

                //关闭编辑选择界面
                EditAndCreatePanel.SetActive(false);

                //然后打开编辑器
                await bundleEditor.OpenManifestEditor();

            }

        }

        /// <summary>
        /// 删除选定的卡组
        /// </summary>
        public void DeletionBundle(string bundleName, string bundleFriendlyName)
        {
            if (allAvailableBundlesDropdownToEdit.options.Count > 0)
            {
                Notify.notify.CreateStrongNotification(null, null, "确认删除？",
                    $"此过程不可撤销。\n要删除“{bundleFriendlyName}”吗？", UniTask.UnityAction(
                        async () =>
                        {
                            BanInputLayer(true, "删除中...");
                            //先删除
                            Directory.Delete(
                                $"{Information.bundlesPath}/{bundleName}",
                                true);

                            Notify.notify.CreateBannerNotification(null, "删除成功", 0.7f);

                            //然后刷新
                            await RefreshAllLoadedBundle();
                        }), "确认删除", delegate { }, "再想想");
            }
        }

#if UNITY_EDITOR
        #region 废弃的 卡牌单独编辑


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
#if UNITY_EDITOR || UNITY_STANDALONE
            FileBrowser.SetFilters(false, new FileBrowser.Filter("卡牌配置文件", Path.GetExtension(Information.CardFileName)));
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "加载卡牌配置", loadButtonText: "选择");

            //android的话，不允许读取单个卡牌
#elif UNITY_ANDROID
          Notify.notify.CreateBannerNotification(null,$"{Application.platform}不支持读取单独的卡牌。");
              return;
#endif



            if (FileBrowser.Success)
            {
                nowEditingBundle = new();
                //现在还没有改内容，关闭修改标记
                changeSignal.SetActive(false);

                cardMaker.BanInputLayer(true, "卡牌配置加载中...");
                nowEditingBundle.loadedCardFullPath = FileBrowser.Result[0];

                nowEditingBundle.card = await CardReadWrite.GetOneCard(FileBrowser.Result[0], true);

                //关闭编辑选择界面
                EditAndCreatePanel.SetActive(false);

                //然后打开编辑器
                await cardEditor.OpenCardEditor();
            }
        }

        #endregion
#endif



        #endregion

        private void RefreshDictionariesAndUpdateVariableInputField()
        {
            BanInputLayer(true, "字典重载中...");
            //重新读取
            CardReadWrite.ReloadDictionaries();
            //刷新可变下拉框
            cardEditor.RefreshVariableDropdownList();
            bundleEditor.RefreshVariableInputField();
            BanInputLayer(false, "字典重载中...");
            Notify.notify.CreateBannerNotification(null, "字典重载成功", 0.8f);
        }

        #region  保存、另存为

        /// <summary>
        /// 异步保存（清单和卡牌自己保存自己的）
        /// </summary>
        /// <param name="manifestDirectoryToSave">manifest储存的路径（仅文件夹）</param>
        /// <param name="manifestNewImageFullPath"></param>
        /// <param name="cardDirectoryToSave">卡牌储存的路径</param>
        /// <param name="newCardImageFullPath">新图片所在路径</param>
        /// <param name="saveManifest"></param>
        /// <param name="saveCard"></param>
        /// <param name="cardAudioSettings"></param>
        /// <exception cref="Exception"></exception>
        public async UniTask AsyncSave(string manifestDirectoryToSave, string manifestNewImageFullPath,
            string cardDirectoryToSave, string newCardImageFullPath,
            bool saveManifest, bool saveCard, audioSetting[] cardAudioSettings)
        {
            BanInputLayer(true, "保存中...");

            //修复少的文件夹（用不到SAF，因为android的保存操作只在缓存中
            if (saveManifest)
            {
                saveStatus.text = "卡组清单保存中...";

                try
                {
                    await CardReadWrite.CreateBundleManifestFile(nowEditingBundle.manifest, manifestDirectoryToSave,
                        manifestNewImageFullPath);

                    //刷新已经加载卡组的缓存
                    await RefreshAllLoadedBundle();

                    Notify.notify.CreateBannerNotification(null, $"清单保存完成");
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    BanInputLayer(false, "保存中...");
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

                    await CardReadWrite.CreateCardFile(nowEditingBundle.card, cardDirectoryToSave, newCardImageFullPath,
                        newAudiosFullPath, audioNamesWithoutExtension);

                    Notify.notify.CreateBannerNotification(null, $"卡牌保存完成");
                }
                catch (Exception e)
                {
                    Notify.notify.CreateBannerNotification(delegate { banInput.SetActive(false); },
                        "文件储存错误，详细信息请看控制台");
                    BanInputLayer(false, "保存中...");
                    throw e;
                }
            }

            BanInputLayer(true, "清理中...");
            GC.Collect();
            await Resources.UnloadUnusedAssets();

            //关闭输入禁用层
            BanInputLayer(false, "保存中...");
            changeSignal.SetActive(false);

        }
        #endregion


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
            //关闭or启用成品预览
            BundleProductPreview.SetActive(!enable);
            CardProductPreview.SetActive(!enable);
        }

        private async UniTask RefreshAllLoadedBundle()
        {
            BanInputLayer(true, "卡组刷新中...");

            //获取所有卡组
            var allCards = await CardReadWrite.GetAllBundles();
            allAvailableBundlesDropdownToExport.ClearOptions();
            allAvailableBundlesDropdownToEdit.ClearOptions();
            allBundleLoadedGUID.Clear();

            if (allCards.Length == 0)
            {

                DeleteThisBundleButton.interactable = false;

            }
            else
            {
                //填充选择用的下拉栏
                var options = new List<TMP_Dropdown.OptionData>();
                foreach (var VARIABLE in allCards)
                {
                    options.Add(new TMP_Dropdown.OptionData(VARIABLE.manifest.FriendlyBundleName));
                    //顺便记录一下识别名称
                    allBundleLoadedGUID.Add(VARIABLE.manifest.UUID);
                }
                allAvailableBundlesDropdownToExport.AddOptions(options);
                allAvailableBundlesDropdownToEdit.options = allAvailableBundlesDropdownToExport.options;
                DeleteThisBundleButton.interactable = true;
            }


            BanInputLayer(false, "卡组刷新中...");

        }

        #region 界面切换（返回，编辑器间切换）和打开帮助文档

        public void OpenHelpDocument() => basicEvents.OpenURL("https://shimo.im/docs/N2A1M7mzZ4S0NRAD");
        public void ExitGame()
        {
         
            //清除缓存
            ClearCache(false);
            basicEvents.ExitGame();
        }

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
            EditAndCreatePanel.SetActive(false);
            ExportAndImportPanel.SetActive(false);
        }

        public void ReturnToCreateEditPanel()
        {
            changeSignal.SetActive(false);
            CardEditorPlane.SetActive(false);
            ManifestEditorPlane.SetActive(false);
            title.SetActive(false);
            EditAndCreatePanel.SetActive(true);
            ExportAndImportPanel.SetActive(false);
        }

        #endregion

        public void ClearCache(bool notify)
        {
            BanInputLayer(true, "清除缓存中...");
            // 判断目录是否存在
            if (Directory.Exists(Application.temporaryCachePath))
            {
                DirectoryInfo info = new DirectoryInfo(Application.temporaryCachePath);
                long size = 0;

                foreach (var item in info.GetFiles())
                {
                    size += item.Length;
                }

                // 删除目录及其所有内容
                Directory.Delete(Application.temporaryCachePath, true);

             if(notify)  Notify.notify.CreateBannerNotification(null, $"缓存清除：{(size / 1024 / 1024f).ToString("F2")}MiB");
            }

            Directory.CreateDirectory(Application.temporaryCachePath);

          

            BanInputLayer(false, "清除缓存中...");
        }

        /// <summary>
        /// 刷新yaml资源（tag anime cv列表，从本地文件中读取）
        /// </summary>
        public void refreshYamlRes() => CardReadWrite.ReloadDictionaries();


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
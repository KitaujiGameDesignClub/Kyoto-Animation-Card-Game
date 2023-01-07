using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

public class CardEditor : MonoBehaviour
{
    public GameObject editor;
    public GameObject abilityEditor;
    public GameObject abilityDescriptionEditor;
    public GameObject voiceEditor;
    
    
    [Header("信息侧")] public TMP_InputField cardNameField;
    public TMP_InputField friendlyNameField;
    public InputFieldWithDropdown AnimeField;
    public InputFieldWithDropdown CharacterNameField;
    public InputFieldWithDropdown CVField;
    public TMP_InputField cardNumberField;
    public TMP_Dropdown genderField;
    public TMP_InputField basicPower;
    public TMP_InputField basicHp;
    public Image imageOfCardField;
    public LeanToggle AsChiefToggle;
    public Sprite defaultImage;

    [Header("标签侧")] public InputFieldWithDropdown tagField;
    /// <summary>
    /// tag list里所有的按钮的预设
    /// </summary>
    public tagListItem tagListButtonPerfebs;
    public RectTransform tagParent;
    /// <summary>
    /// 暂存tag
    /// </summary>
    private List<string> tagStorage = new();


    [Header("原因侧")] public TMP_Dropdown abilityReasonType;
    public TMP_Dropdown abilityReasonLargeScope;
    public TMP_Dropdown abilityReasonParameter;
    public TMP_Dropdown abilityReasonLogic;
    public InputFieldWithDropdown abilityReasonThreshold;
    public TMP_Dropdown abilityReasonJudgeParameter;
    public TMP_Dropdown abilityReasonJudgeMethod;
    public TMP_Dropdown abilityReasonJudgeLogic;
    public InputFieldWithDropdown abilityReasonJudgeThreshold;
    [Header("效果侧")] public Lean.Gui.LeanToggle abilityReasonObjectAsTarget;
    private LeanButton abilityReasonObjectAsTargetButton;
    public TMP_Dropdown abilityResultLargeScope;
    public TMP_Dropdown abilityResultParameter;
    public TMP_Dropdown abilityResultLogic;
    public InputFieldWithDropdown abilityResultThreshold;
    public TMP_Dropdown abilityResultParameterToChange;
    public InputFieldWithDropdown abilityResultSummon;
    public TMP_InputField abilityResultRidicule;
    public TMP_InputField abilityResultSilence;
    public TMP_Dropdown abilityResultChangeMethod;
    public InputFieldWithDropdown abilityResultChangeValue;
    [Header("描述侧")] public Lean.Gui.LeanButton Auto;
    public Lean.Gui.LeanButton Clear;
    public TMP_InputField abilityDescription;
    public LeanToggle autoGenerate;
    [Header("音频侧")] public audioSetting voiceDebut;
    [FormerlySerializedAs("voiceKill")] public audioSetting voiceDefeat;
    public audioSetting voiceAbility;
    public audioSetting voiceExit;
    
    [Header("交互控件")]
    public LeanButton returnToTitle;
    
    public LeanButton switchToBundleEditor;
    
    [Space()] public CardPanel preview;

    
    
    /// <summary>
    /// 现在显示的 正在编辑的卡牌
    /// </summary>
    private CharacterCard nowEditingCard { get; set; }

    /// <summary>
    /// 新的图片所在路径
    /// </summary>
    private string newImageFullPath = string.Empty;

    /// <summary>
    /// 卡组内第几张卡？
    /// </summary>
    private int index = 0;
    
    private void Awake()
    {
        abilityReasonObjectAsTargetButton = abilityReasonObjectAsTarget.GetComponent<LeanButton>();
        
        #region 能力编辑初始化（不可变下拉栏初始化)

        abilityReasonType.ClearOptions();
        var length = Enum.GetNames(typeof(Information.CardAbilityTypes)).Length;
        for (int i = 0; i < length; i++)
        {
            abilityReasonType.options.Add(
                new TMP_Dropdown.OptionData(Information.AbilityChineseIntroduction((Information.CardAbilityTypes)i)));
        }

        abilityReasonLargeScope.ClearOptions();
        abilityResultLargeScope.ClearOptions();
        length = Enum.GetNames(typeof(Information.Objects)).Length;
        for (int i = 0; i < length; i++)
        {
            abilityReasonLargeScope.options.Add(
                new TMP_Dropdown.OptionData(
                    Information.AbilityChineseIntroduction((Information.Objects)i)));
        }

        abilityResultLargeScope.options = abilityReasonLargeScope.options;

        abilityReasonParameter.ClearOptions();
        abilityResultParameter.ClearOptions();
        abilityResultParameterToChange.ClearOptions();
        abilityReasonJudgeParameter.ClearOptions();
        length = Enum.GetNames(typeof(Information.Parameter)).Length;
        for (int i = 0; i < length; i++)
        {
            abilityReasonParameter.options.Add(
                new TMP_Dropdown.OptionData(
                    Information.AbilityChineseIntroduction((Information.Parameter)i)));
        }

        abilityReasonJudgeParameter.options = abilityReasonParameter.options;
        abilityResultParameter.options = abilityReasonParameter.options;
        abilityResultParameterToChange.options = abilityReasonParameter.options;

        abilityReasonLogic.ClearOptions();
        abilityResultLogic.ClearOptions();
        abilityReasonJudgeLogic.ClearOptions();
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("不等于/不包含"));
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("小于"));
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("小于等于"));
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("等于/包含"));
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("大于等于"));
        abilityReasonLogic.options.Add(new TMP_Dropdown.OptionData("大于"));
        abilityResultLogic.options = abilityReasonLogic.options;
        abilityReasonJudgeLogic.options = abilityReasonLogic.options;

        abilityResultChangeMethod.ClearOptions();
        length = Enum.GetNames(typeof(Information.CalculationMethod)).Length;
        for (int i = 0; i < length; i++)
        {
            abilityResultChangeMethod.options.Add(
                new TMP_Dropdown.OptionData(
                    Information.AbilityChineseIntroduction((Information.CalculationMethod)i)));
        }


        abilityReasonJudgeMethod.ClearOptions();
        length = Enum.GetNames(typeof(Information.JudgeMethod)).Length;
        for (int i = 0; i < length; i++)
        {
            abilityReasonJudgeMethod.options.Add(
                new TMP_Dropdown.OptionData(
                    Information.AbilityChineseIntroduction((Information.JudgeMethod)i)));
        }

        #endregion

        #region 事件注册

        //当“判定参数”变化时，同步更新“判定阈值”的辅助下拉框内容
        
       abilityReasonJudgeParameter.onValueChanged.AddListener(delegate(int arg0)
        {
            inputFieldHelperContent(abilityReasonJudgeThreshold, arg0);
        });
        //当“修范围数”变化时，同步更新“范围阈值”的辅助下拉框内容
        abilityReasonParameter.onValueChanged.AddListener(delegate(int arg0)
        {
            inputFieldHelperContent(abilityReasonThreshold, arg0);
        });
        //当“修改参数”变化时，同步更新“值”的辅助下拉框内容
       abilityResultParameterToChange.onValueChanged.AddListener(delegate(int arg0)
        {
            inputFieldHelperContent(abilityResultChangeValue, arg0);
        });
        //当“判定参数”变化时，同步更新“判定阈值”的辅助下拉框内容
        abilityResultParameter.onValueChanged.AddListener(delegate(int arg0)
        {
            inputFieldHelperContent(abilityResultThreshold, arg0);
        });
        
        //参数、能力类型可以选择类似于None的选项，如果选定了，则禁用一些输入内容
        abilityReasonType.onValueChanged.AddListener(delegate(int arg0)
        {
            banAllInputFieldInteraction(arg0 != 0,0);
        });
        
        //当将触发器作为处理对象时，禁用部分结果输入
        abilityReasonObjectAsTarget.OnOn.AddListener(delegate
        {
            abilityResultLargeScope.interactable = false;
            abilityResultParameter.interactable = false;
            abilityResultLogic.interactable = false;
            abilityResultThreshold.interactable = false;
            
        });
        abilityReasonObjectAsTarget.OnOff.AddListener(delegate
        {
            abilityResultLargeScope.interactable = true;
            abilityResultParameter.interactable = true;
            abilityResultLogic.interactable = true;
            abilityResultThreshold.interactable = true;
            
        });
        
        //ability描述点击 清楚内容 按钮，成品预览那边同步更新
      abilityDescription.onValueChanged.AddListener(delegate(string arg0)
      {
          var text = arg0.Replace("<rb>","<color=red><b>");
          text = text.Replace("<g>", "<#00FF25>");
          text = text.Replace("<bl>", "<#0158B7>");
          text = text.Replace("</rb>", "</color>");
          text = text.Replace("</g>", "</color>");
          text = text.Replace("</bl>", "</color>");
          preview.description.text = text;
      });
      
      //音频修改事件
      voiceAbility.OnPrepareToSelectAudio.AddListener(SelectAudio);
      voiceDebut.OnPrepareToSelectAudio.AddListener(SelectAudio);
      voiceExit.OnPrepareToSelectAudio.AddListener(SelectAudio);
      voiceDefeat.OnPrepareToSelectAudio.AddListener(SelectAudio);
    
      
      
        #endregion
        
       
    }
    
    private void Start()
    {
        //返回标题
        returnToTitle.OnClick.AddListener(delegate
        {
            CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
            {
                //检查保存（仅限在这个事件中）
                await SaveOrSaveTo();
            }), delegate { CardMaker.cardMaker.ReturnToMakerTitle(); });
        });

        
     //切换到清单编辑器
     switchToBundleEditor.OnClick.AddListener(delegate
     {
          
         CardMaker.cardMaker.SwitchPanel(UniTask.Action(async () =>
             {
                 //检查保存（仅限在这个事件中）
                 await SaveOrSaveTo();
             }), 
             UniTask.Action(async () =>
             {
                 //切换界面

                 //然后切换
                 await CardMaker.cardMaker.bundleEditor.OpenManifestEditor();
                 gameObject.SetActive(false);
             }));
          
     });

        //音频英文标准名称设定
        voiceExit.VoiceName = nameof(voiceExit);
        voiceDefeat.VoiceName = nameof(voiceDefeat);
        voiceAbility.VoiceName = nameof(voiceAbility);
        voiceDebut.VoiceName = nameof(voiceDebut);




    }


    public async UniTask OpenCardEditor()
    {
        //如果没有加载了的manifest，则禁用“切换到manifest editor”功能
        switchToBundleEditor.gameObject.SetActive(!string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath));
        


        CardMaker.cardMaker.BanInputLayer(true, "卡牌配置加载中...");
        
        nowEditingCard = CardMaker.cardMaker.nowEditingBundle.card;
      
        if (nowEditingCard == null)
        {
            Notify.notify.CreateBannerNotification(null,"意外错误：没有创建卡牌实例，请重新创建或联系作者");
            throw new Exception("意外错误：没有创建卡牌实例，请重新创建或联系作者");
        }
        
      
        //关闭能力和声音编辑器
        editor.SetActive(true);
        voiceEditor.SetActive(false);
        abilityEditor.SetActive(false);
        abilityDescriptionEditor.SetActive(false);
        
        
        //信息显示到editor里

        #region 常规的信息加载

      
        cardNameField.SetTextWithoutNotify(nowEditingCard.CardName);
        friendlyNameField.SetTextWithoutNotify(nowEditingCard.FriendlyCardName);
        AnimeField.inputField.SetTextWithoutNotify(nowEditingCard.Anime);
        CharacterNameField.inputField.SetTextWithoutNotify(nowEditingCard.CharacterName);
        CVField.inputField.SetTextWithoutNotify(nowEditingCard.CV);
        cardNumberField.SetTextWithoutNotify(nowEditingCard.CardCount.ToString());
        genderField.SetValueWithoutNotify(nowEditingCard.gender);
        basicHp.SetTextWithoutNotify(nowEditingCard.BasicHealthPoint.ToString());
        basicHp.contentType = TMP_InputField.ContentType.IntegerNumber;
        basicPower.SetTextWithoutNotify(nowEditingCard.BasicPower.ToString());
        basicHp.contentType = TMP_InputField.ContentType.IntegerNumber;
        abilityReasonType.SetValueWithoutNotify((int)nowEditingCard.AbilityActivityType);
        abilityReasonObjectAsTarget.TurnOff();
        abilityDescription.text = nowEditingCard.AbilityDescription;
        AsChiefToggle.Set(nowEditingCard.allowAsChief);
        abilityResultRidicule.SetTextWithoutNotify("0");
        abilityResultSilence.SetTextWithoutNotify("0");
        CVField.inputField.SetTextWithoutNotify(String.Empty);
        abilityResultSummon.ChangeOptionDatas(CardMaker.cardMaker.nowEditingBundle.allCardsFriendlyName);
        //tag也同步一下
        tagStorage = nowEditingCard.tags;
        if (nowEditingCard.tags.Count > 0)
        {
            foreach (var tag in nowEditingCard.tags)
            {
                addTagListItem(tag);
            }
        }
        else
        {
            //移出所有无用的tag对象
            var UnusedTags = tagParent.GetComponentsInChildren<tagListItem>(false);
            for (int i = 0; i < UnusedTags.Length; i++)
            {
                UnusedTags[i].button.onClick.Invoke();
            }

        }
       
        
        
        //获取可变下拉列表内容
        RefreshVariableDropdownList(false);

        //图片，音频资源加载
        if (CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath != string.Empty)
        {
          
            var cardRootPath = Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath);
          
            //图片加载
            CardMaker.cardMaker.BanInputLayer(true, "图片资源加载中...");
            await AsyncLoadImage($"{cardRootPath}/{nowEditingCard.ImageName}");
      
            //音频加载
            CardMaker.cardMaker.BanInputLayer(true, "音频资源加载中...");
            //先清除音频
            voiceAbility.clear();
            voiceDebut.clear();
            voiceDefeat.clear();
            voiceExit.clear();
            var audioPath = string.Empty;
            //然后逐个拓展名搜索，加载音频，没加载上的就保持clear状态了
            foreach (var extension in Information.SupportedAudioExtension)
            {
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceAbilityFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceAbility, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceExitFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceExit, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceDebutFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceDebut, audioPath);
                audioPath = $"{cardRootPath}/{nowEditingCard.voiceDefeatFileName}";
                if (File.Exists(audioPath)) await AsyncLoadSelectedAudio(voiceDefeat, audioPath);

            }


         
          
        } 
      
        #endregion
        
     
        
        CardMaker.cardMaker.BanInputLayer(false, "卡牌加载中...");
        //启用编辑器，并初始化显示界面
        gameObject.SetActive(true);
        banAllInputFieldInteraction(false,0);
        //同步一下信息
        OnValueChanged();
        CardMaker.cardMaker.changeSignal.SetActive(false);

       
    }

    #region 图片选择

    /// <summary>
    /// 玩家选择图片
    /// </summary>
#pragma warning disable CS4014 // 没有等待的必要
    public void selectImageButton() => AsyncSelectImage();
#pragma warning restore CS4014 // 没有等待的必要

    async UniTask AsyncSelectImage()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", Information.SupportedImageExtension));

        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: "选择卡牌图片", loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            Debug.Log($"加载成功，图片文件{FileBrowser.Result[0]}");
            //加载图片文件
            await AsyncLoadImage(FileBrowser.Result[0]);
            //显示已修改的印记
            CardMaker.cardMaker.changeSignal.SetActive(true);
            //更新新的图片全路径
            newImageFullPath = FileBrowser.Result[0];
        }
    }


    async UniTask AsyncLoadImage(string imageFullPath)
    {
        if (!File.Exists(imageFullPath))
        {
            Debug.LogError($"图片文件“{imageFullPath}”不存在，已应用默认图片");
            imageOfCardField.sprite = defaultImage;
            preview.image.sprite = defaultImage;
            return;
        }

        newImageFullPath = imageFullPath;
        var handler = new DownloadHandlerTexture();
        UnityWebRequest unityWebRequest = new UnityWebRequest(imageFullPath, "GET", handler, null);
        await unityWebRequest.SendWebRequest();

        if (unityWebRequest.isDone)
        {
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                var sprite = Sprite.Create(handler.texture,
                    new Rect(0f, 0f, handler.texture.width, handler.texture.height), Vector2.one / 2);
                imageOfCardField.sprite = sprite;
                preview.image.sprite = sprite;
            }
            //不存在或加载失败，应用默认图片
            else
            {
                imageOfCardField.sprite = defaultImage;
                preview.image.sprite = defaultImage;
            }

            handler.Dispose();
            unityWebRequest.Dispose();
        }
    }

    #endregion

    #region 音频选择

    /// <summary>
    /// 选择音频事件调用
    /// </summary>
    /// <param name="setting"></param>
    public void SelectAudio(audioSetting setting) => AsyncSelectAudio(setting);

    /// <summary>
    /// 弹出选择框进行选择
    /// </summary>
    /// <param name="audioSetting"></param>
    private async UniTask AsyncSelectAudio(audioSetting audioSetting)
    {
        FileBrowser.SetFilters(false,new FileBrowser.Filter("卡牌音频",Information.SupportedAudioExtension));

        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: $"选择{audioSetting.title}",
            loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            await AsyncLoadSelectedAudio(audioSetting, FileBrowser.Result[0]);
        }
    }

    /// <summary>
    /// 加载音频
    /// </summary>
    private async UniTask AsyncLoadSelectedAudio(audioSetting audioSetting, string audioFullPath)
    {
        if (string.IsNullOrEmpty(audioFullPath) || audioSetting == null)
        {
            throw new Exception("音频加载参数值不能为空");         
        }
        
        if (Path.GetFileNameWithoutExtension(audioFullPath).Contains("："))
        {
            var warning = $"{audioFullPath}的文件名中，不应当含有中文引号";
            Notify.notify.CreateBannerNotification(null,warning);
            Debug.LogError(warning);
            return;
        }
            
            
        var  handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.OGGVORBIS);
   
        
        switch (Path.GetExtension(audioFullPath).ToLower())
        {
            case ".ogg":
                handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.OGGVORBIS);
                break;
                
            case ".mp3":
                handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.MPEG);
                break;
                
            case ".aif":
                handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.AIFF);
                break;
                
            case ".wav":
                handler = new DownloadHandlerAudioClip(audioFullPath, AudioType.WAV);
                break;
            
            default:
                var allSupportedFormat = Information.SupportedAudioExtension[0].Substring(1);
                for (int i = 1; i < Information.SupportedAudioExtension.Length; i++)
                {
                    allSupportedFormat = $"{allSupportedFormat}、{Information.SupportedAudioExtension[i].Substring(1)}";//ogg、wav、aif、mp3
                }
                throw new Exception($"{Path.GetExtension(audioFullPath).ToLower()}是不受支持的格式，只接受以下格式：{allSupportedFormat}");
        }

        var uwr = new UnityWebRequest(audioFullPath, "GET", handler, null);

        await uwr.SendWebRequest();

        if (uwr.isDone)
        {
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                audioSetting.AudioSelected(handler.audioClip,audioFullPath);
                
            }
            else
            {
                Debug.LogWarning($"{uwr.url}加载失败，错误原因{uwr.error}");
            }

            handler.Dispose();
            uwr.Dispose();
           
        }
    }
    
    #endregion


    //同步变化
    public void OnValueChanged()
    {
        CardMaker.cardMaker.changeSignal.SetActive(true);
        
        //预览中，除了能力外所有的信息进行同步
        preview.UpdateBasicInformation(friendlyNameField.text,CVField.text,preview.description.text,imageOfCardField.sprite);
        


    }

    public async void SaveButton()
    {

        await SaveOrSaveTo();


    }

    private async UniTask SaveOrSaveTo()
    {
         //内存保存
        var editing = CardMaker.cardMaker.nowEditingBundle.card;
        editing.CardName = cardNameField.text;
        editing.gender = genderField.value;
        editing.FriendlyCardName = friendlyNameField.text;
        editing.Anime = AnimeField.text;
        editing.tags = tagStorage;
        editing.CardCount = int.Parse(cardNumberField.text);
        editing.ImageName = newImageFullPath == string.Empty ? editing.ImageName : $"cover{Path.GetExtension(Path.GetFileName(newImageFullPath))}";
        editing.CharacterName = CharacterNameField.text;
        editing.CV = CVField.text;
        editing.allowAsChief = AsChiefToggle.On;
        editing.BasicPower = int.Parse(basicPower.text);
        editing.BasicHealthPoint = int.Parse(basicHp.text);
        editing.AbilityDescription = abilityDescription.text;
        editing.AbilityActivityType = (Information.CardAbilityTypes)abilityReasonType.value;
        editing.Reason.NeededObjects.LargeScope = (Information.Objects)abilityReasonLargeScope.value;
        editing.Reason.NeededObjects.ParameterToShrinkScope = (Information.Parameter)abilityReasonParameter.value;
        editing.Reason.NeededObjects.Logic = abilityReasonLogic.value;
        editing.Reason.NeededObjects.Threshold = abilityReasonThreshold.text;
        editing.Reason.Logic = abilityReasonLogic.value;
        editing.Reason.ReasonParameter = (Information.Parameter)abilityReasonJudgeParameter.value;
        editing.Reason.ReasonJudgeMethod = (Information.JudgeMethod)abilityReasonJudgeMethod.value;
        editing.Reason.Threshold = abilityReasonJudgeThreshold.text;
        editing.Result.RegardActivatorAsResultObject = abilityReasonObjectAsTarget.On;
        editing.Result.SummonCardName = abilityResultSummon.text;
        editing.Result.Ridicule = int.Parse(abilityResultRidicule.text);
        editing.Result.ResultObject.LargeScope = (Information.Objects)abilityResultLargeScope.value;
        editing.Result.ResultObject.ParameterToShrinkScope = (Information.Parameter)abilityResultParameter.value;
        editing.Result.ResultObject.Logic = abilityResultLogic.value;
        editing.Result.ResultObject.Threshold = abilityResultThreshold.text;
        editing.Result.ParameterToChange = (Information.Parameter)abilityResultParameterToChange.value;
        editing.Result.CalculationMethod = (Information.CalculationMethod)abilityResultChangeMethod.value;
        editing.Result.Value = abilityResultChangeValue.text;
        
        //音频
        var audios = new audioSetting[4];
        audios[0] = voiceAbility;
        audios[1] = voiceDebut;
        audios[2] = voiceDefeat; 
        audios[3] = voiceExit;
        //文件名保存在内存中
        editing.voiceExitFileName = $"{audios[3].VoiceName}{Path.GetExtension(audios[3].newAudioFullFileName)}";
        editing.voiceDefeatFileName = $"{audios[2].VoiceName}{Path.GetExtension(audios[2].newAudioFullFileName)}";
        editing.voiceDebutFileName = $"{audios[1].VoiceName}{Path.GetExtension(audios[1].newAudioFullFileName)}";
        editing.voiceAbilityFileName = $"{audios[0].VoiceName}{Path.GetExtension(audios[0].newAudioFullFileName)}";

        //不隶属于某个卡组
        if (string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
        {
            //还是 新建的卡牌
            if (string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath))
            {
                //另存为
                await CardMaker.cardMaker.AsyncSaveTo(newImageFullPath, audios);
            }
            //不是新建的，有现成的文件了
            else
            {
                //检查是用保存还是另存为
                var FileExistNow = File.Exists(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath);
                var d = Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath).Split("\\");
                var cardRootDirectory = d[^1];
                var cardNameNotChanged = editing.CardName.Equals(cardRootDirectory) && editing.CardName.Equals(Path.GetFileNameWithoutExtension(CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath));
                //额外满足这2个条件（此文件存在，并且卡牌的识别名称没有修改），执行保存操作
                if (FileExistNow && cardNameNotChanged)
                {
                    await CardMaker.cardMaker.AsyncSave(null, null, CardMaker.cardMaker.nowEditingBundle.loadedCardFullPath, newImageFullPath, false, true, audios);
                }
                //任何一点不满足，都是另存为
                else
                {
                    Notify.notify.CreateBannerNotification(null, "原始文件损坏或遗失，执行另存为");
                    await CardMaker.cardMaker.AsyncSaveTo(newImageFullPath, audios);
                }
            }


          
        }
        //隶属于某个卡组
        else
        {
            //卡组清单文件存在
            if (File.Exists(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath))
            {
                var saveFullPath = $"{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}/cards/{cardNameField.text}/{cardNameField.text}{Information.CardExtension}";
                await CardMaker.cardMaker.AsyncSave(null, null, saveFullPath, newImageFullPath, false, true, audios);
                Debug.Log($"此卡牌“{CardMaker.cardMaker.nowEditingBundle.card.FriendlyCardName}”属于卡组“{CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName}”，已自动保存到该卡组中");
            }
            //卡组清单文件不存在
            else
            {
                Debug.Log($"卡组{CardMaker.cardMaker.nowEditingBundle.manifest.FriendlyBundleName}丢失，在{Path.GetDirectoryName(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath)}中找不到");
                await CardMaker.cardMaker.AsyncSaveTo(newImageFullPath, audios);
                //认定此卡牌不属于任何卡组
                CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath = string.Empty;
                switchToBundleEditor.gameObject.SetActive(!string.IsNullOrEmpty(CardMaker.cardMaker.nowEditingBundle.loadedManifestFullPath));

            }

       
 
        }

     


    }

    
    
    //更新阈值（值）输入框下拉菜单内容
    public void OnEditEnd()
    {
        //自动生成能力描述
        if (autoGenerate.On)
        {
            
           // preview.description.text =$"当{abilityReasonLargeScope.captionText}存在{abilityReasonParameter.captionText}为{}时，"
        }
        
        
   
    }

    /// <summary>
    /// 刷新可变下拉列表内容（AnimeList CV CharacterName tags）
    /// </summary>
    /// <param name="reReadFromDisk">重新从硬盘读一遍吗？</param>
    public void RefreshVariableDropdownList(bool reReadFromDisk)
    {
       if (reReadFromDisk)
        {
            CardReadWrite.refreshYamlResFromDisk();
        }

        AnimeField.ChangeOptionDatas(Information.AnimeList);
        CVField.ChangeOptionDatas(Information.CV);
        CharacterNameField.ChangeOptionDatas(Information.characterNamesList);
        tagField.ChangeOptionDatas(Information.tags);

        //有分类标记的额外处理一下（处理分类标记和内容，加入禁用列表，合并同类项）
        WithClassificationNote.format(CharacterNameField);
        WithClassificationNote.format(tagField);
    }

/// <summary>
/// 禁用所有输入框的交互
/// </summary>
/// <param name="ban"></param>
    private void banAllInputFieldInteraction(bool ban,int level)
    {
  //IF先暂时放着吧，以后补上

        //0:触发类型选择“不”会触发
        if (level == 0)
        {
            abilityReasonLargeScope.interactable = ban;
        }
        abilityReasonParameter.interactable = ban;
        abilityReasonLogic.interactable = ban;
        abilityReasonThreshold.interactable = ban;
        abilityReasonJudgeParameter.interactable = ban;
        abilityReasonJudgeMethod.interactable = ban;
        abilityReasonJudgeLogic.interactable = ban;
        abilityReasonJudgeThreshold.interactable = ban;
        abilityReasonObjectAsTargetButton.interactable = ban;
        abilityResultLargeScope.interactable = ban;
        abilityResultParameter.interactable = ban;
        abilityResultLogic.interactable = ban;
        abilityResultThreshold.interactable = ban;
        abilityResultParameterToChange.interactable = ban;
        abilityResultChangeMethod.interactable = ban;
        abilityResultChangeValue.interactable = ban;
        abilityResultSummon.interactable = ban;
        abilityResultRidicule.interactable = ban;
        abilityResultSilence.interactable = ban;
    }
    
    /// <summary>
    /// 为带有helper的输入框提供可变的下拉栏帮助（用于规范输入内容）
    /// </summary>
    /// <param name="inputFieldWithDropdown"></param>
    /// <param name="index"></param>
    private void inputFieldHelperContent(InputFieldWithDropdown inputFieldWithDropdown,int index)
    {
  switch (index)
        {
            //Anime
            case (int)Information.Parameter.Anime:
                inputFieldWithDropdown.ChangeOptionDatas(AnimeField.options, true);
                inputFieldWithDropdown.ban = AnimeField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //tag
            case (int) Information.Parameter.Tag:
                inputFieldWithDropdown.ChangeOptionDatas(tagField.options, true);
                inputFieldWithDropdown.ban = tagField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //state
            case (int) Information.Parameter.State :
                inputFieldWithDropdown.ClearOptions();
                var length = Enum.GetNames(typeof(Information.CardState)).Length;
                for (int i = 0; i < length; i++)
                {
                    inputFieldWithDropdown.options.Add(
                        new TMP_Dropdown.OptionData(
                            Information.AbilityChineseIntroduction((Information.CardState)i)));
                }

                inputFieldWithDropdown.supportFilter = true;
                break;

            //cv
            case (int) Information.Parameter.CV:
                inputFieldWithDropdown.ChangeOptionDatas(CVField.options);
                inputFieldWithDropdown.ban = CVField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //characterName
            case (int) Information.Parameter.CharacterName:
                inputFieldWithDropdown.ChangeOptionDatas(CharacterNameField.options);
                inputFieldWithDropdown.ban = CharacterNameField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;
            
            case (int) Information.Parameter.Gender:
                inputFieldWithDropdown.ChangeOptionDatas(genderField.options);
                inputFieldWithDropdown.ban = null;
                inputFieldWithDropdown.supportFilter = true;
                break;

            default:
                inputFieldWithDropdown.Ban();
                inputFieldWithDropdown.inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                inputFieldWithDropdown.text = String.Empty;
                break;
        }
    }
    
    
    #region 有分类标记的处理方法

    private class WithClassificationNote
    {
        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="inputFieldWithDropdown"></param>
        public static void format(InputFieldWithDropdown inputFieldWithDropdown)
        {
            for (int i = 0; i < inputFieldWithDropdown.options.Count; i++)
            {
                //是分类标记的，粗体，禁用选择
                if (inputFieldWithDropdown.options[i].text.Substring(0, 1) == "%")
                {
                    var newText = $"<b>{inputFieldWithDropdown.options[i].text[1..]}</b>";
                    inputFieldWithDropdown.EditOptionText(i, newText);
                    //记录分类标记的序号，禁止玩家输入
                    inputFieldWithDropdown.ban.Add(newText);
                }
                //不是的，稍许淡化，缩进一点
                else
                {
                    var newText = $"<margin=5%><alpha=#CC>{inputFieldWithDropdown.options[i].text}";
                    inputFieldWithDropdown.EditOptionText(i, newText);
                }
            }
        }
    }

    #endregion

    #region 标签编辑那边的方法

    public void TagAddition()
    {
        if (tagStorage.Contains(tagField.text))
        {
            Notify.notify.CreateBannerNotification(null, $"标签“{tagField.text}”已存在");
            return;
        }

        if (tagField.text.Contains(" "))
        {
            Notify.notify.CreateBannerNotification(null, $"标签“{tagField.text}”不能包含空格");
            return;
        }

        if (tagField.text == "")
        {
            Notify.notify.CreateBannerNotification(null, $"标签不能为空");
            return;
        }

        //储存增加一个
        tagStorage.Add(tagField.text);
        //显示出来
        addTagListItem(tagField.text);
        //清空输入框内容
        tagField.text = "";
        //修改标记显示
        CardMaker.cardMaker.changeSignal.SetActive(true);
    }


    /// <summary>
    /// 在列表内添加一个
    /// </summary>
    /// <param name="text"></param>
    private void addTagListItem(string text)
    {
        var item = Instantiate(tagListButtonPerfebs.gameObject, tagParent).GetComponent<tagListItem>();
        //显示出来tag文本
        item.Initialization(text);
        //注册删除tag功能
        item.onRemove.AddListener(delegate(string arg0) { tagStorage.Remove(arg0); });
        
    }

    #endregion
}
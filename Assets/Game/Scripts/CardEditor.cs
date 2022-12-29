using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Image = UnityEngine.UI.Image;

public class CardEditor : MonoBehaviour
{
    public GameObject editor;
    public GameObject abilityEditor;
    
    
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
    public audioSetting voiceKill;
    public audioSetting voiceAbility;
    public audioSetting voiceExit;
    
    
    [Space()] public CardPanel preview;

    /// <summary>
    /// 是否为仅仅创建卡牌而已
    /// </summary>
    private bool onlyCreateCard { get; set; }

    /// <summary>
    /// 现在显示的 正在编辑的卡牌
    /// </summary>
    private CharacterCard nowEditingCard { get; set; }

    private string newImageFullPath { get; set; }

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
      voiceKill.OnPrepareToSelectAudio.AddListener(SelectAudio);
    
      
      
        #endregion
        
       
        
      
       
    }

    public void OpenCardEditorForCreation(bool onlyCreateCard)
    {
        //启用编辑器，并初始化显示界面
        gameObject.SetActive(true);
        editor.SetActive(true);
        abilityEditor.SetActive(false);

        //把新创新的卡的信息填充进去显示
        if (onlyCreateCard) CardMaker.cardMaker.nowEditingBundle = new();
        //cards数组扩充一位
        var cardsCache = CardMaker.cardMaker.nowEditingBundle.cards;
        CardMaker.cardMaker.nowEditingBundle.cards = new CharacterCard[cardsCache.Length + 1];
        for (int i = 0; i < CardMaker.cardMaker.nowEditingBundle.cards.Length; i++)
        {
            if (i == CardMaker.cardMaker.nowEditingBundle.cards.Length - 1)
            {
                //新创建的，正在编辑的
                index = i;
                CardMaker.cardMaker.nowEditingBundle.cards[i] = new CharacterCard();
            }
            else
            {
               
                CardMaker.cardMaker.nowEditingBundle.cards[i] = cardsCache[i];
            }
        }

        cardsCache = null;

        //信息显示到editor里

        #region 常规的信息编辑

        nowEditingCard =
            CardMaker.cardMaker.nowEditingBundle.cards[CardMaker.cardMaker.nowEditingBundle.cards.Length - 1];
        cardNameField.SetTextWithoutNotify(nowEditingCard.CardName);
        friendlyNameField.SetTextWithoutNotify(nowEditingCard.FriendlyCardName);
        AnimeField.inputField.SetTextWithoutNotify(onlyCreateCard ? nowEditingCard.Anime : CardMaker.cardMaker.nowEditingBundle.manifest.Anime);
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
        //获取可变下拉列表内容
        RefreshVariableDropdownList(false);

        #endregion
        
        banAllInputFieldInteraction(false,0);
        
        OnValueChanged();
        
        CVField.inputField.SetTextWithoutNotify(String.Empty);
        CardMaker.cardMaker.changeSignal.SetActive(false);
        
        
        //召唤生成那边，是要获取该卡组内的所有卡牌
        var card = new string[CardMaker.cardMaker.nowEditingBundle.cards.Length];
        for (int i = 0; i <CardMaker.cardMaker.nowEditingBundle.cards.Length; i++)
        {
            card[i] = CardMaker.cardMaker.nowEditingBundle.cards[i].FriendlyCardName;
        }
        abilityResultSummon.ChangeOptionDatas(card);
        abilityResultSummon.interactable = false;
    }

    #region 图片选择

    /// <summary>
    /// 玩家选择图片
    /// </summary>
#pragma warning disable CS4014 // 没有等待的必要
    public void selectImage() => AsyncSelectImage();
#pragma warning restore CS4014 // 没有等待的必要

    async UniTask AsyncSelectImage()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("图片", ".jpg", ".bmp", ".png", ".gif"));

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
            else
            {
                Debug.LogWarning($"{unityWebRequest.url}加载失败，错误原因{unityWebRequest.error}");
            }
        }
    }

    #endregion

    #region 音频选择

    public void SelectAudio(audioSetting setting) => AsyncSelectAudio(setting);

    private async UniTask AsyncSelectAudio(audioSetting audioSetting)
    {
        FileBrowser.SetFilters(false,new FileBrowser.Filter("卡牌音频",".mp3",".ogg",".wav",".aif"));

        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, title: $"选择{audioSetting.title}",
            loadButtonText: "选择");

        if (FileBrowser.Success)
        {
            if (Path.GetFileNameWithoutExtension(FileBrowser.Result[0]).Contains("："))
            {
                var warning = $"{FileBrowser.Result[0]}不应当含有中文引号";
                Notify.notify.CreateBannerNotification(null,warning);
                Debug.LogError(warning);
                return;
            }
            
            
           var  handler = new DownloadHandlerAudioClip(FileBrowser.Result[0], AudioType.OGGVORBIS);
            switch (Path.GetExtension(FileBrowser.Result[0]).ToLower())
            {
                case ".ogg":
                    handler = new DownloadHandlerAudioClip(FileBrowser.Result[0], AudioType.OGGVORBIS);
                    break;
                
                case ".mp3":
                    handler = new DownloadHandlerAudioClip(FileBrowser.Result[0], AudioType.MPEG);
                    break;
                
                case ".aif":
                    handler = new DownloadHandlerAudioClip(FileBrowser.Result[0], AudioType.AIFF);
                    break;
                
                case ".wav":
                    handler = new DownloadHandlerAudioClip(FileBrowser.Result[0], AudioType.WAV);
                    break;
            }

            var uwr = new UnityWebRequest(FileBrowser.Result[0], "GET", handler, null);

            await uwr.SendWebRequest();

            if (uwr.isDone)
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    audioSetting.AudioSelected(handler.audioClip,uwr.url);
                }
                else
                {
                    Debug.LogWarning($"{uwr.url}加载失败，错误原因{uwr.error}");
                }
            }
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

    public async void Save()
    {
        //内存保存
        var editing = CardMaker.cardMaker.nowEditingBundle.cards[index];
        editing.CardName = cardNameField.text;
        editing.gender = genderField.value;
        editing.FriendlyCardName = friendlyNameField.text;
        editing.Anime = AnimeField.text;
        editing.tags = tagStorage;
        editing.CardCount = int.Parse(cardNumberField.text);
        editing.imageName = newImageFullPath == String.Empty ? editing.imageName : Path.GetFileName(newImageFullPath);
        editing.CharacterName = CharacterNameField.text;
        editing.CV = CVField.text;
        editing.allowAsChief = AsChiefToggle.On;
        editing.BasicPower = int.Parse(basicPower.text);
        editing.BasicHealthPoint = int.Parse(basicHp.text);
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
        editing.voiceAbility = Path.GetFileName(voiceAbility.audioFullFileName);
        editing.voiceDebut = Path.GetFileName(voiceDebut.audioFullFileName);
        editing.voiceExit = Path.GetFileName(voiceExit.audioFullFileName);
        editing.voiceKill = Path.GetFileName(voiceKill.audioFullFileName);
        
        
        

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
            case 1:
                inputFieldWithDropdown.ChangeOptionDatas(AnimeField.options, true);
                inputFieldWithDropdown.ban = AnimeField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //tag
            case 2:
                inputFieldWithDropdown.ChangeOptionDatas(tagField.options, true);
                inputFieldWithDropdown.ban = tagField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //state
            case 7:
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
            case 8:
                inputFieldWithDropdown.ChangeOptionDatas(CVField.options);
                inputFieldWithDropdown.ban = CVField.ban;
                inputFieldWithDropdown.supportFilter = true;
                break;

            //characterName
            case 9:
                inputFieldWithDropdown.ChangeOptionDatas(CharacterNameField.options);
                inputFieldWithDropdown.ban = CharacterNameField.ban;
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
        item.Initialization(text);
    }

    #endregion
}
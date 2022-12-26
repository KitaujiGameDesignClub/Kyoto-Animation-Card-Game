using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using KitaujiGameDesignClub.GameFramework.UI;
using Lean.Gui;
using NUnit.Framework;
using SimpleFileBrowser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
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
    public TMP_InputField abilityReasonThreshold;
    public TMP_Dropdown abilityReasonJudgeParameter;
    public TMP_Dropdown abilityReasonJudgeMethod;
    public TMP_Dropdown abilityReasonJudgeLogic;
    public TMP_InputField abilityReasonJudgeThreshold;
    [Header("效果侧")] public Lean.Gui.LeanToggle abilityReasonObjectAsTarget;
    public TMP_Dropdown abilityResultLargeScope;
    public TMP_Dropdown abilityResultParameter;
    public TMP_Dropdown abilityResultLogic;
    public TMP_InputField abilityResultThreshold;
    public TMP_Dropdown abilityResultParameterToChange;
    public InputFieldWithDropdown abilityResultSummon;
    public TMP_InputField abilityResultRidicule;
    public TMP_Dropdown abilityResultChangeMethod;
    public TMP_InputField abilityResultChangeValue;
    [Header("描述侧")] public Lean.Gui.LeanButton Auto;
    public Lean.Gui.LeanButton Clear;
    public TMP_InputField abilityDescription;

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
        cardNameField.text = nowEditingCard.CardName;
        friendlyNameField.text = nowEditingCard.FriendlyCardName;
        AnimeField.text = onlyCreateCard ? nowEditingCard.Anime : CardMaker.cardMaker.nowEditingBundle.manifest.Anime;
        CharacterNameField.text = nowEditingCard.CharacterName;
        CVField.text = nowEditingCard.CV;
        cardNumberField.text = nowEditingCard.CardCount.ToString();
        genderField.value = nowEditingCard.gender;
        basicHp.text = nowEditingCard.BasicHealthPoint.ToString();
        basicPower.text = nowEditingCard.BasicPower.ToString();
        abilityReasonType.value = (int)nowEditingCard.AbilityActivityType;
        abilityReasonObjectAsTarget.TurnOff();
        abilityDescription.text = nowEditingCard.AbilityDescription;
        AsChiefToggle.Set(nowEditingCard.allowAsChief);
        //获取可变下拉列表内容
        RefreshVariableDropdownList(false);

        #endregion

        #region 能力编辑初始化

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
                Debug.LogWarning($"{unityWebRequest.url}加载失败");
            }
        }
    }

    public void OnValueChanged()
    {
        CardMaker.cardMaker.changeSignal.SetActive(true);
    }

    public void OnEditEnd()
    {
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
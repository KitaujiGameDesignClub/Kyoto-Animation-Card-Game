using System.Collections;
using System.Collections.Generic;
using Core;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine;


public class CardEditor : MonoBehaviour
{
    public GameObject editor;
    public GameObject abilityEditor;

    [Header("信息侧")]
    public TMP_InputField cardNameField;
    public TMP_InputField friendlyNameField;
    public InputFieldWithDropdown AnimeField;
    public InputFieldWithDropdown CharacterNameField;
    public InputFieldWithDropdown CVField;
    public TMP_InputField cardNumberField;
    public TMP_Dropdown genderField;
    public TMP_InputField basicPower;
    public TMP_InputField basicHp;
    public InputFieldWithDropdown tagField;
    [Header("原因侧")]
    public TMP_Dropdown abilityReasonType;
    public TMP_Dropdown abilityReasonLargeScope;
    public TMP_Dropdown abilityReasonParameter;
    public TMP_Dropdown abilityReasonLogic;
    public TMP_InputField abilityReasonThreshold;
    [Header("效果侧")]
    public Lean.Gui.LeanToggle abilityReasonObjectAsTarget;
    public TMP_Dropdown abilityResultLargeScope;
    public TMP_Dropdown abilityResultParameter;
    public TMP_Dropdown abilityResultLogic;
    public TMP_InputField abilityResultThreshold;
    public TMP_Dropdown abilityResultParameterToChange;
    public InputFieldWithDropdown abilityResultSummon;
    public TMP_InputField abilityResultRidicule;
    public TMP_Dropdown abilityResultChangeMethod;
    public TMP_Dropdown abilityResultChangeValue;
    [Header("描述侧")]
    public Lean.Gui.LeanButton Auto;
    public Lean.Gui.LeanButton Clear;
    public TMP_InputField abilityDescription;

    [Space()]
    public CardPanel preview;

    /// <summary>
    /// 是否为仅仅创建卡牌而已
    /// </summary>
    private bool onlyCreateCard;

    /// <summary>
    /// 现在显示的 正在编辑的卡牌
    /// </summary>
    private CharacterCard nowEditingCard;


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
            if(i == CardMaker.cardMaker.nowEditingBundle.cards.Length - 1)
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
        nowEditingCard = CardMaker.cardMaker.nowEditingBundle.cards[CardMaker.cardMaker.nowEditingBundle.cards.Length - 1];
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
        //获取下拉列表内容
        //可变下拉列表
        RefreshVariableDropdownList(false);

    }


   public void OnValueChanged()
    {

    }

    public void OnEditEnd()
    {

    }

    /// <summary>
    /// 刷新可变下拉列表内容（重新读取Information内的AnimeList CV CharacterName tags）
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
    {  /// <summary>
        /// 储存分类标记，和相同分类标记的Index
        /// </summary>
        struct NoteIndexStorage
        {
            public  List<string> Classification;
            public List< int> index;

            public NoteIndexStorage(string s)
            {
                Classification = new();
                index = new();
            }

         }

        /// <summary>
        /// 合并同类项
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void Merge(string[] optionDatas)
        { 
            //找到所有的分类标记和他的index
           var storages = new NoteIndexStorage();
            for (int i = 0; i < optionDatas.Length; i++)
            {
                if (optionDatas[i].Substring(0, 1) == "%")
                {
                    storages.Classification.Add(optionDatas[i]);
                    storages.index.Add(i);
                }
            }

            //用于return的结果
            var merged = new string[optionDatas.Length];

            


        }

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


}

  

    
    #endregion

    #region 标签编辑那边的方法

    #endregion




using System.Collections;
using System.Collections.Generic;
using Core;
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
    public TMP_InputField abilityResultSummon;
    public TMP_InputField abilityResultRidicule;
    public TMP_Dropdown abilityResultChangeMethod;
    public TMP_Dropdown abilityResultChangeValue;
    [Header("描述侧")]
    public Lean.Gui.LeanToggle Auto;
    public Lean.Gui.LeanToggle Clear;
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
        abilityEditor.SetActive(true);

        //把新创新的卡的信息填充进去显示
        if (onlyCreateCard) CardMaker.cardMaker.nowEditingBundle = new();
         CardMaker.cardMaker.nowEditingBundle.cards.Add(new CharacterCard());
        nowEditingCard = CardMaker.cardMaker.nowEditingBundle.cards[CardMaker.cardMaker.nowEditingBundle.cards.Count - 1];
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

    }


   
}

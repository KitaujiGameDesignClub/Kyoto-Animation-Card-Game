using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core;
using System;
using Core.Interface;
using Cysharp.Threading.Tasks;

/// <summary>
/// 图形化显示卡牌的信息（characterInGame的表状态）
/// </summary>
public class CardPanel : MonoBehaviour, ICharacterCardInGame
{
    [Header("信息模式")]
    public TMP_Text cardName;
    public TMP_Text cv;
    public TMP_Text description;
    [Header("通用")]
    public SpriteRenderer image;
    /// <summary>
    /// 所用卡牌在游戏中的状态
    /// </summary>
    [Header("游戏模式")]
    public CharacterInGame cardStateInGame;
    public Transform tr;
    public GameObject[] othersToDestroy;

 
    public void UpdateBasicInformation(string cardName,string cv, string description,Sprite image)
    {
        this.cardName.text = cardName ?? throw new ArgumentNullException(nameof(cardName));
        this.cv.text = cv == "不设置声优" ? string.Empty : $"cv:{cv}";
        this.description.text = description ?? throw new ArgumentNullException(nameof(description));
        this.image.sprite = image;
    }

    /// <summary>
    /// 进入到游戏模式，从里文件CharacterInGame中读取，并展示出来
    /// </summary>
    /// <param name="cardState"></param>
    public void EnterGameMode(CharacterInGame cardState)
    {
        image.sprite = cardState.CoverImage;
        DestroyImmediate(cardName.gameObject);
        DestroyImmediate(cv.gameObject);
        DestroyImmediate(description.gameObject);

        foreach (var item in othersToDestroy)
        {
            DestroyImmediate(item.gameObject);
        }

       // cardName.gameObject.SetActive(false);
        //cv.gameObject.SetActive(false);
       // description.gameObject.SetActive(false);
        cardStateInGame = cardState;

        //修复莫名其妙的图片变大的（显示不完全）的bug
        image.size = Vector2.one * 0.6228073f;
    }

  
    public void GetDamaged(int damage, CharacterInGame activator)
    {
        ((ICharacterCardInGame)cardStateInGame).GetDamaged(damage, activator);
    }

    public void PowerUp(int value, CharacterInGame activator)
    {
        ((ICharacterCardInGame)cardStateInGame).PowerUp(value, activator);
    }

    public void ChangeHealthAndPower(bool changeHealth, int value1, bool changePower, int value2, CharacterInGame Activator)
    {
        ((ICharacterCardInGame)cardStateInGame).ChangeHealthAndPower(changeHealth, value1, changePower, value2, Activator);
    }

    public void ChangeState(Information.CardState cardState)
    {
        ((ICharacterCardInGame)cardStateInGame).ChangeState(cardState);
    }

    public void OnDebut()
    {
        ((ICharacterCardInGame)cardStateInGame).OnDebut();
    }

    public void Normal()
    {
        ((ICharacterCardInGame)cardStateInGame).Normal();
    }

    public void Exit()
    {
        ((ICharacterCardInGame)cardStateInGame).Exit();
    }

    public void OnHurt(CharacterInGame activator)
    {
        ((ICharacterCardInGame)cardStateInGame).OnHurt(activator);
    }
}

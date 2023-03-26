using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// 读取到的卡牌的缓存，全局都是只读的
/// </summary>
public class CardCache 
{
    /// <summary>
    /// 此角色卡的配置文件（用于游戏中对某些参数的修改）
    /// </summary>
    public CharacterCard Profile { get; set; }

    //识别
    public readonly string UUID;

    //可变的信息缓存
   public readonly string CharacterName;
    public readonly int Gender;
    public readonly string Anime;
    public readonly List<string> Tag;
    public readonly int hp;
    public readonly int power;


    /// <summary>
    /// 音效资源
    /// </summary>
    public readonly AudioClip voiceDebut;
    public readonly AudioClip voiceDefeat;
public readonly AudioClip voiceExit;
    public readonly AudioClip voiceAbility;
    //图片资源
    public readonly Sprite CoverImage;

    public CardCache(CharacterCard card)
    {
        Profile = card;
        //信息缓存
        this.UUID = card.UUID;
        CharacterName = card.CharacterName;
        Gender = card.gender;
        Anime = card.Anime;
        Tag = card.tags;
        hp = card.BasicHealthPoint;
        power = card.BasicPower;
        this.voiceDebut = null;
        this.voiceDefeat = null;
        this.voiceExit = null;
        this.voiceAbility = null;
        CoverImage = null;
    }

    public CardCache(CharacterCard card,ref AudioClip voiceDebut,ref AudioClip voiceDefeat,ref AudioClip voiceExit,ref AudioClip voiceAbility,ref Sprite coverImage)
    {
        Profile = card;
        //信息缓存
        this.UUID =  card.UUID;
        CharacterName = card.CharacterName;
        Gender = card.gender;
        Anime = card.Anime;
        Tag = card.tags;
        hp = card.BasicHealthPoint;
        power = card.BasicPower;
        this.voiceDebut = voiceDebut;
        this.voiceDefeat = voiceDefeat;
        this.voiceExit = voiceExit;
        this.voiceAbility = voiceAbility;
        CoverImage = coverImage;
    }
}

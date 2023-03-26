using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// ��ȡ���Ŀ��ƵĻ��棬ȫ�ֶ���ֻ����
/// </summary>
public class CardCache 
{
    /// <summary>
    /// �˽�ɫ���������ļ���������Ϸ�ж�ĳЩ�������޸ģ�
    /// </summary>
    public CharacterCard Profile { get; set; }

    //ʶ��
    public readonly string UUID;

    //�ɱ����Ϣ����
   public readonly string CharacterName;
    public readonly int Gender;
    public readonly string Anime;
    public readonly List<string> Tag;
    public readonly int hp;
    public readonly int power;


    /// <summary>
    /// ��Ч��Դ
    /// </summary>
    public readonly AudioClip voiceDebut;
    public readonly AudioClip voiceDefeat;
public readonly AudioClip voiceExit;
    public readonly AudioClip voiceAbility;
    //ͼƬ��Դ
    public readonly Sprite CoverImage;

    public CardCache(CharacterCard card)
    {
        Profile = card;
        //��Ϣ����
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
        //��Ϣ����
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

using Core;
using UnityEngine;

/// <summary>
/// 读取到的卡牌的缓存，全局都是只读的
/// </summary>
public class CardCache 
{
    public readonly CharacterCard card;
    /// <summary>
    /// 音效资源
    /// </summary>
    public readonly AudioClip voiceDebut;
    public readonly AudioClip voiceDefeat;
    public readonly AudioClip voiceExit;
    public readonly AudioClip voiceAbility;
    //图片资源
    public readonly Sprite CoverImage;

    public CardCache(CharacterCard card, AudioClip voiceDebut, AudioClip voiceDefeat, AudioClip voiceExit, AudioClip voiceAbility, Sprite coverImage)
    {
        this.card = card;
        this.voiceDebut = voiceDebut;
        this.voiceDefeat = voiceDefeat;
        this.voiceExit = voiceExit;
        this.voiceAbility = voiceAbility;
        CoverImage = coverImage;
    }
}

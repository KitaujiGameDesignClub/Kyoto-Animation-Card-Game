using Core;
using UnityEngine;

/// <summary>
/// ��ȡ���Ŀ��ƵĻ��棬ȫ�ֶ���ֻ����
/// </summary>
public class CardCache 
{
    public readonly CharacterCard card;
    /// <summary>
    /// ��Ч��Դ
    /// </summary>
    public readonly AudioClip voiceDebut;
    public readonly AudioClip voiceDefeat;
    public readonly AudioClip voiceExit;
    public readonly AudioClip voiceAbility;
    //ͼƬ��Դ
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

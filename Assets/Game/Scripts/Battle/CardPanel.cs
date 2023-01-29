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
    public GameObject[] othersToDestroy;
    public CharacterInGame cardStateInGame;
    public Transform tr;
    public TMP_Text powerValue;
    public TMP_Text hpValue;

    /// <summary>
    /// 信息展示用（即不是游戏模式）
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="cv"></param>
    /// <param name="description"></param>
    /// <param name="image"></param>
    public void UpdateBasicInformation(string cardName,string cv, string description,Sprite image)
    {
        this.cardName.text = cardName ?? throw new ArgumentNullException(nameof(cardName));
        this.cv.text = cv == "不设置声优" ? string.Empty : $"cv:{cv}";
        this.description.text = description ?? throw new ArgumentNullException(nameof(description));
        this.image.sprite = image;
        Destroy(powerValue.gameObject);
        Destroy(hpValue.gameObject);
    }

    /// <summary>
    /// 进入到游戏模式，从里文件CharacterInGame中读取，并展示出来
    /// </summary>
    /// <param name="cardState"></param>
    public void EnterGameMode(CharacterInGame cardState)
    {
        //设置上图片
        image.sprite = cardState.CoverImage == null ? image.sprite: cardState.CoverImage;
        image.sortingOrder = 0;//层级调整
        //初始化体力值与行动力
        powerValue.text = cardState.actualPower.ToString();
        powerValue.gameObject.SetActive(true);
        hpValue.text = cardState.actualHealthPoint.ToString();
        hpValue.gameObject.SetActive(true);
        DestroyImmediate(cardName.gameObject);
        DestroyImmediate(cv.gameObject);
        DestroyImmediate(description.gameObject);

        foreach (var item in othersToDestroy)
        {
          if(item != null) DestroyImmediate(item);
        }

       // cardName.gameObject.SetActive(false);
        //cv.gameObject.SetActive(false);
       // description.gameObject.SetActive(false);
        cardStateInGame = cardState;

        //修复莫名其妙的图片变大的（显示不完全）的bug
        image.size = Vector2.one * 0.6228073f;

        //将图片放到panel中间
        image.transform.localPosition = Vector3.zero;
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
        //更新数据显示
        powerValue.text = cardStateInGame.actualPower.ToString();
        hpValue.text = cardStateInGame.actualHealthPoint.ToString();
    }

    public void ChangeState(Information.CardState cardState)
    {
        ((ICharacterCardInGame)cardStateInGame).ChangeState(cardState);
    }

    public void OnDebut()
    {
        ((ICharacterCardInGame)cardStateInGame).OnDebut();
    }

    public void Attack(CharacterInGame target)
    {
        //执行攻击逻辑
        ((ICharacterCardInGame)cardStateInGame).Attack(target);
    }

    /// <summary>
    /// 针对：每一回合都执行的攻击逻辑 用的动画之类的东西
    /// </summary>
    /// <returns></returns>
    public async UniTask AnimationForNormal(Vector2 attackPoint)
    {
        //通常的攻击动画
        await attackAnimation(attackPoint);
    }

    private async UniTask attackAnimation(Vector2 attackPoint)
    {
        //记录原位置
        var originalPos = tr.position;

        //靠近要攻击目标卡牌
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, attackPoint, 0.1f);
            //足够近，停止循环
            if(Math.Abs(tr.position.x - attackPoint.x) <= 0.1f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        //等100ms
        await UniTask.Delay(100);
        //回到远地点
        while (true)
        {
            tr.position = Vector2.Lerp(tr.position, originalPos, 0.1f);
            //足够近，停止循环（尽量能靠近）
            if (Math.Abs(tr.position.x - originalPos.x) <= 0.01f)
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
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

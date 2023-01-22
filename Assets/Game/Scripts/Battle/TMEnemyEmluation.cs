using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;

public class TMEnemyEmluation : MonoBehaviour
{
  /// <summary>
  /// 模拟的敌机数据，不用很复杂，甚至可以不用写能力
  /// </summary>
  public CharacterCard enemyProfile;

  /// <summary>
  /// 卡牌信息展示
  /// </summary>
  public TMP_Text Information;
    public TMP_Text Name;

    //数值修改
    public IntegerInput power;
    public IntegerInput hp;
    public IntegerInput cardCount;

  private void Awake()
  {
        //更新信息
        Name.text = enemyProfile.FriendlyCardName;
        var allTags = string.Empty;
        for (int i = 0; i < enemyProfile.tags.Count; i++)
        {
            if(i == 0) allTags = $"{enemyProfile.tags[i]}";
            else allTags = $"{allTags}、{enemyProfile.tags[i]}";

        }
    Information.text = $"所属动画：{enemyProfile.Anime}\n" +
                       $"声优：{enemyProfile.CV}\n" +
                       $"能力描述：{enemyProfile.AbilityDescription}\n" +
                       $"标签：{allTags}";
        power.onEndEdit.AddListener(delegate(int value) { enemyProfile.BasicPower = value; });
        hp.onEndEdit.AddListener(delegate (int value) { enemyProfile.BasicHealthPoint = value; });
        cardCount.onEndEdit.AddListener(delegate (int value) { enemyProfile.CardCount = value; });
    }
}

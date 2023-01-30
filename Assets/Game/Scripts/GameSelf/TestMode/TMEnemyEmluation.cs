using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using KitaujiGameDesignClub.GameFramework.UI;
using TMPro;
using UnityEngine.UI;

public class TMEnemyEmluation : MonoBehaviour
{
  /// <summary>
  /// 模拟的敌机数据，不用很复杂，甚至可以不用写能力
  /// </summary>
  public CharacterCard enemyProfile;

    /// <summary>
    /// 卡牌信息展示
    /// </summary>
    [SerializeField] TMP_Text Information;
    [SerializeField] TMP_Text Name;
    public Image image;

    //数值修改
    [SerializeField] IntegerInput power;
    [SerializeField] IntegerInput hp;
    [SerializeField] IntegerInput cardCount;

  private void Start()
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

        //初始值与事件
        enemyProfile.BasicPower = power.value;
        power.onEndEdit.AddListener(delegate(int value) { enemyProfile.BasicPower = value; });
        enemyProfile.BasicHealthPoint = hp.value;
        hp.onEndEdit.AddListener(delegate (int value) { enemyProfile.BasicHealthPoint = value; });
        enemyProfile.CardCount = cardCount.value;
        cardCount.onEndEdit.AddListener(delegate (int value) { enemyProfile.CardCount = value;});
       
    }
}

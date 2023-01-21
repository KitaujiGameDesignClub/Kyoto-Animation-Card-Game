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

  private void Awake()
  {
    //更新信息
    Information.text = $"所属动画：{enemyProfile.Anime}\n" +
                       $"声优：{enemyProfile.CV}\n" +
                       $"能力描述：{enemyProfile.AbilityDescription}\n" +
                       $"标签";
  }
}

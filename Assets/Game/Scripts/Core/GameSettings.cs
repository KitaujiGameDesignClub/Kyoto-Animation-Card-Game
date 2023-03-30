using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameSettings
{

}

/// <summary>
/// 打架用的延迟管理器
/// </summary>
public class BattleDelayManagement
{
    /// <summary>
    /// 能力使用后，的等待时间
    /// </summary>
    public const int AbilityPerformedWaitTime = 300;

    /// <summary>
    /// 能力执行之前的等待时间
    /// </summary>
    public const int AbilityToPerformWaitTime = 200;

    /// <summary>
    /// 卡牌去打架后，在敌机那边的停留时间
    /// </summary>
    public const int CardStayTime = 250;

    /// <summary>
    /// 每个卡牌执行结束之后的等待时间
    /// </summary>
    public const int RoundEndWaitTime = 350;
}

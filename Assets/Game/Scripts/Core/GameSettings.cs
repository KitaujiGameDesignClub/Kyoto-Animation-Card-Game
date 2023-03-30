using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameSettings
{

}

/// <summary>
/// ����õ��ӳٹ�����
/// </summary>
public class BattleDelayManagement
{
    /// <summary>
    /// ����ʹ�ú󣬵ĵȴ�ʱ��
    /// </summary>
    public const int AbilityPerformedWaitTime = 300;

    /// <summary>
    /// ����ִ��֮ǰ�ĵȴ�ʱ��
    /// </summary>
    public const int AbilityToPerformWaitTime = 200;

    /// <summary>
    /// ����ȥ��ܺ��ڵл��Ǳߵ�ͣ��ʱ��
    /// </summary>
    public const int CardStayTime = 250;

    /// <summary>
    /// ÿ������ִ�н���֮��ĵȴ�ʱ��
    /// </summary>
    public const int RoundEndWaitTime = 350;
}

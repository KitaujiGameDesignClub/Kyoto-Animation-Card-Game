using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 在加载好卡牌的情况下，游戏的本体逻辑应当都在这边
/// </summary>
public class GameStageCtrl : MonoBehaviour
{
    public static GameStageCtrl stageCtrl;
    /// <summary>
    /// cardPanel的预设（含默认图片）
    /// </summary>
    public CardPanel cardPrefeb;

    /// <summary>
    /// 卡牌预用点位 A=0 B=1
    /// </summary>
    [SerializeField] private Team[] CardPrePoint  = new Team[2];
    [SerializeField] private Transform CardCache;

    /// <summary>
    /// 每个卡牌之间 攻击的间隔(ms)
    /// </summary>
    [Header("打架逻辑")]
    [Tooltip("单位：ms")]
    public int basicCardInterval = 300;
    /// <summary>
    /// 强制停止游戏逻辑
    /// </summary>
    bool forceToStop = false;

    public TestMode testMode;

    private void Awake()
    {
        stageCtrl = this;
       
    }

    #region 打架逻辑
    /// <summary>
    /// 初始化社团之间的战场
    /// </summary>
    public void InitializeGame()
    {
        //初始化一个新游戏
        GameState.CreateNewGame();
    }

    /// <summary>
    /// 打架系统
    /// </summary>
    /// <param name="pauseModeOfBattle"></param>
    /// <returns></returns>
   public  async UniTask BattleSystem(Information.PauseModeOfBattle pauseModeOfBattle,Action battleEnd)
    {
        //就要打架了，让场上所有的卡牌执行“登场”函数
        if(GameState.gameState != Information.GameState.Competition)
        {
            var allCards = GetAllCardOnStage(0);
            foreach (var item in allCards)
            {
                await item.OnDebut();
            }
            allCards = GetAllCardOnStage(1);
            foreach (var item in allCards)
            {
                await item.OnDebut();
            }
        }

        await UniTask.Delay(500);

        ShowAbilityNews($"{nameof(BattleSystem)}",null,"卡牌登场指令执行完成");

        //调整游戏状态为“在打架”
        GameState.gameState = Information.GameState.Competition;


        switch (pauseModeOfBattle)
        {
            //每张卡牌运行后，都要暂停
            case Information.PauseModeOfBattle.EachCard:

                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1:0);
                
                break;

            case Information.PauseModeOfBattle.EachEnemyCard:
                //玩家这边优先开打（这一轮做什么）     
                await CardRound(0);
                //卡牌间隔
                await UniTask.Delay(basicCardInterval);
                //敌人打
                await CardRound(1);
                break;

            case Information.PauseModeOfBattle.EachOurCard:
                while (true)
                {
                    //每个卡牌该怎么做就怎么做
                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //如果是玩家这边执行完了，就停止这个循环，即暂停了游戏loop
                    if (GameState.whichTeamIsAttacking == 0) break;
                }
                break;

            case Information.PauseModeOfBattle.Legacy:
                while (true)
                {
                    //每个卡牌该怎么做就怎么做
                    await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //如果要强制停止(或者有一方没牌了），就这样退出游戏循环
                    if (forceToStop || GameState.CardOnSpot[0].Count == 0 || GameState.CardOnSpot[1].Count == 0) break;
                    await UniTask.Delay(10);
                }
                break;
        }

        //取消强制停止
        forceToStop = false;
        //执行打架结束事件
        battleEnd.Invoke();
    }



    /// <summary>
    /// 卡牌每一个回合该做什么
    /// </summary>
    /// <param name="teamId">哪一组要打</param>
    /// <returns></returns>
    private async UniTask CardRound(int teamId)
    {
        //记录是哪一组在打
        GameState.whichTeamIsAttacking = teamId;

        //GameState.CardOnSpot[teamId].Count会动态变化，千万不能提前缓存
        for (int i = 0; i < GameState.CardOnSpot[teamId].Count; i++)
        {
            //这一回合干活的卡
            var card = GetCardPanelOnSpot(teamId, i);

            //如果找到了一个这一回合还没打的卡牌的话，让他打，并终止for
            if (!card.thisRoundHasActiviated)
            {
                //但是，如果这个牌 在同一方 比起刚刚已经打过的卡牌的序号要小（即在左侧多了个牌），跳过去，并认为他打了
                if(GameState.whichCardPerforming[teamId] > card.cardId)
                {
                    card.thisRoundHasActiviated = true;
                    continue;
                }

                //记录一下，现在是这个牌在打
                GameState.whichCardPerforming[teamId] = i;

                if(testMode != null)
                {
                    testMode.RoundLoggerManager($"team:{teamId}的“{GetCardPanelOnSpot(teamId,i).Profile.FriendlyCardName}”(排序:{i})正在活动");
                }

                //执行卡牌这一回合该做的事情              

                //没有沉默，正常打架
                if (card.silence <= 0)
                {
                    //沉默回合数<0，＋1
                    if(card.silence < 0)  card.silence++;
                    //执行每回合都执行的攻击逻辑
                   await card.Attack(GetCardPanelOnSpot(teamId == 0? 1:0,UnityEngine.Random.Range(0,GameState.CardOnSpot[teamId == 0 ? 1 : 0].Count)));
                }
                //沉默了的，不打架了
                else
                {
                    card.silence--;
                }

                    //记录一下，这个牌打过了
                    card.thisRoundHasActiviated = true;

                //如果最右侧的卡打完了（打架逻辑在上面，则重置这一方所有的卡
                if (i == GameState.CardOnSpot[teamId].Count - 1)
                {
                    foreach (var item in GameState.CardOnSpot[teamId])
                    {
                        item.thisRoundHasActiviated = false;
                    }
                    GameState.whichCardPerforming[teamId] = 0;
                }
                //没打完，遍历停在这个卡牌
                else break;
                
            }
            else
            {
                continue;
            }



        }

       
    }

    #endregion


    #region 卡牌调整逻辑


    /// <summary>
    /// 将某一卡牌上场，并在舞台上显示（正式游戏中要求提前加载好所需的资源）
    /// </summary>
    /// <param name="Profile">卡牌配置</param>
    /// <param name="teamId"></param>
    /// <param name="coverImage"></param>
    /// <param name="Debut"></param>
    /// <param name="ability"></param>
    /// <param name="Defeat"></param>
    /// <param name="Exit"></param>
    /// <param name="index">放置位置。-1则为最后一个卡，0则放在第一个，1放在第二个，以此类推，最大5</param>
    /// <returns></returns>
    public async UniTask<CardPanel> AddCardAndDisplayInStage(CharacterCard Profile, int teamId, Sprite coverImage, AudioClip Debut, AudioClip ability, AudioClip Defeat, AudioClip Exit,int index = -1)
    {
        //这个对外显示
        CardPanel panel = null;

        //还没超过上限的话 //以后要解除上线
        if (GetCardCount(teamId) < Information.TeamMaxCardOnSpotCount)
        {
            if(panel == null)
            {
                //新建一个卡面
                panel = Instantiate(cardPrefeb, Vector2.zero, Quaternion.identity, TeamParent(teamId));

                //信息填充
                panel.Profile = Profile;
                //对于音频资源和图片资源的填充，零待商榷
            }

            //层级视图中的上下关系修正
            if (index >= 0) panel.tr.SetSiblingIndex(index + 6);

            //记录此卡牌
            if (index < 0) GameState.CardOnSpot[teamId].Add(panel);
            else GameState.CardOnSpot[teamId].Insert(index, panel);

            //进入到游戏模式中
            panel.EnterGameMode();
            //赋予teamId
            panel.teamId = teamId;
            //整理场上的卡牌排序
            ArrangeTeamCardOnSpot(teamId);
            //修改物体名称
            panel.gameObject.name = $"{panel.Profile.CardName}";


            //如果在打架，那就执行一下Ondebut
            if(GameState.gameState == Information.GameState.Competition)
            {
                await panel.OnDebut();
            }

            return panel;

        }

        return panel;
    }

 



    /// <summary>
    /// 整理某一队的卡牌，并将其展示在游戏舞台上（可能耗能高点）
    /// </summary>
    /// <param name="teamId"></param>
    void ArrangeTeamCardOnSpot(int teamId)
    {
        // TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length) 相当于获取这一组卡牌中的第一张卡牌

        //缓存一下，这一组有多少子对象
        var totalChildCount = TeamParent(teamId).childCount;
        //卡牌数量 总子对象数目 - 定位用对象数目
        var cardCount = totalChildCount - CardPrePoint[teamId].PrePoint.Length;

        //中间两个定位点间的中位数（x轴）
        var pointMiddle = TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2 + 1).position.x - TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2).position.x;
        //右移移动距离
        float distance;
        if (cardCount % 2 == 0)
        {
            //偶数个卡牌
            //中间两种卡牌的坐标（x轴）的中位数
            var middle = (TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x + TeamParent(teamId).GetChild(cardCount / 2).position.x) / 2f;
            //计算移动距离
            distance = pointMiddle - middle;
        }
        else
        {
            //奇数个卡牌，直接计算移动距离就行（中间那个卡牌当作middle）
            distance = pointMiddle - TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x;
        }
        //（对每一张卡牌的变换组件进行操作）最后，把卡牌左对齐，并在定位点上，在进行缩进，将所有卡牌定位到中间位置
        //排序是按照这些卡牌在层级视图中的上下关系决定的
        for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
        {
            //TeamA(Player)的第 CardPrePoint[teamId].PrePoint.Length 个子对象（从0开始数）就是第一张卡牌
           
            //卡牌的变换组件
            var card = TeamParent(teamId).GetChild(i);
            //仅限被激活的卡牌排序
            if (card.gameObject.activeSelf)
            {
                //左对齐放置
                //i - Information.TeamMaxCardOnSpotCount：第 1 2 3 4 5 6个定位点 //之后得解除场上最多6张卡牌的限制
                card.position = CardPrePoint[teamId].PrePoint[i - Information.TeamMaxCardOnSpotCount].position;
                //缩进移动
                //计算缩进量（卡牌全满了就不用缩进了，这里先默认卡牌左对齐，并在定位点上）
                if (cardCount < Information.TeamMaxCardOnSpotCount)
                {
                    card.Translate(distance, 0f, 0f, Space.World);
                }

                //赋予cardId
                GameState.CardOnSpot[teamId][i - 6].cardId = i - 6;
            }
            //没被激活的话，还是报错吧，这里不应该有没被激活的
            else
            {
                Debug.LogError($"卡牌“{card.name}”（第{teamId}第{i}个）未激活，应当考虑是否将其放入{nameof(CardCache)}中");
            }
              
        }

    }

    /// <summary>
    /// 移除某个组内所有的卡牌
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    public void RemoveAllCardsOnSpot(int teamId)
    {
        //卡牌数量 总子对象数目 - 定位用对象数目
        var cardCount = TeamParent(teamId).childCount - CardPrePoint[teamId].PrePoint.Length;
        //
        for (int i = 0; i < cardCount; i++)
        {
            RemoveCardOnSpot(teamId, 0,false);            
        }            
    }

    /// <summary>
    /// 移除某个组的某个卡牌，并释放资源
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">第几张卡（从0开始）</param>
    /// <param name="autoArrange">是否允许移除后自动整理卡牌排序</param>
    public void RemoveCardOnSpot(int teamId, int index,bool autoArrange = true)
    {

        //此组卡牌数量 总子对象数目 - 定位用对象数目
        var cardCount = GetCardCount(teamId);
        if (cardCount > 0)
        {
            //消除游戏中的显示
            DestroyImmediate(GetCardTransformOnSpot(teamId,index).gameObject,true);
            //消除游戏记录
            GameState.CardOnSpot[teamId].RemoveAt(index);
            if (autoArrange) ArrangeTeamCardOnSpot(teamId);
        }       
    }

    /// <summary>
    ///  （用于卡牌打输了）回收某一个卡牌，使其回到缓存中
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="index"></param>
    /// <param name="autoArrange"></param>
    public void RecycleCardOnSpot(int teamId, int index, bool autoArrange = true)
    {
        //将此卡牌移到缓存中，并将其隐藏（父对象is disactive）
       GetCardTransformOnSpot(teamId, index).parent = CardCache;
        Destroy(GetCardTransformOnSpot(teamId, index).gameObject);
        //消除游戏记录
        GameState.CardOnSpot[teamId].RemoveAt(index);
        if (autoArrange) ArrangeTeamCardOnSpot(teamId);
    }
    #endregion

    #region 获取卡片


    /// <summary>
    /// 获取场上一个卡牌的变换组件
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Transform GetCardTransformOnSpot(int teamId, int index)
    {
        //之前缓存了
        return GameState.CardOnSpot[teamId][index].tr;
    }

    public CardPanel GetCardPanelOnSpot(int teamId, int index)
    {
        //之前缓存了
        return GameState.CardOnSpot[teamId][index];
    }

    /// <summary>
    /// 获取场上的某一个卡牌
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">第几张卡（从0开始）</param>
    /// <returns></returns>
    public object GetCardOnSpot<T>(int teamId, int index)
    {
        //TeamA(Player)的第 CardPrePoint[teamId].PrePoint.Length 个子对象（从0开始数）就是第一张卡牌
        var cardPanel = TeamParent(teamId).GetChild(Information.TeamMaxCardOnSpotCount + index);
        //   var cardPanel = TeamParent(teamId).GetChild(index + CardPrePoint[teamId].PrePoint.Length);
       

        if (typeof(T) == typeof(Transform))
        {
            Debug.LogError($"use {nameof(GetCardTransformOnSpot)} instead");
            return null;
        }
        else if (typeof(T) == typeof(CardPanel))
        {
            Debug.LogError($"use {nameof(GetCardPanelOnSpot)} instead");
            return null;           
        }
        else
        {
            return cardPanel.GetComponent<T>();
        }
    }

    /// <summary>
    /// 获取缓存中的所有卡牌panel组件
    /// </summary>
    /// <returns></returns>
    public CardPanel[] GetAllCardPanelsInCache() => CardCache.GetComponentsInChildren<CardPanel>();



    /// <summary>
    /// 获取场上的所有卡牌
    /// </summary>
    /// <returns></returns>
    public CardPanel[] GetAllCardOnStage(int teamId)
    {
        //缓存一下，这一组有多少子对象
        var totalChildCount = TeamParent(teamId).childCount;
        //此组卡牌数量 总子对象数目 - 定位用对象数目
        var cardCount = totalChildCount - CardPrePoint[teamId].PrePoint.Length;
        var cardPanels = new CardPanel[cardCount];
        //获取每一个卡牌的panel组件
        for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
        {
            //卡牌的变换组件
            var card = TeamParent(teamId).GetChild(i);
            cardPanels[i - CardPrePoint[teamId].PrePoint.Length] = card.GetComponent<CardPanel>();
        }
        return cardPanels;
    }

    /// <summary>
    /// 获取某一组卡牌的数量
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <returns></returns>
    public int GetCardCount(int teamId)
    {
        return GameState.CardOnSpot[teamId].Count;
    }
    #endregion

    #region 信息输出与输入
    /// <summary>
    /// 展示能力的一些新信息（格式弄好了）
    /// </summary>
   internal void ShowAbilityNews(string Subject, string Recepetors, string DoWhat)
    {      

        //如果开着测试模式
        if(testMode != null)
        {
            string content = string.Empty;
            if (string.IsNullOrEmpty(Subject))
            {
                throw new ArgumentException($"“{nameof(Subject)}”不能为 null 或空。", nameof(Subject));
            }

            if (string.IsNullOrEmpty(DoWhat))
            {
                throw new ArgumentException($"“{nameof(DoWhat)}”不能为 null 或空。", nameof(DoWhat));
            }

            if (string.IsNullOrEmpty(Recepetors))
            {
                content = string.Format("“{0}” {1}", Subject, DoWhat);
            }
            else
            {
                content = string.Format("“{0}” 使 “{1}” {2}", Subject, Recepetors, DoWhat);
            }

            testMode.RoundLoggerManager(content);

        }


    }
    #endregion

    /// <summary>
    /// 每一个卡牌点位分组的父对象（就是那个TeamA(Player))
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <returns></returns>
    private Transform TeamParent(int teamId)
    {
        return CardPrePoint[teamId].PrePoint[0].parent;
    }

    [System.Serializable]
    private class Team
    {
        public Transform[] PrePoint = new Transform[Information.TeamMaxCardOnSpotCount];
    }
}

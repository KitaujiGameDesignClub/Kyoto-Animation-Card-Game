using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;

/// <summary>
/// 在加载好卡牌的情况下，游戏的本体逻辑应当都在这边
/// </summary>
public class GameStageCtrl : MonoBehaviour
{
    public static GameStageCtrl stageCtrl;

    public CardPanel cardPrefeb;

    /// <summary>
    /// 卡牌预用点位 A=0 B=1
    /// </summary>
    [SerializeField] private Team[] CardPrePoint  = new Team[2];

    /// <summary>
    /// 每个卡牌之间 攻击的间隔(ms)
    /// </summary>
    [Header("打架逻辑")]
    public int basicCardInterval = 300;
    /// <summary>
    /// 强制停止游戏逻辑
    /// </summary>
    public bool forceToStop = false;

    private void Awake()
    {
        stageCtrl = this;
       
    }

    #region 打架逻辑
    /// <summary>
    /// 初始化社团之间的战场
    /// </summary>
    void InitializeClubBattleField()
    {
        //初始化一个新游戏
        GameState.CreateNewGame();
    }

    /// <summary>
    /// 打架系统
    /// </summary>
    /// <param name="pauseModeOfBattle"></param>
    /// <returns></returns>
   async UniTask BattleSystem(GameState.PauseModeOfBattle pauseModeOfBattle)
    {
        switch (pauseModeOfBattle)
        {
            //每张卡牌运行后，都要暂停
            case GameState.PauseModeOfBattle.EachCard:

                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1:0);
                
                break;

            case GameState.PauseModeOfBattle.EachEnemyCard:
                //玩家这边优先开打（这一轮做什么）     
                await CardRound(0);
                //卡牌间隔
                await UniTask.Delay(basicCardInterval);
                //敌人打
                await CardRound(1);
                break;

            case GameState.PauseModeOfBattle.EachOurCard:
                while (true)
                {
                    //每个卡牌该怎么做就怎么做
                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //如果是玩家这边执行完了，就停止这个循环，即暂停了游戏loop
                    if (GameState.whichTeamIsAttacking == 0) break;
                }
                break;

            case GameState.PauseModeOfBattle.Legacy:
                while (true)
                {
                    //每个卡牌该怎么做就怎么做
                    await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //如果要强制停止，就这样退出游戏循环
                    if (forceToStop) break;
                }
                break;
        }

        //玩家这边优先开打（这一轮做什么）     
        await CardRound(0);  
        //卡牌间隔
        await UniTask.Delay(basicCardInterval);
        //敌人打
        await CardRound(1);
    

       
    }



    /// <summary>
    /// 卡牌每一个回合该做什么
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async UniTask CardRound(int teamId)
    {
        //记录是哪一组在打
        GameState.whichTeamIsAttacking = teamId;
        
        for (int i = 0; i < GameState.CardOnSpot[teamId].Count; i++)
        {
            var card = ((CardPanel)GetCardOnSpot<CardPanel>(teamId, i));

            //如果找到了一个这一回合还没打的卡牌的话，让他打，并终止for
            if (!card.cardStateInGame.thisRoundHasActiviated)
            {
                //记录一下，现在是这个牌在打
                GameState.whichCardPerforming[teamId] = i;

                //执行卡牌这一回合该做的事情              

                    //没有沉默，正常打架
                    if (card.cardStateInGame.silence <= 0)
                    {
                        //先执行每回合都执行的攻击逻辑
                        card.Attack(card.cardStateInGame);
                        //然后是动画相关的
                        await card.AnimationForNormal(GetCardTransformOnSpot(teamId == 0 ? 1 : 0, Random.Range(0, GameState.CardOnSpot[1].Count)).position);
                    }
                    //沉默了的，不打架了
                    else
                    {
                        card.cardStateInGame.silence--;
                    }

                    //记录一下，这个牌打过了
                    card.cardStateInGame.thisRoundHasActiviated = true;
                
                //循环停在这个卡牌
                break;
            }
            else
            {
                continue;
            }
        }

       
    }

    #endregion

    /// <summary>
    /// 将某一卡牌上场，并在舞台上显示（正式游戏中要求提前加载好所需的资源）
    /// </summary>
    public CardPanel AddCardAndDisplayInStage(CharacterCard profile, int teamId, Sprite coverImage, AudioClip Debut, AudioClip ability, AudioClip Defeat, AudioClip Exit)
    {
       
        var card = AddCard(profile, teamId);

        //能添加进来的话（指人数没有超过限制，就不是null）
        if (card != null)
        {
            //资源填充
            card.voiceAbility = ability;
            card.voiceExit = Exit;
            card.voiceDefeat = Defeat;
            card.voiceDebut = Debut;
            //如果不提供图片，则用此预设的默认图片
            card.CoverImage = coverImage;
            //卡面显示，并将卡牌信息panel进入到游戏模式中
            var panel = Instantiate(cardPrefeb, Vector2.zero, Quaternion.identity, TeamParent(teamId));
            panel.EnterGameMode(card);
            //整理场上的卡牌排序
            ArrangeTeamCardOnSpot(teamId);
            return panel;
        }
        else return null;
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
        for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
        {
            //TeamA(Player)的第 CardPrePoint[teamId].PrePoint.Length 个子对象（从0开始数）就是第一张卡牌
           
            //卡牌的变换组件
            var card = TeamParent(teamId).GetChild(i);
            //仅限被激活的卡牌排序
            if (card.gameObject.activeSelf)
            {
                //左对齐放置
                //i - Information.TeamMaxCardOnSpotCount：第 1 2 3 4 5 6个定位点
                card.position = CardPrePoint[teamId].PrePoint[i - Information.TeamMaxCardOnSpotCount].position;
                //缩进移动
                //计算缩进量（卡牌全满了就不用缩进了，这里先默认卡牌左对齐，并在定位点上）
                if (cardCount < Information.TeamMaxCardOnSpotCount)
                {
                    card.Translate(distance, 0f, 0f, Space.World);
                }
            }
            //没被激活的话。放那里就行
            else
            {
                continue;
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
    /// 移除某个组的某个卡牌
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
            DestroyImmediate(GetCardTransformOnSpot(teamId,index).gameObject);
            Debug.Log($"{index}  {GameState.CardOnSpot[teamId].Count}");
            GameState.CardOnSpot[teamId].RemoveAt(index);
            if (autoArrange) ArrangeTeamCardOnSpot(teamId);
        }       
    }


    #region 获取卡片
    /// <summary>
    /// 获取场上的某一个卡牌的游戏里状态
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">第几张卡（从0开始）</param>
    /// <returns></returns>
    public CharacterInGame GetCardOnSpot(int teamId, int index)
    {
        return GameState.CardOnSpot[teamId][index];
    }

    /// <summary>
    /// 获取场上一个卡牌的变换组件
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Transform GetCardTransformOnSpot(int teamId, int index)
    {
        return TeamParent(teamId).GetChild(6 + index);
    }

    public CardPanel GetCardPanelOnSpot(int teamId, int index) => ((CardPanel)GetCardOnSpot<CardPanel>(teamId, index));

    /// <summary>
    /// 获取场上的某一个卡牌
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">第几张卡（从0开始）</param>
    /// <returns></returns>
    public object GetCardOnSpot<T>(int teamId, int index)
    {
        //TeamA(Player)的第 CardPrePoint[teamId].PrePoint.Length 个子对象（从0开始数）就是第一张卡牌
        var cardPanel = TeamParent(teamId).GetChild(6 + index);
        //   var cardPanel = TeamParent(teamId).GetChild(index + CardPrePoint[teamId].PrePoint.Length);
        if (typeof(T) == typeof(Transform))
        {
            return cardPanel;
        }
        else
        {
            return cardPanel.GetComponent<T>();
        }
    }


    #endregion



    /// <summary>
    /// 给某个组添加出场卡牌（但是没有对用户显示，即仅有里状态）
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="teamId">0=A 1=B</param>
    CharacterInGame AddCard(CharacterCard profile, int teamId)
    {
        //此队伍的出场卡牌够多了（指到达上限6个）
        if (GameState. CardOnSpot[teamId].Count >= Information.TeamMaxCardOnSpotCount)
        {
            return null;
        }
        //还能加
        else
        {
            var card = new CharacterInGame(profile, teamId);
            GameState.CardOnSpot[teamId].Add(card);
            return card;
        }
    }



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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class GameStageCtrl : MonoBehaviour
{
    public static GameStageCtrl stageCtrl;

    public CardPanel cardPrefeb;

    /// <summary>
    /// 卡牌预用点位 A=0 B=1
    /// </summary>
    [SerializeField] private Team[] CardPrePoint  = new Team[2];


    private void Awake()
    {
        stageCtrl = this;
        //初始化一个新游戏
        GameState.CreateNewGame();
    }

    /// <summary>
    /// 添加卡牌，并在舞台上显示（正式游戏中要求提前加载好所需的资源）
    /// </summary>
    public CardPanel AddCardAndDisplayInStage(CharacterCard profile,int teamId,Sprite coverImage,AudioClip Debut,AudioClip Defeat,AudioClip Exit,AudioClip ability)
    {
        var card = GameState.AddCard(profile, teamId);

        //能添加进来的话（指人数没有超过限制，就不是null）
       if (card != null)
        {
            //资源填充
            card.voiceAbility = ability;
            card.voiceExit = Exit;
            card.voiceDefeat = Defeat;
            card.voiceDebut = Debut;
            card.CoverImage = coverImage == null ? card.CoverImage : coverImage;
            //卡面显示，并将卡牌信息panel进入到游戏模式中
            var panel =  Instantiate(cardPrefeb, TeamParent(teamId));
            panel.EnterGameMode(card);
            //整理场上的卡牌排序
            ArrangeTeamCardOnSpot(teamId);
            return panel;

        }
        else
        {
            return null;
        }
    }



    /// <summary>
    /// 整理某一队的卡牌，并将其展示在游戏舞台上（可能耗能高点）
    /// </summary>
    /// <param name="teamId"></param>
    void ArrangeTeamCardOnSpot(int teamId)
    {
        // TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length) 相当于获取这一组卡牌中的第一张卡牌

        //缓存一下，每一组有多少子对象
        var totalChildCount = TeamParent(teamId).childCount;
        //卡牌数量 总子对象数目 - 定位用对象数目
        var cardCount = totalChildCount - CardPrePoint[teamId].PrePoint.Length;     
        //计算缩进量（卡牌全满了就不用缩进了，这里先默认卡牌左对齐，并在定位点上）
        if(cardCount < Information.TeamMaxCardOnSpotCount)
        {
            float distance = 0f;//移动距离
            //中间两个定位点间的中位数（x轴）
            var pointMiddle = TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2 + 1).position.x - TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2).position.x;
            if (cardCount % 2 == 0)
            {
                //偶数个卡牌
                //中间两种卡牌的坐标（x轴）的中位数
                var middle = (TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x + TeamParent(teamId).GetChild(cardCount / 2).position.x)/2f;
                //计算移动距离
                distance = pointMiddle - middle;
            }
            else
            {
                //奇数个卡牌，直接计算移动距离就行（中间那个卡牌当作middle）
                distance = pointMiddle - TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x;
            }
            //最后，把卡牌左对齐，并在定位点上，在进行缩进，将所有卡牌定位到中间位置
            for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
            {
                var card = TeamParent(teamId).GetChild(i);
                //左对齐放置
                card.position = CardPrePoint[teamId].PrePoint[totalChildCount - i - 1].position;
                //缩进移动
                card.Translate(distance, 0f, 0f, Space.World);
            }
        }
       

    }
    /// <summary>
    /// 每一个卡牌点位分组的父对象
    /// </summary>
    /// <param name="teamId"></param>
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

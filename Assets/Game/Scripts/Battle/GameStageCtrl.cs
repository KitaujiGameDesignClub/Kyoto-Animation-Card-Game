using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// �ڼ��غÿ��Ƶ�����£���Ϸ�ı����߼�Ӧ���������
/// </summary>
public class GameStageCtrl : MonoBehaviour
{
    public static GameStageCtrl stageCtrl;

    public CardPanel cardPrefeb;

    /// <summary>
    /// ����Ԥ�õ�λ A=0 B=1
    /// </summary>
    [SerializeField] private Team[] CardPrePoint  = new Team[2];


    private void Awake()
    {
        stageCtrl = this;
        //��ʼ��һ������Ϸ
        GameState.CreateNewGame();
    }

    /// <summary>
    /// ��ӿ��ƣ�������̨����ʾ����ʽ��Ϸ��Ҫ����ǰ���غ��������Դ��
    /// </summary>
    public CardPanel AddCardAndDisplayInStage(CharacterCard profile,int teamId,Sprite coverImage,AudioClip Debut, AudioClip ability, AudioClip Defeat,AudioClip Exit)
    {
        var card = GameState.AddCard(profile, teamId);

        //����ӽ����Ļ���ָ����û�г������ƣ��Ͳ���null��
       if (card != null)
        {
            //��Դ���
            card.voiceAbility = ability;
            card.voiceExit = Exit;
            card.voiceDefeat = Defeat;
            card.voiceDebut = Debut;
            card.CoverImage = coverImage == null ? card.CoverImage : coverImage;
            //������ʾ������������Ϣpanel���뵽��Ϸģʽ��
            var panel =  Instantiate(cardPrefeb, Vector2.zero,Quaternion.identity, TeamParent(teamId));
            panel.EnterGameMode(card);
            //�����ϵĿ�������
            ArrangeTeamCardOnSpot(teamId);
            return panel;

        }
        else
        {
            return null;
        }
    }



    /// <summary>
    /// ����ĳһ�ӵĿ��ƣ�������չʾ����Ϸ��̨�ϣ����ܺ��ܸߵ㣩
    /// </summary>
    /// <param name="teamId"></param>
    void ArrangeTeamCardOnSpot(int teamId)
    {
        // TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length) �൱�ڻ�ȡ��һ�鿨���еĵ�һ�ſ���

        //����һ�£���һ���ж����Ӷ���
        var totalChildCount = TeamParent(teamId).childCount;
        //�������� ���Ӷ�����Ŀ - ��λ�ö�����Ŀ
        var cardCount = totalChildCount - CardPrePoint[teamId].PrePoint.Length;

        //�м�������λ������λ����x�ᣩ
        var pointMiddle = TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2 + 1).position.x - TeamParent(teamId).GetChild(CardPrePoint[teamId].PrePoint.Length / 2).position.x;
        //�����ƶ�����
        float distance;
        if (cardCount % 2 == 0)
        {
            //ż��������
            //�м����ֿ��Ƶ����꣨x�ᣩ����λ��
            var middle = (TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x + TeamParent(teamId).GetChild(cardCount / 2).position.x) / 2f;
            //�����ƶ�����
            distance = pointMiddle - middle;
        }
        else
        {
            //���������ƣ�ֱ�Ӽ����ƶ�������У��м��Ǹ����Ƶ���middle��
            distance = pointMiddle - TeamParent(teamId).GetChild(cardCount / 2 + 1).position.x;
        }
        //����ÿһ�ſ��Ƶı任������в�������󣬰ѿ�������룬���ڶ�λ���ϣ��ڽ��������������п��ƶ�λ���м�λ��
        for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
        {
            //TeamA(Player)�ĵ� CardPrePoint[teamId].PrePoint.Length ���Ӷ��󣨴�0��ʼ�������ǵ�һ�ſ���

            //���Ƶı任���
            var card = TeamParent(teamId).GetChild(i);
            //��������
            //i - Information.TeamMaxCardOnSpotCount���� 1 2 3 4 5 6����λ��
            card.position = CardPrePoint[teamId].PrePoint[i - Information.TeamMaxCardOnSpotCount].position;
            //�����ƶ�
            //����������������ȫ���˾Ͳ��������ˣ�������Ĭ�Ͽ�������룬���ڶ�λ���ϣ�
            if (cardCount < Information.TeamMaxCardOnSpotCount)
            {
                card.Translate(distance, 0f, 0f, Space.World);
            }
        }

    }
  
    /// <summary>
    /// ÿһ�����Ƶ�λ����ĸ����󣨾����Ǹ�TeamA(Player))
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private Transform TeamParent(int teamId)
    {
        return CardPrePoint[teamId].PrePoint[0].parent;
    }

    /// <summary>
    /// ��ȡ���ϵ����п���
    /// </summary>
    /// <returns></returns>
    public CardPanel[] GetAllCardOnStage(int teamId)
    {
        //����һ�£���һ���ж����Ӷ���
        var totalChildCount = TeamParent(teamId).childCount;
        //�������� ���Ӷ�����Ŀ - ��λ�ö�����Ŀ
        var cardCount = totalChildCount - CardPrePoint[teamId].PrePoint.Length;
        var cardPanels = new CardPanel[cardCount];
        //��ȡÿһ�����Ƶ�panel���
        for (int i = CardPrePoint[teamId].PrePoint.Length; i < totalChildCount; i++)
        {
            //���Ƶı任���
            var card = TeamParent(teamId).GetChild(i);
            cardPanels[i - CardPrePoint[teamId].PrePoint.Length] = card.GetComponent<CardPanel>();
        }
        return cardPanels;
    }
    

    [System.Serializable]
    private class Team
    {
        public Transform[] PrePoint = new Transform[Information.TeamMaxCardOnSpotCount];
    }
}

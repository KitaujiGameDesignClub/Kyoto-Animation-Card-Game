using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;

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

    /// <summary>
    /// ÿ������֮�� �����ļ��(ms)
    /// </summary>
    [Header("����߼�")]
    public int basicCardInterval = 300;
    /// <summary>
    /// ǿ��ֹͣ��Ϸ�߼�
    /// </summary>
    public bool forceToStop = false;

    private void Awake()
    {
        stageCtrl = this;
       
    }

    #region ����߼�
    /// <summary>
    /// ��ʼ������֮���ս��
    /// </summary>
    void InitializeClubBattleField()
    {
        //��ʼ��һ������Ϸ
        GameState.CreateNewGame();
    }

    /// <summary>
    /// ���ϵͳ
    /// </summary>
    /// <param name="pauseModeOfBattle"></param>
    /// <returns></returns>
   async UniTask BattleSystem(GameState.PauseModeOfBattle pauseModeOfBattle)
    {
        switch (pauseModeOfBattle)
        {
            //ÿ�ſ������к󣬶�Ҫ��ͣ
            case GameState.PauseModeOfBattle.EachCard:

                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1:0);
                
                break;

            case GameState.PauseModeOfBattle.EachEnemyCard:
                //���������ȿ�����һ����ʲô��     
                await CardRound(0);
                //���Ƽ��
                await UniTask.Delay(basicCardInterval);
                //���˴�
                await CardRound(1);
                break;

            case GameState.PauseModeOfBattle.EachOurCard:
                while (true)
                {
                    //ÿ�����Ƹ���ô������ô��
                   await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //�����������ִ�����ˣ���ֹͣ���ѭ��������ͣ����Ϸloop
                    if (GameState.whichTeamIsAttacking == 0) break;
                }
                break;

            case GameState.PauseModeOfBattle.Legacy:
                while (true)
                {
                    //ÿ�����Ƹ���ô������ô��
                    await CardRound(GameState.whichTeamIsAttacking == 0 ? 1 : 0);
                    //���Ҫǿ��ֹͣ���������˳���Ϸѭ��
                    if (forceToStop) break;
                }
                break;
        }

        //���������ȿ�����һ����ʲô��     
        await CardRound(0);  
        //���Ƽ��
        await UniTask.Delay(basicCardInterval);
        //���˴�
        await CardRound(1);
    

       
    }



    /// <summary>
    /// ����ÿһ���غϸ���ʲô
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    private async UniTask CardRound(int teamId)
    {
        //��¼����һ���ڴ�
        GameState.whichTeamIsAttacking = teamId;
        
        for (int i = 0; i < GameState.CardOnSpot[teamId].Count; i++)
        {
            var card = ((CardPanel)GetCardOnSpot<CardPanel>(teamId, i));

            //����ҵ���һ����һ�غϻ�û��Ŀ��ƵĻ��������򣬲���ֹfor
            if (!card.cardStateInGame.thisRoundHasActiviated)
            {
                //��¼һ�£�������������ڴ�
                GameState.whichCardPerforming[teamId] = i;

                //ִ�п�����һ�غϸ���������              

                    //û�г�Ĭ���������
                    if (card.cardStateInGame.silence <= 0)
                    {
                        //��ִ��ÿ�غ϶�ִ�еĹ����߼�
                        card.Attack(card.cardStateInGame);
                        //Ȼ���Ƕ�����ص�
                        await card.AnimationForNormal(GetCardTransformOnSpot(teamId == 0 ? 1 : 0, Random.Range(0, GameState.CardOnSpot[1].Count)).position);
                    }
                    //��Ĭ�˵ģ��������
                    else
                    {
                        card.cardStateInGame.silence--;
                    }

                    //��¼һ�£�����ƴ����
                    card.cardStateInGame.thisRoundHasActiviated = true;
                
                //ѭ��ͣ���������
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
    /// ��ĳһ�����ϳ���������̨����ʾ����ʽ��Ϸ��Ҫ����ǰ���غ��������Դ��
    /// </summary>
    public CardPanel AddCardAndDisplayInStage(CharacterCard profile, int teamId, Sprite coverImage, AudioClip Debut, AudioClip ability, AudioClip Defeat, AudioClip Exit)
    {
       
        var card = AddCard(profile, teamId);

        //����ӽ����Ļ���ָ����û�г������ƣ��Ͳ���null��
        if (card != null)
        {
            //��Դ���
            card.voiceAbility = ability;
            card.voiceExit = Exit;
            card.voiceDefeat = Defeat;
            card.voiceDebut = Debut;
            //������ṩͼƬ�����ô�Ԥ���Ĭ��ͼƬ
            card.CoverImage = coverImage;
            //������ʾ������������Ϣpanel���뵽��Ϸģʽ��
            var panel = Instantiate(cardPrefeb, Vector2.zero, Quaternion.identity, TeamParent(teamId));
            panel.EnterGameMode(card);
            //�����ϵĿ�������
            ArrangeTeamCardOnSpot(teamId);
            return panel;
        }
        else return null;
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
            //���ޱ�����Ŀ�������
            if (card.gameObject.activeSelf)
            {
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
            //û������Ļ������������
            else
            {
                continue;
            }
              
        }

    }

    /// <summary>
    /// �Ƴ�ĳ���������еĿ���
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    public void RemoveAllCardsOnSpot(int teamId)
    {
        //�������� ���Ӷ�����Ŀ - ��λ�ö�����Ŀ
        var cardCount = TeamParent(teamId).childCount - CardPrePoint[teamId].PrePoint.Length;
        //
        for (int i = 0; i < cardCount; i++)
        {
            RemoveCardOnSpot(teamId, 0,false);            
        }            
    }

    /// <summary>
    /// �Ƴ�ĳ�����ĳ������
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">�ڼ��ſ�����0��ʼ��</param>
    /// <param name="autoArrange">�Ƿ������Ƴ����Զ�����������</param>
    public void RemoveCardOnSpot(int teamId, int index,bool autoArrange = true)
    {

        //���鿨������ ���Ӷ�����Ŀ - ��λ�ö�����Ŀ
        var cardCount = GetCardCount(teamId);
        if (cardCount > 0)
        {
            DestroyImmediate(GetCardTransformOnSpot(teamId,index).gameObject);
            Debug.Log($"{index}  {GameState.CardOnSpot[teamId].Count}");
            GameState.CardOnSpot[teamId].RemoveAt(index);
            if (autoArrange) ArrangeTeamCardOnSpot(teamId);
        }       
    }


    #region ��ȡ��Ƭ
    /// <summary>
    /// ��ȡ���ϵ�ĳһ�����Ƶ���Ϸ��״̬
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">�ڼ��ſ�����0��ʼ��</param>
    /// <returns></returns>
    public CharacterInGame GetCardOnSpot(int teamId, int index)
    {
        return GameState.CardOnSpot[teamId][index];
    }

    /// <summary>
    /// ��ȡ����һ�����Ƶı任���
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
    /// ��ȡ���ϵ�ĳһ������
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <param name="index">�ڼ��ſ�����0��ʼ��</param>
    /// <returns></returns>
    public object GetCardOnSpot<T>(int teamId, int index)
    {
        //TeamA(Player)�ĵ� CardPrePoint[teamId].PrePoint.Length ���Ӷ��󣨴�0��ʼ�������ǵ�һ�ſ���
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
    /// ��ĳ������ӳ������ƣ�����û�ж��û���ʾ����������״̬��
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="teamId">0=A 1=B</param>
    CharacterInGame AddCard(CharacterCard profile, int teamId)
    {
        //�˶���ĳ������ƹ����ˣ�ָ��������6����
        if (GameState. CardOnSpot[teamId].Count >= Information.TeamMaxCardOnSpotCount)
        {
            return null;
        }
        //���ܼ�
        else
        {
            var card = new CharacterInGame(profile, teamId);
            GameState.CardOnSpot[teamId].Add(card);
            return card;
        }
    }



    /// <summary>
    /// ��ȡ���ϵ����п���
    /// </summary>
    /// <returns></returns>
    public CardPanel[] GetAllCardOnStage(int teamId)
    {
        //����һ�£���һ���ж����Ӷ���
        var totalChildCount = TeamParent(teamId).childCount;
        //���鿨������ ���Ӷ�����Ŀ - ��λ�ö�����Ŀ
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

    /// <summary>
    /// ��ȡĳһ�鿨�Ƶ�����
    /// </summary>
    /// <param name="teamId">0=A 1=B</param>
    /// <returns></returns>
    public int GetCardCount(int teamId)
    {
        return GameState.CardOnSpot[teamId].Count;
    }

    /// <summary>
    /// ÿһ�����Ƶ�λ����ĸ����󣨾����Ǹ�TeamA(Player))
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

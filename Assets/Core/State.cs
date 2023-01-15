using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 用于储存和获取当前游戏的所有状态
    /// </summary>
    public static class GameState
    {

        /// <summary>
        /// 玩家各自所有可用的卡牌（包含手牌+场上的牌+可以被招募的牌）
        /// </summary>
        public static List<CharacterCard>[] AllAvailableCards = new List<CharacterCard>[2];


        /// <summary>
        /// 各自登场的角色卡（最多6个）
        /// </summary>
        public static List<CharacterInGame>[] CardOnSpot = new List<CharacterInGame>[2];
        /// <summary>
        /// 各自手牌里的角色卡（最多10个）
        /// </summary>
        public static List<CharacterInGame>[] CardInHand = new List<CharacterInGame>[2];


        /// <summary>
        /// 哪一队在攻击 0=A 1=B -1还没开始
        /// </summary>
        public static int whichTeamIsAttacking = -1;

        /// <summary>
        /// 哪一张卡正在执行攻击等有关逻辑（1-6）  A,B队
        /// </summary>
        public static int[] whichCardPerforming = { 1, 1 };

        /// <summary>
        /// 双方的部长
        /// </summary>
        public static Chief[] chiefs = new Chief[2];

        /// <summary>
        /// 创建新游戏
        /// </summary>
        public static void CreateNewGame()
        {
            AllAvailableCards = new List<CharacterCard>[2];
            CardOnSpot = new List<CharacterInGame>[2];
            CardOnSpot[0] = new();
            CardOnSpot[1] = new();
            CardInHand = new List<CharacterInGame>[2];
            CardInHand[0] = new();
            CardInHand[1] = new();
            whichTeamIsAttacking = -1;
            whichCardPerforming[0] = 1; whichCardPerforming[1] = 1;
            chiefs = new Chief[2];
        }

        /// <summary>
        /// 添加出场卡牌
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="teamId">0=A 1=B</param>
        public static CharacterInGame AddCard(CharacterCard profile, int teamId)
        {
            //此队伍的出场卡牌够多了（指到达上限6个）
            if (CardOnSpot[teamId].Count >= Information.TeamMaxCardOnSpotCount)
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


        public static async UniTask LoadResourceInBackground()
        {

        }
    }
}



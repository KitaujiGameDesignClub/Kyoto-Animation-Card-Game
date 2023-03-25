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
        /// 各自登场的角色卡（最多6个）
        /// </summary>
        public static List<CardPanel>[] CardOnSpot = new List<CardPanel>[2];

        /// <summary>
        /// 缓存所有的卡牌
        /// </summary>
        public static List<CardCache> cardCaches = new();

        /// <summary>
        /// 哪一队在攻击 0=A 1=B -1还没开始
        /// </summary>
        public static int whichTeamIsAttacking = -1;

        /// <summary>
        /// 哪一张卡正在执行攻击等有关逻辑（0-5）  A,B队
        /// </summary>
        public static int[] whichCardPerforming = { 0, 0 };

        /// <summary>
        /// 双方的部长
        /// </summary>
        public static Chief[] chiefs = new Chief[2];

        public static Information.GameState gameState = Information.GameState.Preparation;

        /// <summary>
        /// 创建新游戏
        /// </summary>
        public static void CreateNewGame()
        {
            cardCaches = new();
            CardOnSpot = new List<CardPanel>[2];
            CardOnSpot[0] = new();
            CardOnSpot[1] = new();
            whichTeamIsAttacking = -1;
            whichCardPerforming[0] = 0; whichCardPerforming[1] = 0;
            chiefs = new Chief[2];
            gameState = Information.GameState.Preparation;
        }
        
    }
}



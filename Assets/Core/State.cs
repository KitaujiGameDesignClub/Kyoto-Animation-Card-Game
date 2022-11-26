using System.Collections.Generic;

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
        /// 哪一队在攻击 0=A 1=B
        /// </summary>
        public static int whichTeamIsAttacking;

        /// <summary>
        /// 哪一张卡正在执行攻击等有关逻辑（1-6）
        /// </summary>
        public static int[] whichCardPerforming = { 1, 1 };

        /// <summary>
        /// 双方的部长
        /// </summary>
        public static Chief[] chiefs = new Chief[2];
    }
}



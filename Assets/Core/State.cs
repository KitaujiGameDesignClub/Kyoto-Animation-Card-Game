using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 用于储存和获取当前游戏的所有状态
    /// </summary>
    public static class State
    {
      

        /// <summary>
        /// 玩家A登场的角色卡（最多6个）
        /// </summary>
        public static List<CharacterInGame> CardOnSpotPlayerA;
        /// <summary>
        /// 角色A手牌里的角色卡（最多10个）
        /// </summary>
        public static List<CharacterInGame> CardInHandPlayerA;
        /// <summary>
        /// 玩家B登场的角色卡（最多6个）
        /// </summary>
        public static List<CharacterInGame> CardOnSpotPlayerB;
        /// <summary>
        /// 角色B手牌里的角色卡（最多10个）
        /// </summary>
        public static List<CharacterInGame> CardInHandPlayerB;
    }
}



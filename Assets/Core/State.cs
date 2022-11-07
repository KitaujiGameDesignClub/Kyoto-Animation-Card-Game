using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 用于储存和获取当前游戏的所有状态
    /// </summary>
    public class State
    {
        public enum GameStage
        {
            /// <summary>
            /// 准备游戏。在所有玩家准备之前，处于此阶段
            /// </summary>
            Preparation,
            /// <summary>
            /// 选择阵营（社团，Club）阶段，
            /// </summary>
            ClubSelection,
            /// <summary>
            /// 选择部长（主持，英雄）
            /// </summary>
            ChiefSelection,
            /// <summary>
            /// 社团纳新（招募随从）
            /// </summary>
            Recruitment,
            /// <summary>
            /// 竞争阶段（对战）
            /// </summary>
            Competition,
            /// <summary>
            /// 分出胜负，游戏结束
            /// </summary>
            Fin,
        }

        /// <summary>
        /// 玩家A登场的角色卡（最多6个）
        /// </summary>
        public List<Card> CardOnSpotPlayerA;
        /// <summary>
        /// 角色A手牌里的角色卡（最多10个）
        /// </summary>
        public List<Card> CardInHandPlayerA;
        /// <summary>
        /// 玩家B登场的角色卡（最多6个）
        /// </summary>
        public List<Card> CardOnSpotPlayerB;
        /// <summary>
        /// 角色B手牌里的角色卡（最多10个）
        /// </summary>
        public List<Card> CardInHandPlayerB;
    }
}



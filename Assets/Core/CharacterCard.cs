using System;

namespace Core
{
    /// <summary>
    /// 记录了每个角色卡的设定(yaml配置文件与游戏搭桥）
    /// </summary>
    [System.Serializable]
    public class CharacterCard
    {

        /// <summary>
        /// 卡牌名称（仅英语，文件名也是这个）
        /// </summary>
        public string CardName;

        /// <summary>
        /// 友好卡牌名，用来显示给玩家的
        /// </summary>
        public string FriendlyCardName;
        
        
        /// <summary>
        /// 此卡的总数。不宜过大。0则不在牌堆中出现，需要summon（召唤）
        /// </summary>
        public int CardCount;
        
        /// <summary>
        /// 图片名字（一般与角色配置文件同路径）
        /// </summary>
        public string imageName;
        
        /// <summary>
        /// 角色名称（无则空）
        /// </summary>
        public string CharacterName;

        /// <summary>
        /// 性别 -1 无性别或性别不重要 1男 0女
        /// </summary>
        public int gender;

        /// <summary>
        /// 所用声优
        /// </summary>
        public readonly Information.CV CV ;

        /// <summary>
        /// 所属卡组（可以自定义，一般是番剧名称，额外添加一些修饰符什么的，比如 中二世界-中二病也要谈恋爱、与中二病也要谈恋爱就分属两个不同的阵营了
        /// </summary>
        public readonly string BelongBundleName;

        /// <summary>
        /// 是否允许作为部长（主持，英雄）
        /// </summary>
        public bool allowAsChief;

        /// <summary>
        /// 基础生命值
        /// </summary>
        public int BasicHealthPoint;

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public int BasicPower;

        /// <summary>
        /// 此卡的能力类型
        /// </summary>
        public Information.CardAbilityTypes AbilityType = Information.CardAbilityTypes.None;

        public AbilityLogicReason Reason;

        public AbilityLogicResult Result;

        /// <summary>
        /// 能力描述
        /// </summary>
        public string AbilityDescription;

        /// <summary>
        /// 角色间羁绊
        /// </summary>
        public CharactersConnect[] Connects;


        public CharacterCard()
        {
            imageName = String.Empty;
            CardName = "ZhongTian.jpg";
            FriendlyCardName = "种田.jpg";
            CardCount = 1;
            CharacterName = string.Empty;
            gender = -1;
            CV = Information.CV.None;
            BelongBundleName = Information.Anime.Universal.ToString();
            allowAsChief = false;
            BasicHealthPoint = 3;
            BasicPower = 2;
            AbilityType = Information.CardAbilityTypes.Debut;
            Reason = new AbilityLogicReason(Information.Objects.AllOnSpot, Information.Parameter.CV,
                Information.JudgeMethod.Count, 2, "1");
            Result = new AbilityLogicResult(true, string.Empty,Information.Objects.AllOnSpot, Information.Parameter.State,
                Information.CalculationMethod.ChangeTo, Information.CardState.Available.ToString());
            
            AbilityDescription = "使场上所有相同声优的角色退场返回到准备区";
            Connects = null;
        }
    }
}
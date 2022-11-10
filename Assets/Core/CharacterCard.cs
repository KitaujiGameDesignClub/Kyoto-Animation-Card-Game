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
        /// 卡牌名称
        /// </summary>
        public string CardName;

        /// <summary>
        /// 此卡的总数。不宜过大
        /// </summary>
        public int CardCount;
        
        /// <summary>
        /// 图片路径（一般与角色配置文件同路径）
        /// </summary>
        public string imagePath;
        
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
        /// 所属动画
        /// </summary>
        public readonly Information.Anime Anime ;

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
            imagePath = String.Empty;
            CardName = "种田.jpg";
            CardCount = 1;
            CharacterName = string.Empty;
            gender = -1;
            CV = Information.CV.None;
            Anime = Information.Anime.Universal;
            allowAsChief = false;
            BasicHealthPoint = 3;
            BasicPower = 2;
            AbilityType = Information.CardAbilityTypes.Debut;
            Reason = new AbilityLogicReason(Information.Objects.AllOnSpot, Information.Parameter.CV,
                Information.JudgeMethod.Count, 2, "1");
            Result = new AbilityLogicResult(true, Information.Objects.AllOnSpot, Information.Parameter.State,
                Information.CalculationMethod.ChangeTo, Information.CardState.Available.ToString());
            
            AbilityDescription = "使场上所有相同声优的角色退场返回到准备区";
            Connects = null;
        }
    }
}
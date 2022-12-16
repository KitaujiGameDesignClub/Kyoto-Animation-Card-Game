using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 记录了每个角色卡的设定(yaml配置文件与游戏搭桥）
    /// </summary>
    [System.Serializable]
    public class CharacterCard
    {
        /// <summary>
        /// 角色卡文件代码版本号（如果代码上有修改，且修改后不兼容，就+1）
        /// </summary>
        public int CodeVersion = Information.CharacterCardMaker;
        
        /// <summary>
        /// 卡牌名称（仅英语，文件名也是这个）
        /// </summary>
        public string CardName = "CharacterCard";

        /// <summary>
        /// 友好卡牌名，用来显示给玩家的
        /// </summary>
        public string FriendlyCardName = "默认卡牌";

        /// <summary>
        /// 所属卡包的名字（阵营）
        /// </summary>
        public string BundleName = string.Empty;

        /// <summary>
        /// 角色标签
        /// </summary>
        public List<string> tags;

        /// <summary>
        /// 此卡的总数。不宜过大。0则不在牌堆中出现，需要summon（召唤）
        /// </summary>
        public int CardCount = 1;

        /// <summary>
        /// 图片名字（一般与角色配置文件同路径）
        /// </summary>
        public string imageName;

        /// <summary>
        /// 角色名称（无则空）
        /// </summary>
        public string CharacterName;

        /// <summary>
        /// 所用声优
        /// </summary>
        public string CV = Information.CV[0];

        /// <summary>
        /// 性别 0 无性别或性别不重要 1男 2女
        /// </summary>
        public int gender = -1;

        /// <summary>
        /// 是否允许作为部长（主持，英雄）
        /// </summary>
        public bool allowAsChief = true;

        /// <summary>
        /// 基础生命值
        /// </summary>
        public int BasicHealthPoint = 3;

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public int BasicPower = 2;

        /// <summary>
        /// 此能力的触发类型
        /// </summary>
        public Information.CardAbilityTypes AbilityActivityType = Information.CardAbilityTypes.None;

        public AbilityLogicReason Reason;

        public AbilityLogicResult Result;

        /// <summary>
        /// 能力描述
        /// </summary>
        public string AbilityDescription = "我很好奇能力是什么";

/*
        /// <summary>
        /// 角色间羁绊
        /// </summary>
        public CharactersConnect[] Connects;
*/
        /// <summary>
        /// 登场音频.ogg（仅限主持/部长/chief）
        /// </summary>
        public string voiceDebut;

        /// <summary>
        /// 退场音频 .ogg
        /// </summary>
        public string voiceExit;

        /// <summary>
        /// 能力触发音频 .ogg
        /// </summary>
        public string voiceAbility;
        


#if UNITY_EDITOR
        /// <summary>
        /// 在这里存一些例子
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static CharacterCard Examples(int index)
        {
            switch (index)
            {
                //节能折木，每回合降低自身攻击力，除非千反田在场
                case 0:
                    return new CharacterCard()
                    {
                       
                    };
                    break;

                default:
                    return new CharacterCard();
                    break;
            }
        }
#endif
    }
}
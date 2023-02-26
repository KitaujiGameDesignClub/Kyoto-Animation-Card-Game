using System;
using System.Collections.Generic;

#if  UNITY_EDITOR
using UnityEngine;
#endif


namespace Core
{
    /// <summary>
    /// 记录了每个角色卡的设定(yaml配置文件与游戏搭桥）
    /// </summary>
    [System.Serializable]
    public class CharacterCard
    {
        public string UUID = string.Empty;
        
        /// <summary>
        /// 角色卡文件代码版本号（如果代码上有修改，且修改后不兼容，就+1）
        /// </summary>
        public int CodeVersion = Information.CharacterCardMaker;
        
        /// <summary>
        /// 卡牌名称（仅英语，文件名也是这个）
        /// </summary>f
        public string CardName = string.Empty;

        public string AuthorName;
        
        /// <summary>
        /// 友好卡牌名，用来显示给玩家的
        /// </summary>
        public string FriendlyCardName = "默认卡牌";

        /// <summary>
        /// 所属动画
        /// </summary>
        public string Anime;

        /// <summary>
        /// 角色标签
        /// </summary>
        public List<string> tags = new();

        /// <summary>
        /// 此卡的总数。不宜过大。0则不在牌堆中出现，需要summon（召唤）
        /// </summary>
        public int CardCount = 1;

        /// <summary>
        /// 图片名字（含拓展名）
        /// </summary>
        public string ImageName;

        /// <summary>
        /// 角色名称（无则空）
        /// </summary>
        public string CharacterName;

        /// <summary>
        /// 所用声优
        /// </summary>
        public string CV = "不设置声优";

        /// <summary>
        /// 性别 0 无性别或性别不重要 1男 2女
        /// </summary>
        public int gender = 0;

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
#if  UNITY_EDITOR
        [TextArea]
        #endif
        public string AbilityDescription = "甚至可以写一点简短的小骚话\n我很好奇能力是什么";

        /* 暂时做不了，以后用别的东西实现
                /// <summary>
                /// 角色间羁绊
                /// </summary>
                public CharactersConnect[] Connects;
        */

        /// <summary>
        /// 退场音频的文件名（含拓展名）
        /// </summary>
        public string voiceExitFileName = "voiceExit.ogg";
        /// <summary>
        /// 击败音频的文件名（含拓展名）
        /// </summary>
        public string voiceDefeatFileName = "voiceDefeat.ogg";
        /// <summary>
        /// 登场音频的文件名（仅部长）（含拓展名）
        /// </summary>
        public string voiceDebutFileName = "voiceDebut.ogg";
        /// <summary>
        /// 能力发动音频的文件名（含拓展名）
        /// </summary>
        public string voiceAbilityFileName = "voiceAbility.ogg";


    }
}
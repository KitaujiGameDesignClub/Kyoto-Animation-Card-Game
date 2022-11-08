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
        public readonly string CardName;
        
        /// <summary>
        /// 角色名称
        /// </summary>
        public readonly string CharacterName;

        /// <summary>
        /// 性别 -1 无性别或性别不重要 1男 0女
        /// </summary>
        public readonly int gender;

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
        /// 生命值
        /// </summary>
        public int HealthPoint;

        /// <summary>
        /// 攻击力
        /// </summary>
        public int Power;

        /// <summary>
        /// 此卡能力
        /// </summary>
        public Information.CardAbilityTypes Ability = Information.CardAbilityTypes.None;

        /// <summary>
        /// 角色能力逻辑
        /// </summary>
        public AbilityLogic AbilityLogic;


        public CharacterCard()
        {
           // CardName = ""
        }
    }
}
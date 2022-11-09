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
        /// 角色名称（无则空）
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
        /// 此卡的能力类型
        /// </summary>
        public Information.CardAbilityTypes AbilityType = Information.CardAbilityTypes.None;

        /// <summary>
        /// 能力描述
        /// </summary>
        public string AbilityDescription;
        



        public CharacterCard()
        {
            CardName = "种田.jpg";
            CharacterName = string.Empty;
            gender = -1;
            CV = Information.CV.None;
            Anime = Information.Anime.Universal;
            allowAsChief = false;
            HealthPoint = 3;
            Power = 2;
            AbilityType = Information.CardAbilityTypes.Debut;
            AbilityDescription = "将场上";

        }
    }
}
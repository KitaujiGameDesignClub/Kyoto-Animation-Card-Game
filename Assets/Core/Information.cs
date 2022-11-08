namespace Core
{
    [System.Serializable]
    public struct AbilityLogic
    {
        /// <summary>
        /// 条件对象的检索范围
        /// </summary>
        public Information.Objects ScopeOfObj;

        /// <summary>
        /// 收缩范围以确定条件对象
        /// </summary>
        public Information.ExtraCondition ShrinkedScopeToSpecifyObj;

        /// <summary>
        /// 要对条件对象判断的参数
        /// </summary>
        public Information.ParameterToChange[] ObjParameter;

        /// <summary>
        /// 条件对象参数的逻辑 -1非 0或 1与
        /// </summary>
        public int[] ParameterLogic;

        /// <summary>
        /// 条件对象参数的阈值
        /// </summary>
        public string[] ParameterThreshold;
        
        
        //在ParameterLogic要求的逻辑下，所有要判断的条件对象的参数均为true时，才执行下面的逻辑
        
        
        /// <summary>
        /// 目标对象的检索范围
        /// </summary>
        public Information.Objects ScopeOfGoalObj;
        
        /// <summary>
        /// 确定目标对象用的额外条件
        /// </summary>
        public Information.ExtraCondition ShrinkedScopeToSpecifyGoalObj;
        
        /// <summary>
        /// 目标对象要修改的变量
        /// </summary>
        public Information.ParameterToChange[] ParameterToChange;
        
        /// <summary>
        /// 目标对象参数的变化数值
        /// </summary>
        public string[] values;
    }

    public class Information
    {

        /// <summary>
        /// 事件执行对象或事件检索范围
        /// </summary>
        public enum Objects
        {
            /// <summary>
            /// 发动者自身
            /// </summary>
            Self,

            /// <summary>
            /// 己方部长（主持，英雄）
            /// </summary>
            ChiefInTeam,

            /// <summary>
            /// 敌方部长（主持，英雄）
            /// </summary>
            ChiefOfEnemy,

            /// <summary>
            /// 己方场上随机一位角色
            /// </summary>
            RandomInTeam,

            /// <summary>
            /// 地方场上随机一位角色
            /// </summary>
            RandomOfEnemy,

            /// <summary>
            /// 己方全部
            /// </summary>
            AllInTeam,

            /// <summary>
            /// 敌方全部
            /// </summary>
            AllOfEnemy,

            /// <summary>
            /// 场上所有角色卡
            /// </summary>
            AllOnSpot,

            /// <summary>
            /// （己方）发动者的上一位
            /// </summary>
            Next,

            /// <summary>
            /// （己方）发动者的下一位
            /// </summary>
            Last,
            
            /// <summary>
            /// 召唤
            /// </summary>
            Summon,
            
        }

        /// <summary>
        /// 确定事件执行对象用的额外条件
        /// </summary>
        public enum ExtraCondition
        {
            None,
            CardName,
            CharacterName,
            Gender,
            CV,
            Summon,
        }

        /// <summary>
        /// 要修改的参数
        /// </summary>
        public enum ParameterToChange
        {
            None,

            Coin,

            Power,

            HealthPoint,

            Silence,

            State,
            
            Summon,
        }

        /// <summary>
        /// 角色卡能力类型
        /// </summary>
        public enum CardAbilityTypes
        {
            /// <summary>
            /// 没有能力
            /// </summary>
            None,

            /// <summary>
            /// 每次轮到此卡均会出发
            /// </summary>
            Normal,

            /// <summary>
            /// 登场。每次战斗中第一次轮到此卡牌时触发
            /// </summary>
            Debut,

            /// <summary>
            /// 退场（亡语），每次战斗中此卡被击败时发动
            /// </summary>
            Exit,
        }


        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum GameState
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
        /// 角色卡状态
        /// </summary>
        public enum CardState
        {
            //默认的，没有在手牌、场上、部长位，可招募卡牌中出现
            None,

            /// <summary>
            /// 在可招募卡牌中出现
            /// </summary>
            Available,

            /// <summary>
            /// 是手牌
            /// </summary>
            Hand,

            /// <summary>
            /// 登场出战
            /// </summary>
            Present,

            /// <summary>
            /// 部长位
            /// </summary>
            Chief,
        }

        /// <summary>
        /// 所属动画（阵营，社团）
        /// </summary>
        public enum Anime
        {
            通用,
            全金属狂潮,
            AIR,
            凉宫春日的忧郁,
            Kanon,
            幸运星,
            CLANNAD,
            轻音少女,
            冰菓,
            中二病也要谈恋爱,
            玉子市场,
            Free,
            境界的彼方,
            甘城光辉游乐园,
            吹响吧上低音号,
            无彩限的怪灵世界,
            小林家的龙女仆,
            紫罗兰永恒花园,
            弦音风舞高中弓道部,
            巴加的工作室,
        }

        /// <summary>
        /// 声优（按照萌娘百科中人物照片上方的中文译名记）
        /// </summary>
        public enum CV
        {
            None,
            Else,
            /// <summary>
            /// 丰崎爱生
            /// </summary>
            ToyosakiAki,
            /// <summary>
            /// 茅原实里
            /// </summary>
            ChiharaMinori,
            种田梨沙,
            铃木达央,
            KENN,
            进藤尚美,
            山冈百合,
            丰田萌绘,
            渡边明乃,
            川澄绫子,
            今野宏美,
            平野绫,
            杉田智和,
            后藤邑子,
            小野大辅,
            松冈由贵,
            桑谷夏子,
            白石稔,
            松元惠,
            青木沙耶香,
            加藤英美里,
            福原香织,
            古谷静佳,
            中村悠一,
            佐藤聪美,
            阪口大助,
            茅野爱衣,
            日笠阳子,
            早见沙织,
            小仓唯,
            悠木碧,
            竹达彩奈,
            寿美菜子,
            福山润,
            内田真礼,
            长妻树里,
            赤崎千夏,
            保志总一朗,
            设乐麻美,
            浅仓杏美,
            上坂堇,
            仙台惠理,
            洲崎绫,
            田丸笃志,
            金子有希,
            黑泽朋世,
            安济知佳,
            朝井彩加,
            石谷春贵,
            藤村鼓乃美,
            种崎敦美,
            东山奈央,
            樱井孝宏,
            沼仓爱美,
            田村睦心,
            桑原由气,
            长绳麻理亚,
            高田忧希,
            高桥未奈美,
            岭内知美,
            石原夏织,
            中原麻衣,
            野中蓝,
            田所梓,
            金元寿子,
            川上伦子,
            冈本麻见,
            柚木凉香,
            久川绫,
            冬马由美,
            田村由香里,
            西村千奈美,
            斋藤千和,
            兴梠里美,
            置鲇龙太郎,
            井上喜久子,
            广桥凉,
            神田朱未,
            桑岛法子,
            雪野五月,
            榎本温子,
            麻生美代子,
            青木静香,
            小伏伸之,
            
        }


        public enum CharacterName
        {
            通用,
            //凉宫
            凉宫春日,
            阿虚,
            长门有希,
            朝比奈实玖瑠,
            古泉一树,
            朝仓凉子,
            鹤屋,
            喜绿江美里,
            国木田,
            电脑研究社社长,
            虚妹,
            //幸运星
            泉此方,
            柊镜,
            柊司,
            高良美幸,
        }
    }
}
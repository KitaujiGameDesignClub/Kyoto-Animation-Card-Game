using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace Core
{
    [System.Serializable]
    public struct AbilityLogicReason
    {
        /// <summary>
        /// 条件对象
        /// </summary>
        public Information.Objects ReasonObject;

        /// <summary>
        /// 确定对象用的额外条件
        /// </summary>
        public Information.ExtraCondition ReasonExtraCondition;


        /// <summary>
        /// 判断的参数
        /// </summary>
        public Information.Parameter ReasonParameter;

        /// <summary>
        /// 判断方法
        /// </summary>
        public Information.JudgeMethod ReasonJudgeMethod;

        /// <summary>
        /// 原因判断逻辑 | -2 < | -1 <= | 0 == | 1 >= | 2 > |
        /// </summary>
        /// <returns></returns>
        public int Logic;
        
        /// <summary>
        /// 阈值
        /// </summary>
        public string Threshold;
        
       


    }
    
    [System.Serializable]
    public struct AbilityLogicResult
    {
        /// <summary>
        /// true=对满足阈值条件的卡牌发动效果，忽略下方新一轮的对象筛选（结果对象筛选）
        /// </summary>
        public bool RevisePassedReasonObjects;
        
        //新一轮的对象筛选（筛选出结果对象）
        
        /// <summary>
        /// 结果对象
        /// </summary>
        public Information.Objects ResultObject;

        /// <summary>
        /// 确定结果对象用的额外条件
        /// </summary>
        public Information.ExtraCondition ReasonExtraCondition;
        
        
        /// <summary>
        /// 结果对象要修改的参数
        /// </summary>
        public Information.Parameter ParameterToChange;

        /// <summary>
        /// 结果对象参数的修改方法
        /// </summary>
        public Information.CalculationMethod CalculationMethod;

        /// <summary>
        /// 修改的值。如何计算按照CalculationMethod来
        /// </summary>
        public string Value;
    }

    public class Information
    {
        /// <summary>
        /// 对象检索范围
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
            
        }

        /// <summary>
        /// 确定对象用的额外条件
        /// </summary>
        public enum ExtraCondition
        {
            None,
            CardName,
            CharacterName,
            Gender,
            CV,
         
        }

        /// <summary>
        /// 判断或修改的参数
        /// </summary>
        public enum Parameter
        {
            None,
            CardCount,
            Coin,
            Power,
            HealthPoint,
            Silence,
            State,
            CV,
            CharacterName,
            CardName,
        }

        /// <summary>
        /// 判断方法
        /// </summary>
        public enum JudgeMethod
        {
            /// <summary>
            /// 取值
            /// </summary>
            Value,
            /// <summary>
            /// 计数
            /// </summary>
            Count,
        }

        /// <summary>
        /// 对结果参数的计算方法
        /// </summary>
        public enum CalculationMethod
        {
           /// <summary>
           /// 加法
           /// </summary>
            addition,
           /// <summary>
           /// 减法
           /// </summary>
            subtraction, 
           /// <summary>
           /// 乘法
           /// </summary>
            multiplication,
           /// <summary>
           /// 除法
           /// </summary>
            division,
           /// <summary>
           /// 设定为某个值
           /// </summary>
           ChangeTo,
            
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
            /// <summary>
            /// 通用或不重要
            /// </summary>
            Universal,
            /// <summary>
            /// 全金属狂潮
            /// </summary>
            FullMetalPanic,
            AIR,
            /// <summary>
            /// 凉宫春日的忧郁
            /// </summary>
            TheMelancholyOfHaruhiSuzumiya,
            Kanon,
            /// <summary>
            /// 幸运星
            /// </summary>
            LuckyStar,
            CLANNAD,
            /// <summary>
            /// 轻音少女
            /// </summary>
            Kon,
            /// <summary>
            /// 冰菓
            /// </summary>
            Hyouka,
            /// <summary>
            /// 中二病也要谈恋爱
            /// </summary>
            LoveChunibyoAndOtherDelusions,
            /// <summary>
            /// 玉子市场
            /// </summary>
            TamakoMarket,
            Free,
            /// <summary>
            /// 境界的彼方
            /// </summary>
            BeyondTheBoundary,
            /// <summary>
            /// 甘城光辉游乐园
            /// </summary>
            AmagiBrilliantPark,
            /// <summary>
            /// 吹响吧!上低音号
            /// </summary>
            SoundEuphonium,
            /// <summary>
            /// 无彩限的怪灵世界
            /// </summary>
            MyriadColorsPhantomWorld,
            /// <summary>
            /// 小林家的龙女仆
            /// </summary>
            MissKobayashiDragonMaid,
            /// <summary>
            /// 紫罗兰永恒花园
            /// </summary>
            VioletEvergarden,
            /// <summary>
            /// 弦音风舞高中弓道部
            /// </summary>
            KazemaiKoukouKyuudoubu,
            /// <summary>
            /// 巴加的工作室
            /// </summary>
            BajaStudio,
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
            /// <summary>
            /// 种田梨沙
            /// </summary>
            TanedaRisa,
            /// <summary>
            /// 铃木达央
            /// </summary>
            SuzukiTatsuhisa,
            KENN,
            /// <summary>
            /// 进藤尚美
            /// </summary>
            ShindoNaomi,
            /// <summary>
            /// 山冈百合
            /// </summary>
            YamaokaYuri,
            /// <summary>
            /// 丰田萌绘
            /// </summary>
            ToyotaMoe,
            /// <summary>
            /// 渡边明乃
            /// </summary>
            WatanabeAkeno,
            /// <summary>
            /// 川澄绫子
            /// </summary>
            KawasumiAyako,
            /// <summary>
            /// 今野宏美
            /// </summary>
            KonnoHiromi,
            /// <summary>
            /// 平野绫
            /// </summary>
            HiranoAya,
            /// <summary>
            /// 杉田智和
            /// </summary>
            SugidaTomokazu,
            /// <summary>
            /// 后藤邑子
            /// </summary>
            GotouYuuko,
            /// <summary>
            /// 小野大辅
            /// </summary>
            OnoDaisuke,
            /// <summary>
            /// 松冈由贵
            /// </summary>
            MatsuokaYuki,
            /// <summary>
            /// 桑谷夏子
            /// </summary>
            KuwataniNatsuko,
            /// <summary>
            /// 白石稔
            /// </summary>
            ShiraishiMInoru,
            /// <summary>
            /// 松元惠
            /// </summary>
            MatsumotoMegumi,
            /// <summary>
            /// 青木沙耶香
            /// </summary>
            AokiSayaka,
            /// <summary>
            /// 加藤英美里
            /// </summary>
            KatouEmiri,
            /// <summary>
            /// 福原香织
            /// </summary>
            HukuharaKaori,
            /// <summary>
            /// 古谷静佳
            /// </summary>
            HuruyaShizuka,
            /// <summary>
            /// 中村悠一
            /// </summary>
            NakamuraYuuichi,
            /// <summary>
            /// 佐藤聪美
            /// </summary>
            SatouSatomi,
            /// <summary>
            /// 阪口大助
            /// </summary>
            SakaguchiDaisuke,
            /// <summary>
            /// 茅野爱衣
            /// </summary>
            KayanoAi,
            /// <summary>
            /// 日笠阳子
            /// </summary>
            HikasaYouko,
            /// <summary>
            /// 早见沙织
            /// </summary>
            HayamiSaori,
            /// <summary>
            /// 小仓唯
            /// </summary>
            OguraYui,
            /// <summary>
            /// 悠木碧
            /// </summary>
            YuukiAoi,
            /// <summary>
            /// 竹达彩奈
            /// </summary>
            TaketatsuAyana,
            /// <summary>
            /// 寿美菜子
            /// </summary>
            KotobukiMinako,
            /// <summary>
            /// 福山润
            /// </summary>
            HukuyamaJun,
            /// <summary>
            /// 内田真礼
            /// </summary>
            UchitdaMaaya,
            /// <summary>
            /// 长妻树里
            /// </summary>
            NagatsumaJuri,
            /// <summary>
            /// 赤崎千夏
            /// </summary>
            AkasakiChinatsu,
            /// <summary>
            /// 保志总一朗
            /// </summary>
            HoshiSouichitou,
            /// <summary>
            /// 设乐麻美
            /// </summary>
            ShitaraMami,
            /// <summary>
            /// 浅仓杏美
            /// </summary>
            AsakuraAzumi,
            /// <summary>
            /// 上坂堇
            /// </summary>
            UesakaSumire,
            /// <summary>
            /// 仙台惠理
            /// </summary>
            SendaiEri,
            /// <summary>
            /// 洲崎绫
            /// </summary>
            SuzakiAya,
            /// <summary>
            /// 田丸笃志
            /// </summary>
            TamaruAtsushi,
            /// <summary>
            /// 金子有希
            /// </summary>
            KanekoYuuki,
            /// <summary>
            /// 黑泽朋世
            /// </summary>
            KurosawaTomoyo,
            /// <summary>
            /// 安济知佳
            /// </summary>
            AnzaiChika,
            /// <summary>
            /// 朝井彩加
            /// </summary>
            AsaiAyaka,
            /// <summary>
            /// 石谷春贵
            /// </summary>
            IshiyaHaruki,
            /// <summary>
            /// 藤村鼓乃美
            /// </summary>
            HujimuraKonomi,
            /// <summary>
            /// 种崎敦美
            /// </summary>
            TanezakiAtsumi,
            /// <summary>
            /// 东山奈央
            /// </summary>
            TouyamaNao,
            /// <summary>
            /// 樱井孝宏
            /// </summary>
            SakuraiTakahiro,
            /// <summary>
            /// 沼仓爱美
            /// </summary>
            NumakuraManami,
            /// <summary>
            /// 田村睦心
            /// </summary>
            TamuraMutsumi,
            /// <summary>
            /// 桑原由气
            /// </summary>
            KuwaharaYuuki,
            /// <summary>
            /// 长绳麻理亚
            /// </summary>
            NaganawaMaria,
            /// <summary>
            /// 高田忧希
            /// </summary>
            TakadaYuuki,
            /// <summary>
            /// 高桥未奈美
            /// </summary>
            TakahashiMinami,
            /// <summary>
            /// 岭内知美
            /// </summary>
            MineuchiTomomi,
            /// <summary>
            /// 石原夏织
            /// </summary>
            IshiharaKaori,
            /// <summary>
            /// 中原麻衣
            /// </summary>
            NakaharaMai,
            /// <summary>
            /// 野中蓝
            /// </summary>
            NonakaAi,
            /// <summary>
            /// 田所梓
            /// </summary>
            TadokoroAzusa,
            /// <summary>
            /// 金元寿子
            /// </summary>
            KanemotoHisako,
            /// <summary>
            /// 川上伦子
            /// </summary>
            KawakamiTomoko,
            /// <summary>
            /// 冈本麻见
            /// </summary>
            OkamotoAsami,
            /// <summary>
            /// 柚木凉香
            /// </summary>
            YuzukiRyouka,
            /// <summary>
            /// 久川绫
            /// </summary>
            HisakawaAya,
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

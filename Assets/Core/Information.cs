
namespace Core
{

    
    
    /// <summary>
    /// 角色卡能力的触发原因（简称能力原因）
    /// </summary>
    [System.Serializable]
    public struct AbilityLogicReason
    {


        /// <summary>
        /// 确定条件对象
        /// </summary>
        public NeededObjects NeededObjects;

        
        //下面这些参数，并不是为了再次缩小条件对象的范围，而是在上面求出来的范围中，设置触发能力的条件
        
        /// <summary>
        /// 可以对确定的条件对象的参数进行判断，判断结果为真（或参数设置为None），会触发能力效果
        /// </summary>
        public Information.Parameter ReasonParameter;
        
        /// <summary>
        /// 如何进行参数判断，对值判断亦或是对数量判断
        /// </summary>
        public Information.JudgeMethod ReasonJudgeMethod;
        
        /// <summary>
        /// 参数判断逻辑 （-3 不包含/不等于）( -2 小于 ) ( -1 小于等于 )(  0 等于/包含 )( 1 大于等于 ) ( 2 大于) 
        /// </summary>
        /// <returns></returns>
        public int Logic;
        
        /// <summary>
        /// 参数判断阈值
        /// </summary>
        public string Threshold;
      
        

    }
    
    /// <summary>
    /// 角色卡能力触发的效果（简称能力效果）
    /// </summary>
    [System.Serializable]
    public struct AbilityLogicResult
    {
        /// <summary>
        /// true=对触发此能力的卡牌发动效果，忽略下方新一轮的对象筛选（结果对象筛选）
        /// </summary>
        public bool RegardActivatorAsResultObject;

        /// <summary>
        /// 召唤一个符合CardName的卡牌（空则不召唤）
        /// </summary>
        public string SummonCardName;

        /// <summary>
        /// 嘲讽（回合数用下面的value和配套算法来计算）
        /// </summary>
        public bool Ridicule;
        
        //新一轮的对象筛选（筛选出结果对象）

        /// <summary>
        /// 结果对象
        /// </summary>
        public NeededObjects ResultObject;

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

    /// <summary>
    /// 需要对象（最终对这些对象的参数进行判定或者是修改）
    /// </summary>
    [System.Serializable]
    public struct NeededObjects
    {
        /// <summary>
        /// 确定所需对象的大范围 。chief的话，就不进行后续处理了
        /// </summary>
        public Information.Objects LargeScope;
        /// <summary>
        /// 用于缩小范围以确定对象的参数 None时，直接把需要的对象定义为大范围内的所有对象，不进行后续处理
        /// </summary>
        public Information.Parameter ParameterToShrinkScope;

        /// <summary>
        /// 参数判断逻辑 （-3 不包含/不等于）( -2 小于 ) ( -1 小于等于 )(  0 等于/包含 )( 1 大于等于 ) ( 2 大于) 
        /// </summary>
        /// <returns></returns>
        public int Logic;
        
        /// <summary>
        /// 参数判断阈值
        /// </summary>
        public string Threshold;
    }

    /// <summary>
    /// 角色羁绊
    /// </summary>
    [System.Serializable]
    public struct CharactersConnect
    {
        
        /*
         * 羁绊类型：最终用于确定羁绊的效果。
         * 羁绊层（ConnectLayer）；
         *  例子如下：
         *  折木奉太郎（Love[Male]），LowPower折木（Love[Male]），千反田爱馏（Love[Female]），黄喉偶人（Love[Female]），伊原摩耶花（Love）
         *  当己方场上存在 折木奉太郎和LowPower折木时，不会触发羁绊效果。因为羁绊层相同，[标记内容]也相同；
         *  当己方场上存在 折木奉太郎和千反田爱馏时，会触发羁绊效果。因为羁绊层相同，但是[标记内容]不同；
         *  当己方场上存在 千反田爱馏和伊原摩耶花时，会触发羁绊效果，因为没有标记内容的”伊原摩耶花“可以和任何羁绊层为”Love“的角色卡产生羁绊。
         */
        
        /// <summary>
        /// 羁绊类型
        /// </summary>
        public Information.ConnectTypes ConnectType;
        /// <summary>
        /// 同一种羁绊类型中，只有在相同层上的，才可以激活。可以加入额外标记：[标记内容]：标记内容不同的卡牌之间才能够激活羁绊。
        /// </summary>
        public string ConnectLayer;
        
    }
    
    public class Information
    {
        #region 各种版本号信息

        //a.b:a修改后，说明不兼容之前的旧版本，需要升级旧版文件
        //b修改后，是小修改，可以兼容
        
        public const string  CardMakerVersion = "1.0";
        public const string CharacterCardMaker = "1.0";
        public const string ManifestVersion = "1.0";

        #endregion
        
        /// <summary>
        /// 对象检索范围
        /// </summary>
        public enum Objects
        {
            /// <summary>
            /// 不设定范围
            /// </summary>
            None,
            
            /// <summary>
            /// 任何情况下都会可以 ，不进行后续判断，直接运行Result所定义的能力，且RegardActivatorAsResultObject=false
            /// </summary>
            Any,
            /// <summary>
            /// 发动者自身
            /// </summary>
            Self,

            /// <summary>
            /// 场上所有卡牌
            /// </summary>
            AllOnSpot,
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
            /// （己方）发动者的上一位
            /// </summary>
            Next,

            /// <summary>
            /// （己方）发动者的下一位
            /// </summary>
            Last,
            
            /// <summary>
            /// 成功触发能力的那个角色卡（现在就用于判断是谁打了我）
            /// </summary>
            Activator,
            
            /// <summary>
            /// 对方的chief
            /// </summary>
            ChiefOfEnemy,
            
            /// <summary>
            /// 己方的chief
            /// </summary>
            OurChief,
        }

        /// <summary>
        /// 判断或修改的参数（Objects为Any时，忽视此类型）
        /// </summary>
        public enum Parameter
        {
            None,
            
            Tag,
            Power,
            HealthPoint,
            Silence,
            Ridicule,
            State,
            CV,
            /// <summary>
            /// 角色名字
            /// </summary>
            CharacterName,
            Coin,
            
        }

        /// <summary>
        /// 判断方法
        /// </summary>
        public enum JudgeMethod
        {
            /// <summary>
            /// 取值（获取参数的值or内容）
            /// </summary>
            Value,
            /// <summary>
            /// 计数（获得参数的数量，仅适用于CV、CharacterName等定性的、非布尔类型）
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
           /// 乘法
           /// </summary>
            multiplication,
        
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
            
            /// <summary>
            /// 自己受伤时触发
            /// </summary>
            GetHurt,
        }


        /// <summary>
        /// 角色名称
        /// </summary>
        public enum CharacterName
        {
            /// <summary>
            /// 没有角色设定或不重要
            /// </summary>
            None,
            
            //冰菓
            /// <summary>
            /// 折木奉太郎
            /// </summary>
            OrekiHoutarou,
            /// <summary>
            /// 千反田爱馏
            /// </summary>
            ChitandaEru,
            
            //Kon
            /// <summary>
            /// 平泽唯
            /// </summary>
            HirasawaYui,
            
            //Hibike!Euphonium
            
            
            //中二病
            
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
        /// 角色卡羁绊类别
        /// </summary>
        public enum ConnectTypes
        {
            /// <summary>
            /// 无任何羁绊或不必要
            /// </summary>
            None,
            /// <summary>
            /// 恋人
            /// </summary>
            Lovers,
            /// <summary>
            /// 挚友
            /// </summary>
            BestFriends,
            /// <summary>
            /// 竞争对手
            /// </summary>
            Competitor,
            /// <summary>
            /// 兄弟姐妹
            /// </summary>
            BroOrSis,

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
        /// 所属动画（卡包，阵营，社团）
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
            /// <summary>
            /// 没有使用声优或者声优不重要
            /// </summary>
            None,
            /// <summary>
            /// 列表中不存在的声优
            /// </summary>
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
            /// <summary>
            /// 冬马由美
            /// </summary>
            TomaYumi,
            /// <summary>
            /// 田村由香里
            /// </summary>
            TamuraYukari,
            /// <summary>
            /// 西村千奈美
            /// </summary>
            NishimuraChinami,
            /// <summary>
            /// 斋藤千和
            /// </summary>
            SaitouChiwa,
            /// <summary>
            /// 兴梠里美
            /// </summary>
            KoorogiSatomi,
            /// <summary>
            /// 置鲇龙太郎
            /// </summary>
            OkiayuRyoutarou,
            /// <summary>
            /// 井上喜久子
            /// </summary>
            KumagaiKikuko,
            /// <summary>
            /// 广桥凉
            /// </summary>
            HirohashiRyou,
            /// <summary>
            /// 神田朱未
            /// </summary>
            KandaAkemi,
            /// <summary>
            /// 桑岛法子
            /// </summary>
            KuwashimaHoko,
            /// <summary>
            /// 雪野五月
            /// </summary>
            YukinoSatsuki,
            /// <summary>
            /// 榎本温子
            /// </summary>
            EnomotoAtsuko,
            /// <summary>
            /// 麻生美代子
            /// </summary>
            AsouMiyoko,
            /// <summary>
            /// 小伏伸之
            /// </summary>
            KobushiNobuyuk,
            
        }

/// <summary>
/// 玩家tag（预设的，玩家可以自定义）
/// </summary>
        public enum CharacterTag
        {
            /// <summary>
            /// 无特殊标签
            /// </summary>
            None,
            /// <summary>
            /// 隶属于SOS
            /// </summary>
            SOS,
            /// <summary>
            /// 属于轻音部
            /// </summary>
            Kon,
            
            /// <summary>
            /// 吹奏乐部
            /// </summary>
            Suisogakubu,
           
            /// <summary>
            /// 北宇治的学生
            /// </summary>
            Kitauji,
            /// <summary>
            /// 恐怖的
            /// </summary>
            Horrifying,
            /// <summary>
            /// 胆小的
            /// </summary>
            Coward,
            
            
            
        }
    }
}

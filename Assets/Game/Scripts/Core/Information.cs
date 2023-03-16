using KitaujiGameDesignClub.GameFramework;
using Maker;

namespace Core
{

    #region 能力与羁绊

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
        public Information.Parameter JudgeParameter;

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
        public Information.CalculationMethod ChangeMethod;

        /// <summary>
        /// 修改的值。如何计算按照CalculationMethod来
        /// </summary>
        public string Value;

        /// <summary>
        /// 召唤一个符合CardName的卡牌（空则不召唤）
        /// </summary>
        public string SummonCardName;

        /// <summary>
        /// 嘲讽回合数（加法，>1时，就只会攻击嘲讽他的那位）
        /// </summary>
        public int Ridicule;

        /// <summary>
        /// 嘲讽回合数（加法）
        /// </summary>
        public int Silence;
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
        // public Information.ConnectTypes ConnectType;
        /// <summary>
        /// 同一种羁绊类型中，只有在相同层上的，才可以激活。可以加入额外标记：[标记内容]：标记内容不同的卡牌之间才能够激活羁绊。
        /// </summary>
        //  public string ConnectLayer;

    }

    #endregion

    public class Information
    {
        /// <summary>
        /// 每一组最大的卡牌数（上场的）
        /// </summary>
        public const int TeamMaxCardOnSpotCount = 6;

        #region 各种版本号信息

        //a.b:a修改后，说明不兼容之前的旧版本，需要升级旧版文件
        //b修改后，是小修改，可以兼容
        public const int CharacterCardMaker = 1;
        public const int ManifestVersion = 1;

        public const string AnimeListVersion = "ver 1.0-京阿尼更新";
        public const string TagsVersion = "ver 1.0-京阿尼更新";
        public const string CharacterVoiceVersion = "ver 1.0-京阿尼更新";
        public const string CharacterVersion = "ver 1.0-京阿尼更新";

        #endregion


        #region 内置路径与文件拓展名规范和字典预设

        public const string ManifestFileName = "manifest.yml";
        public const string CardFileName = "card.yml";
        public const string DefaultCoverNameWithoutExtension = "cover";
        public static readonly string[] SupportedImageExtension = { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };
        public static readonly string[] SupportedAudioExtension = { ".mp3", ".ogg", ".wav", ".aif" };


        /// <summary>
        /// 游戏会在这里读取卡组
        /// </summary>
        public static string bundlesPath = $"{YamlReadWrite.UnityButNotAssets}/bundles";

        public static readonly DescribeFileIO AnimeListIO = new DescribeFileIO("animeList.yml", "saves", "# 此文件包含了动漫列表，用于规范卡组中所属动画的文本" +
    "\n# 此文件不会影响正常游戏，但是卡组编辑器“所属动画”一栏中的可选内容会受到此文件的影响（便于卡组内卡牌的互动，以及卡组间卡牌的互动）" +
    "\n# 为便于后期维护，如果要添加自定义词条，请在默认内容之后添加（词条后方可以加“#”来写注释）。可以加入空行" +
    $"\n# 此文件的默认内容由本游戏的“{AnimeListVersion}”版本呈现");


        public static readonly DescribeFileIO TagsIO = new DescribeFileIO("tags.yml", "saves", "# 此文件包含了角色卡牌可以使用的tag，用于规范tag文本" +
            "\n# 此文件不会影响正常游戏，但是卡牌编辑器“角色标签”一栏中的可选内容会受到此文件的影响（便于卡组内卡牌的互动，以及卡组间卡牌的互动）" +
            "\n# 为便于后期维护，如果要添加自定义词条，请在默认内容之后添加（词条后方可以加“#”来写注释）。可以加入空行" +
            "\n# 分类标记要用“%”开头，含有此标记的字符不会作为角色tag" +
            $"\n# 此文件的默认内容由本游戏的“{TagsVersion}”版本呈现");

        public static readonly DescribeFileIO cvIO = new DescribeFileIO("cv.yml", "saves", "# 此文件包含了角色卡牌可以使用的声优名称，用于规范声优翻译" +
            "\n# 此文件不会影响正常游戏，但是卡牌编辑器“角色声优”一栏中的可选内容会受到此文件的影响（便于卡组内卡牌的互动，以及卡组间卡牌的互动）" +
            "\n# 为便于后期维护，如果要添加自定义词条，请在默认内容之后添加（词条后方可以加“#”来写注释）。可以加入空行" +
            $"\n# 此文件的默认内容由本游戏的“{CharacterVoiceVersion}”版本呈现");

        public static readonly DescribeFileIO characterNameIO = new DescribeFileIO("characters.yml", "saves", "# 此文件包含了角色卡牌可以使用的角色名称，用于规范声优翻译" +
           "\n# 此文件不会影响正常游戏，但是卡牌编辑器“角色名称”一栏中的可选内容会受到此文件的影响（便于卡组内卡牌的互动，以及卡组间卡牌的互动）" +
           "\n# 为便于后期维护，如果要添加自定义词条，请在默认内容之后添加（词条后方可以加“#”来写注释）。可以加入空行" +
           "\n# 分类标记要用“%”开头，含有此标记的字符不会作为角色名称" +
           $"\n# 此文件的默认内容由本游戏的“{CharacterVersion}”版本呈现");

        public enum DictionaryType
        {
            animeList,
            cv,
            characters,
            tags,

        }

        #endregion


        #region 名称字典
        public static string[] tags =
   {

            //尽量按照字符数量多少排序，然后类似类别的放在一起

            "%所属组织",
            "HTT",
            "SOS团",
            "古典部",
            "舞棒部",
             "吹奏乐部",
             "极东魔术昼寝结社之夏",

            "%活动场所",
            "北宇治",
            "樱丘高中",
            "神山高校",
            "县立北高",
            "银杏学园高中",
            "市立南中学校",

            "%担任职业",
            "IT",
            "老师",
            "音乐家",
             "无职业",
            "学生，高中生",
            "学生，初中生",
            "学生，大学生",
            "学生，社团部长",
            "学生，社团部员",
            "学生，学生会长",



            "%特殊分类",
            "普通人",
            "外星人",
            "未来人",
            "异世界人",
            "超能力者",
            "凉宫春日希望的",


            "%性格",
             "萌",
             "中二",
             "节能",
             "傲娇",
             "腹黑",
             "严格",
             "和蔼",
             "鲁莽",
             "元气",
             "冒失",
             "慵懒",
             "胆小",
             "姐控兄控",
             "天然呆",

             "%服饰",
             "眼镜",
             "水手服",
             "丝带",
             "兔女郎",
             "连裤袜",


             "%外貌",
             "泪痣",
             "猫耳",
             "马尾",
              "短发",
             "长发",
             "长发，黑长直",
             "京都脸",
             "亮额头",
             "腌萝卜",
             


            "%特长/能力",
            "唱歌",
            "乐器，吉他",
            "乐器，贝斯",
            "乐器，键盘",
            "乐器，短笛",
             "乐器，长笛",
             "乐器，小号",
             "乐器，大号",
             "乐器，长号",
             "乐器，圆号",
             "乐器，架子鼓",
             "乐器，单簧管",
             "乐器，双簧管",
             "乐器，打击乐",
             "乐器，上低音号",
             "乐器，低音提琴",
             "乐器，萨克斯，上低音萨克斯",
            "天才",
            "吐槽",
            "左撇子",



            "%感情",
            "有男女朋友",
            "有挚友",
            "单身",
            "百合",
            "已婚",
            "丧偶",

            "%物体类别",
            "食物",
            "乐器（物体）",


            "%场所泛称（地点限定）",
            "教室",
            "操场",
            "广场",
            "宿舍",
            "家",
            "工作室/公司",

            "%场所特点（地点限定）",
            "神秘",
            "恐怖",
            "音乐用",
            "体育活动用",
            "画画用",
            "动漫制作公司",

        };


        public static string[] AnimeList =
        {
            "通用卡牌",
            "全金属狂潮第二季（校园篇）",
            "AIR",
            "凉宫春日的忧郁",
            "Kanon",
            "幸运☆星",
            "CLANNAD",
            "轻音少女",
            "冰菓",
            "中二病也要谈恋爱！",
            "玉子市场",
            "free!",
            "境界的彼方",
            "甘城光辉游乐园",
            "吹响吧!上低音号",
            "无彩限的怪灵世界",
            "小林家的龙女仆",
            "紫罗兰永恒花园",
            "弦音 -风舞高中弓道部-",
            "巴加的工作室",
        };


        public static string[] characterNamesList =
              {
    "通用卡牌",

"%全金属狂潮第二季（校园篇）",

"%AIR",
"国崎往人",
"神尾观铃",
"雾岛佳乃",
"远野美凪",
"神尾晴子",
"雾岛圣",
"小满",

"%凉宫春日的忧郁",
"凉宫春日",
"阿虚",
"长门有希",
"朝比奈实玖瑠",
"古泉一树",
"朝仓凉子",
"谷口",
"鹤屋",

"%Kanon",
"相泽佑一",
"月宫亚由",
"水濑名雪",
"泽渡真琴",
"美坂栞",
"川澄舞",
"仓田佐祐里",
"水濑秋子",
"美坂香里",
"天野美汐",

"%幸运☆星",
"泉此方",
"柊镜",
"柊司",
"高良美幸",
"成实唯",
"小早川优",
"黑井奈奈子",
"岩崎南",
"日下部美纱绪",
"峰岸绫乃",
"小神晶",
"白石稔",

"%CLANNAD",
"冈崎朋也",
"古河渚",
"冈崎汐",
"古河秋生",
"古河早苗",
"藤林杏",
"藤林椋",
"一之濑琴美",
"坂上智代",
"伊吹风子",
"春原阳平",
"相乐美佐枝",
"宫泽有纪宁",
"牡丹",
"志麻贺津纪",
"幸村俊夫",
"春原芽衣",
"芳野祐介",
"伊吹公子",
"冈崎直幸",

"%轻音少女",
"平泽唯",
"山中佐和子",
"秋山澪",
"琴吹紬",
"田井中律",
"中野梓",
"平泽优",
"真锅和",
"铃木纯",
"曾我部惠",
"田井中聪",

"%冰菓",
"千反田爱瑠",
"折木奉太郎",
"伊原摩耶花",
"福部里志",
"折木供惠",
"入须冬实",

"%日常",
"东云名乃",
"博士",
"阪本先生",
"相生佑子",
"长野原美绪",
"水上麻衣",
"笹原幸治郎",
"立花美里",
"中之条刚",

"%中二病也要谈恋爱！",
"小鸟游六花",
"富樫勇太",
"七宫智音",
"丹生谷森夏",
"一色诚",
"富樫樟叶",
"富樫梦叶",
"五月七日茴香",
"凸守早苗",
"小鸟游十花",
"奇美拉",

"%玉子市场",
"北白川玉子",
"大路饼藏",
"常盘绿",
"牧野神奈",
"朝雾史织",
"迪拉·莫奇马兹",
"乔伊·莫奇马兹",
"北白川馅子",
"北白川雏子",
"北白川豆大",
"北白川福",
"大路吾平",
"大路道子",
"花濑熏",

"%free!",

"%境界的彼方",
"栗山未来",
"神原秋人",
"名濑美月",
"名濑博臣",
"新堂爱",
"新堂彩华",
"二之宫雫",
"伊波樱",
"名濑泉",
"神原弥生",

"%甘城光辉游乐园",
"可儿江西也",
"千斗五十铃",
"拉媞珐·芙尔兰扎",
"松松饼",
"马卡龙",
"堤拉米",
"缪斯",
"沙罗曼",
"席尔菲",
"珂玻莉",
"安达映子",
"中城椎菜",

"%吹响吧!上低音号",
"黄前久美子",
"泷昇",
"高坂丽奈",
"川岛绿辉",
"加藤叶月",
"冢本秀一",
"小笠原晴香",
"田中明日香",
"中世古香织",
"吉川优子",
"中川夏纪",
"铠冢霙",
"伞木希美",
"长濑梨子",
"后藤卓也",
"斋藤葵",
"松本美知惠",
"桥本真博",
"新山聪美",
"黄前麻美子",
"田边名来",
"堺万纱子",
"井上顺菜",
"加部友惠",
"剑崎梨梨花",
"久石奏",
"铃木美玲",
"铃木五月",
"月永求",
"佐佐木梓",

"%无彩限的怪灵世界",

"%小林家的龙女仆",
"小林",
"托尔",
"康娜卡姆依",
"伊露露",
"艾尔玛",
"露科亚",
"法夫纳",
"泷谷真",
"才川莉子",
"才川乔吉",
"真土翔太",
"会田竹人",
"终焉帝",

"%紫罗兰永恒花园",
"薇尔莉特·伊芙加登",
"克劳迪亚·霍金斯",
"吉尔伯特·布干维利亚",
"嘉德丽雅·波德莱尔",
"贝内迪克特·布卢",
"迪特福利特·布干维利亚",
"奥斯卡·韦伯斯特",
"奥莉薇娅·韦伯斯特",
"安妮·马格诺利亚",
"艾莉卡·布朗",
"爱丽丝·加那利",

"%弦音 -风舞高中弓道部-",
"鸣宫凑",
"竹早静弥",
"山之内辽平",
"小野木海斗",
"如月七绪",
"泷川雅贵",
"森冈富男",
"白菊乃爱",
"妹尾梨可",
"花泽悠奈",
"藤原愁",

"%二十世纪电气目录",

"%巴加的工作室"

        };


        /// <summary>
        /// 声优（按照萌娘百科中人物照片上方的中文译名记）（用于储存文件）
        /// </summary>
        public static string[] CV =
        {

"不设置声优",
"丰崎爱生",
"真田麻美",
"茅原实里",
"种田梨沙",
"铃木达央",
"进藤尚美",
"山冈百合",
"丰田萌绘",
"渡边明乃",
"川澄绫子",
"今野宏美",
"平野绫",
"杉田智和",
"后藤邑子",
"小野大辅",
"松冈由贵",
"桑谷夏子",
"白石稔",
"松元惠",
"青木沙耶香",
"加藤英美里",
"福原香织",
"古谷静佳",
"中村悠一",
"佐藤聪美",
"阪口大助",
"茅野爱衣",
"日笠阳子",
"早见沙织",
"小仓唯",
"悠木碧",
"竹达彩奈",
"寿美菜子",
"福山润",
"内田真礼",
"长妻树里",
"赤崎千夏",
"保志总一朗",
"设乐麻美",
"浅仓杏美",
"上坂堇",
"仙台惠理",
"洲崎绫",
"田丸笃志",
"金子有希",
"黑泽朋世",
"安济知佳",
"朝井彩加",
"石谷春贵",
"藤村鼓乃美",
"种崎敦美",
"东山奈央",
"樱井孝宏",
"沼仓爱美",
"田村睦心",
"桑原由气",
"长绳麻理亚",
"高田忧希",
"高桥未奈美",
"岭内知美",
"石原夏织",
"中原麻衣",
"野中蓝",
"田所梓",
"金元寿子",
"川上伦子",
"冈本麻见",
"柚木凉香",
"久川绫",
"冬马由美",
"田村由香里",
"西村千奈美",
"斋藤千和",
"兴梠里美",
"置鲇龙太郎",
"井上喜久子",
"广桥凉",
"神田朱未",
"桑岛法子",
"雪野五月",
"榎本温子",
"麻生美代子",
"小伏伸之",
"古贺葵",
"上田丽奈",
"本渡枫",

        };
        #endregion



        #region 角色能力设定

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
            /// 己方全部
            /// </summary>
            AllInTeam,

            /// <summary>
            /// 敌方全部
            /// </summary>
            AllOfEnemy,

            /// <summary>
            /// 己方场上随机一位角色
            /// </summary>
            RandomInTeam,

            /// <summary>
            /// 地方场上随机一位角色
            /// </summary>
            RandomOfEnemy,

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
            CharacterName,
            Team,
            Gender,
            CV,
            Anime,
            Tag,
            Power,
            HealthPoint,
            Silence,
            Ridicule,
           // State,
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

            /// <summary>
            /// 使用表达式
            /// </summary>
            expression,

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
            Round,

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

        public static string AbilityChineseIntroduction<T>(T abilityEnum)
        {

            var type = abilityEnum.GetType();

            #region 对能力类型的判定

            if (type == typeof(CardAbilityTypes))
            {
                var text = "触发";

                switch (abilityEnum)
                {
                    case CardAbilityTypes.Debut:
                        return $"出场时{text}";

                    case CardAbilityTypes.Exit:
                        return $"被击退时{text}";

                    case CardAbilityTypes.GetHurt:
                        return $"受伤时{text}";

                    case CardAbilityTypes.None:
                        return "不触发能力or无能力";

                    case CardAbilityTypes.Round:
                        return $"每回合{text}";

                    default:
                        return "不触发能力";
                }
            }

            #endregion

            #region 参数

            else if (type == typeof(Parameter))
            {
                switch (abilityEnum)
                {
                    case Parameter.Anime:
                        return "所属动画";

                    case Parameter.CharacterName:
                        return "角色名称";

                    case Parameter.Team:
                        return "所属队伍（社团）";

                    case Parameter.Coin:
                        return "硬币数量";

                    case Parameter.CV:
                        return "CV名称";

                    case Parameter.HealthPoint:
                        return "体力值";

                    case Parameter.None:
                        return "不涉及参数";

                    case Parameter.Power:
                        return "执行力";

                    case Parameter.Ridicule:
                        return "剩余嘲讽回合数";

                    case Parameter.Tag:
                        return "标签";

                    case Parameter.Gender:
                        return "角色性别";
                }
            }


            #endregion

            #region 对象检索范围（大范围）

            else if (type == typeof(Objects))
            {
                switch (abilityEnum)
                {
                    case Objects.Activator:
                        return "触发器";

                    case Objects.AllInTeam:
                        return "己方全部卡牌";

                    case Objects.AllOfEnemy:
                        return "敌方全部卡牌";

                    case Objects.AllOnSpot:
                        return "场上所有卡牌";

                    case Objects.Any:
                        return "任何情况下都可以";

                    case Objects.ChiefOfEnemy:
                        return "敌方部长";

                    case Objects.Last:
                        return "己方上位卡牌";

                    case Objects.Next:
                        return "己方下位卡牌";

                    case Objects.None:
                        return "不设定范围";

                    case Objects.OurChief:
                        return "己方部长卡牌";

                    case Objects.RandomInTeam:
                        return "己方随机一位卡牌";

                    case Objects.RandomOfEnemy:
                        return "敌方随机一位卡牌";

                    case Objects.Self:
                        return "卡牌自身";

                }
            }

            #endregion

            #region 判定方法

            else if (type == typeof(JudgeMethod))
            {
                switch (abilityEnum)
                {
                    case JudgeMethod.Count:
                        return "计数";

                    case JudgeMethod.Value:
                        return "取值";
                }
            }

            else if (type == typeof(CalculationMethod))
            {
                switch (abilityEnum)
                {
                    case CalculationMethod.addition:
                        return "加法运算";

                    case CalculationMethod.multiplication:
                        return "乘法运算";

                    case CalculationMethod.ChangeTo:
                        return "数值变更";

                    case CalculationMethod.expression:
                        return "[还没完成的功能]";
                }
            }

            #endregion


            return string.Empty;


        }

        #endregion


        #region 角色标签（默认值 可以按照萌娘百科萌点来）





        #endregion


        #region 游戏逻辑用 
        /// <summary>
        /// 打架中的游戏模式
        /// </summary>
        public enum PauseModeOfBattle
        {
            Legacy,
            EachCard,
            EachEnemyCard,
            EachOurCard,
        }
        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum GameState
        {
            /// <summary>
            /// 准备游戏。在所有玩家准备之前/其他情况下，处于此阶段
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
        #endregion



        #region  暂时不用了的羁绊


        /*
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
                      */

        #endregion

   


    }




    public class Bundle
    {
        public CardBundlesManifest manifest = new();
        public CharacterCard[] cards = new CharacterCard[0];
        public string manifestFullPath;

        public Bundle(CardBundlesManifest manifest, CharacterCard[] cards)
        {
            this.manifest = manifest;
            this.cards = cards;
            manifestFullPath = null;
        }
        public Bundle(CardBundlesManifest manifest, CharacterCard[] cards, string manifestFullPath)
        {
            this.manifest = manifest;
            this.cards = cards;
            this.manifestFullPath = manifestFullPath;
        }

        public Bundle()
        {
            CardBundlesManifest manifest = new();
            CharacterCard[] cards = new CharacterCard[0];
            manifestFullPath = null;
        }

        //定义一个隐式转换
        public static implicit operator BundleOfCreate(Bundle c)
        {
            var s = new BundleOfCreate
            {
                manifest = c.manifest
            };
            return s;
        }
    }
}

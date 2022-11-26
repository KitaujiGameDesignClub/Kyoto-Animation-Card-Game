using System;
using System.Collections;
using System.Collections.Generic;
using KitaujiGameDesignClub.GameFramework.Tools;

namespace  Core
{
    /// <summary>
    /// 为每一个卡组列一个清单
    /// </summary>
    [System.Serializable]
    public class CardBundlesManifest
    {
        /// <summary>
        /// 清单文件版本号（从文件中读取的）
        /// </summary>
        public string version = Information.ManifestVersion;


        /// <summary>
        /// （要求英文）卡包名称。（可以自定义，一般是番剧名称，额外添加一些修饰符什么的，比如 中二世界-中二病也要谈恋爱、与中二病也要谈恋爱就分属两个不同的阵营了）
        /// 最终会作为路径，和各类文件（夹）的名字
        /// </summary>
        public string BundleName = "DefaultBundle";

        /// <summary>
        /// 友好型名称。在游戏中会显示此卡包名称。（可以中文）
        /// </summary>
        public string FriendlyBundleName = "默认卡包（支持TMP_text的富文本）";

        /// <summary>
        /// 图片名字
        /// </summary>
        public string ImageName = String.Empty;

        /// <summary>
        /// 此卡包作者名称
        /// </summary>
        public string AuthorName = "作者名称（支持TMP_text的富文本）";

        /// <summary>
        /// 卡包介绍（支持TMP_text的富文本）
        /// </summary>
        public string Description = "卡包介绍（支持TMP_text的富文本）";

        /// <summary>
        /// 卡包备注。只在卡包制作器中显示，不会对玩家显示
        /// </summary>
        public string Remarks = "游戏默认卡包。可以按照提示进行修改。此处的备注只会在卡包中显示，不会对玩家显示";


        /// <summary>
        /// 储存所有的卡牌（便于游戏和编辑卡包中使用）
        /// </summary>
        [NonSerialized] public List<CharacterCard> AllCards = new List<CharacterCard>();

        /// <summary>
        /// 按照羁绊类型和羁绊层把所有的卡牌归纳一下（便于游戏和编辑卡包中使用）
        /// </summary>
        [NonSerialized] public ConnectWithCardName[] ConnectsCategorization = Array.Empty<ConnectWithCardName>();

        /// <summary>
        /// 记录一下在那里读写
        /// </summary>
        [NonSerialized]
        public KitaujiGameDesignClub.GameFramework.Tools.DescribeFileIO io =
            new DescribeFileIO("DefaultBundle.KgdCardBundles", $"temp/NewBundle");
        
        /// <summary>
        /// 读取所有的卡牌，得到上面两个不序列化的变量的值，并进行后续的分析
        /// </summary>
        public void Analysis()
        {
            
        }
        
    }


    /// <summary>
    /// 对某一个卡牌的羁绊进行记录（弄个列表，方便查询）
    /// </summary>
    [System.Serializable]
    public struct ConnectWithCardName
    {
        /// <summary>
        ///羁绊类型与羁绊层
        /// </summary>
       public CharactersConnect ConnectsCategorization;

        public string CardName;

        public string FriendlyCardName;
    }
}



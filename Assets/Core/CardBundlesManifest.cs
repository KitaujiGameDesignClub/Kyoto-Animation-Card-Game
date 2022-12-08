using System;
using System.Collections;
using System.Collections.Generic;
using KitaujiGameDesignClub.GameFramework.Tools;

namespace  Core
{
    /// <summary>
    /// 为每一个卡组列一个清单（卡包清单）
    /// </summary>
    [System.Serializable]
    public class CardBundlesManifest
    {
        /// <summary>
        /// 清单文件代码版本号（如果代码上有修改，且修改后不兼容，就+1）
        /// </summary>
        public int CodeVersion = Information.ManifestVersion;


        /// <summary>
        /// （要求英文）卡包名称。（可以自定义，一般是番剧名称，额外添加一些修饰符什么的，比如 中二世界-中二病也要谈恋爱、与中二病也要谈恋爱就分属两个不同的阵营了）
        /// 最终会作为路径，和各类文件（夹）的名字
        /// </summary>
        public string BundleName = "BundleManifest";

        /// <summary>
        /// 友好型名称。在游戏中会显示此卡包名称。（可以中文）
        /// </summary>
        public string FriendlyBundleName = "默认卡包";

        /// <summary>
        /// 卡包版本。用于玩家和作者进行区分
        /// </summary>
        public string BundleVersion = "v1.0";
        
        /// <summary>
        /// 图片名字
        /// </summary>
        public string ImageName ="default";

        /// <summary>
        /// 此卡包作者名称
        /// </summary>
        public string AuthorName = "作者名称";

        /// <summary>
        ///卡包简短介绍（支持TMP_text的富文本） 
        /// </summary>
        public string shortDescription = "卡包简短介绍 ";
        
        /// <summary>
        /// 卡包介绍（支持TMP_text的富文本）
        /// </summary>
        public string Description = "卡包介绍";

        /// <summary>
        /// 卡包备注。只在卡包制作器中显示，不会对玩家显示
        /// </summary>
        public string Remarks = "游戏默认卡包。可以按照提示进行修改。此处的备注只会在卡包中显示，不会对玩家显示";


        /// <summary>
        /// 卡牌数量
        /// </summary>
        public int cardNumber = 0;

      
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



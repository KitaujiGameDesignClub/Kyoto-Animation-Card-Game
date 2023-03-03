
namespace  Core
{
    /// <summary>
    /// 为每一个卡组列一个清单（卡包清单）
    /// </summary>
    [System.Serializable]
    public class CardBundlesManifest
    {
        public string UUID = string.Empty;
        
        /// <summary>
        /// 清单文件代码版本号（如果代码上有修改，且修改后不兼容，就+1）
        /// </summary>
        public int CodeVersion = Information.ManifestVersion;


        /// <summary>
        /// （要求英文）卡包名称。（可以自定义，一般是番剧名称，额外添加一些修饰符什么的，比如 中二世界-中二病也要谈恋爱、与中二病也要谈恋爱就分属两个不同的阵营了）
        /// 最终会作为路径，和各类文件（夹）的名字
        /// </summary>
        public string BundleName = "BundleManifest";

        public bool OtherContent = false;

        /// <summary>
        /// 友好型名称。在游戏中会显示此卡包名称。（可以中文）
        /// </summary>
        public string FriendlyBundleName = "默认卡包";

        public string Anime;

        /// <summary>
        /// 封面图片（含拓展名）
        /// </summary>
        public string ImageName;
        
        /// <summary>
        /// 卡包版本。用于玩家和作者进行区分
        /// </summary>
        public string BundleVersion = "v1.0";

        /// <summary>
        /// 此卡包作者名称
        /// </summary>
        public string AuthorName = "作者名称";

        /// <summary>
        /// 卡包介绍（支持TMP_text的富文本）
        /// </summary>
        public string Description = "卡包介绍";

        /// <summary>
        /// 卡包备注。只在卡包制作器中显示，不会对玩家显示
        /// </summary>
        public string Remarks = "这是你新建的卡组。从这里开始创作！";


        public override string ToString()
        {
            YamlDotNet.Serialization.Serializer serializer = new();
            return serializer.Serialize(this);
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



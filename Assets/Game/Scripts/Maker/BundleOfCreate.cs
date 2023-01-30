
using Core;
using System.Collections.Generic;

namespace Maker
{
    /// <summary>
    /// 目前正在编辑的manifest 和 card（每次只能编辑一个card，且与manifest不共存）
    /// </summary>
    /// 
    public class BundleOfCreate
    {
        public CardBundlesManifest manifest;
        public CharacterCard card;

        /// <summary>
        /// 记录这个卡组内，所有卡牌的友好名称
        /// </summary>
        public List<string> allCardsFriendlyName = new ();
        /// <summary>
        /// 记录这个卡组内，所有卡牌的识别名称（这两个名称数组是要相互对应的
        /// </summary>
        public List<string> allCardsName = new ();

        /// <summary>
        /// 上次加载的清单文件的完整路径
        /// </summary>
        public string loadedManifestFullPath = string.Empty;
        /// <summary>
        /// 上次加载的卡牌文件的完整路径
        /// </summary>
        public string loadedCardFullPath = string.Empty;
        
        public BundleOfCreate()
        {
            manifest = new();
            card = new CharacterCard();
            allCardsFriendlyName = new ();
            allCardsName = new ();
            loadedManifestFullPath = string.Empty;
        }
    }
    
    
}


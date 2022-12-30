
using System.Collections.Generic;

namespace Core
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
        public string[] allCardsFriendlyName;

        public BundleOfCreate()
        {
            manifest = new();
            card = new CharacterCard();
        }
    }
    
    public class Bundle
    {
        public CardBundlesManifest manifest;
        public CharacterCard[] cards;

        /// <summary>
        /// 记录这个卡组内，所有卡牌的友好名称
        /// </summary>
        public string[] allCardsFriendlyName;

        public Bundle()
        {
            manifest = new();
            cards = new CharacterCard[0];
        }
    }
}


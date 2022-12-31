
using System;
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
        /// <summary>
        /// 记录这个卡组内，所有卡牌的识别名称（这两个名称数组是要相互对应的
        /// </summary>
        public string[] allCardsName;

        /// <summary>
        /// 尝试获取每个卡组的清单文件的路径
        /// </summary>
        public string loadedManifestFullPath = string.Empty;
        /// <summary>
        /// 上次加载东西用的路径
        /// </summary>
        public string lastFileBrowerFullPath = string.Empty;
        
        public BundleOfCreate()
        {
            manifest = new();
            card = new CharacterCard();
            allCardsFriendlyName = Array.Empty<string>();
            allCardsName= Array.Empty<string>();
            
        }
    }
    
    public class Bundle
    {
        public CardBundlesManifest manifest;
        public CharacterCard[] cards;
        

        public Bundle()
        {
            manifest = new();
            cards = new CharacterCard[0];
        }
    }
}

